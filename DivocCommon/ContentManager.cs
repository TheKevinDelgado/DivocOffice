using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DivocCommon.DataModel;
using DivocCommon.DataModel.Teams;
using Forms = System.Windows.Forms;
using System.Text.RegularExpressions;

namespace DivocCommon
{
    /// <summary>
    /// Class for handling getting, updating and uploading files/folders
    /// </summary>
    /// <notes>
    /// Two Options:
    ///     * REST api calls over http
    ///     * Microsoft Grapi.Net API
    /// 
    /// Separate into two sets of calls (or derive classes, whatever).
    /// REST stuff first, since this is ideal for moving to an
    /// out of process proxy that the Add-ins can call, just like
    /// the authentication can/should be moved to shared proxy.
    /// </notes>
    /// <TODO>
    ///     * Start burying the Graph calls behind specialized UI as much as possible?
    ///     * Authentication moved into this scope. Now need to add something for add-ins to check to see if the user is authenticated so that
    ///         menu items,e tc can be properly enabled. May need to have a manual login option in case the user accidentally cancels out of
    ///         the login dialog so they don't have to close and re-open the application. Or, leave everything in default enabled states, but 
    ///         if the user is not authenticated, bring up the authentication window and let the proceed afterwards.
    ///     * Investigate filtering by file type.No reason to open PPT files from within Word, etc.Currently not available in the API because Microsoft.
    /// </TODO>
    public class ContentManager
    {
        /// <summary>
        /// Root item in the default drive of the default site of the tenant.
        /// </summary>
        public DriveItem Root { get; private set; }

        readonly AuthenticationManager auth = new AuthenticationManager();

        List<TeamInfo> _teams = null;

        public ContentManager()
        {
            try
            {
                Init();
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
            }
        }

        /// <summary>
        /// Initialization, including authentication.
        /// </summary>
        /// <notes>
        /// For fun, we're going to add some rudimentary MSTeams integration.
        /// </notes>
        private async void Init()
        {
            await auth.Authenticate(IntPtr.Zero);
            Root = await GetTenantRootREST();
            _teams = await GetUsersTeams();

            foreach(TeamInfo info in _teams)
            {
                info.Channels = await GetChannelsForTeam(info.id);
            }
        }

        /// <summary>
        /// Get all of the Teams the user is a member of.
        /// </summary>
        /// <returns>List of <see cref="T:DivocCommon.DataModel.TeamInfo">TeamInfo</see> objects found</returns>
        private static async Task<List<TeamInfo>> GetUsersTeams()
        {
            List<TeamInfo> teams = null;

            var httpClient = new HttpClient();
            HttpResponseMessage response;
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, EndPoints.JoinedTeams);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthenticationManager.AccessToken);
                response = await httpClient.SendAsync(request);
                string strContent = await response.Content.ReadAsStringAsync();

                TeamResultSet results = (TeamResultSet)JsonConvert.DeserializeObject(strContent, typeof(TeamResultSet));
                teams = results.Items;
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
            }

            return teams;
        }

        /// <summary>
        /// Get all of the channels for a given team.
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns>List of <see cref="T:DivocCommon.DataModel.ChannelInfo">ChannelInfo</see> objects found</returns>
        private static async Task<List<ChannelInfo>> GetChannelsForTeam(string teamId)
        {
            List<ChannelInfo> channels = null;

            var httpClient = new HttpClient();
            HttpResponseMessage response;
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, EndPoints.ChannelsForTeam(teamId));
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthenticationManager.AccessToken);
                response = await httpClient.SendAsync(request);
                string strContent = await response.Content.ReadAsStringAsync();

                ChannelResultSet results = (ChannelResultSet)JsonConvert.DeserializeObject(strContent, typeof(ChannelResultSet));
                channels = results.Items;
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
            }

            return channels;
        }

        /// <summary>
        /// Just for fun, send a message to the user's first team and first channel
        /// </summary>
        /// <param name="message"></param>
        public async void SendMessageToTeams(string message)
        {
            try
            {
                if(_teams.Count > 0 && _teams[0].Channels.Count > 0)
                {
                    HttpResponseMessage response;

                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + AuthenticationManager.AccessToken);

                        ChannelMessageInfo msg = new ChannelMessageInfo
                        {
                            body = new ChannelMessageBody
                            {
                                contentType = "html",
                                content = message
                            }
                        };

                        StringContent content = new StringContent(JsonConvert.SerializeObject(msg), Encoding.UTF8, "application/json");

                        response = await httpClient.PostAsync(EndPoints.MessageToChannel(_teams[0].id, _teams[0].Channels[0].id), content);
                        string strContent = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
            }
        }

        /// <summary>
        /// Show UI for user to select an item to open in the given application.
        /// </summary>
        /// <param name="fileTypes">List of <see cref="T:DivocCommon.ItemMimeTypes">ItemMimeTypes</see> to filter on.</param>
        /// <returns>WebDav URL for the selected item to open.</returns>
        public string BrowseForItem(List<string> fileTypes = null)
        {
            string itemUrl = null;

            BrowseHostForm browse = new BrowseHostForm(this, fileTypes);

            if (Forms.DialogResult.OK == browse.ShowDialog())
            {
                itemUrl = browse.ItemUrl;
            }

            return itemUrl;
        }

        /// <summary>
        /// Show UI for user to select a location to store an item.
        /// </summary>
        /// <notes>This is the reverse of BrowseForItem, which allows the user to select
        /// an item to be opened by the application. Here, the user is selecting a location
        /// into which they want to save the items. For Word, PPT, and Excel, this will only
        /// ever be a single document.(?) But in the case of Outlook, a user may be saving 
        /// many emails or attachments. For single item save, an input for file name should
        /// be visible and auto-populated with a suggested default name, handled by each
        /// add-in. For multiple items...?
        /// Debating on whether or not to do the saving in here or just ship the location
        /// id back to the caller. Everything should already be saved to the user's temp
        /// directory at this point, but that order of operations could be changed.
        /// </notes>
        /// <TODO>
        ///     * Handle the inevitable name-collisions?
        /// </TODO>
        /// <param name="fileNameDefaultsList">List of default names of new items</param>
        /// <returns>Id of the location to be used for the new item.</returns>
        public string BrowseForLocation()
        {
            string itemId = null;

            BrowseHostForm browse = new BrowseHostForm(this, location: true);

            if (Forms.DialogResult.OK == browse.ShowDialog())
            {
                itemId = browse.ItemId;

                if (string.IsNullOrEmpty(itemId))
                    itemId = Root.Id;
            }

            return itemId;
        }

        /// <summary>
        /// Get the tenant's default drive's root item id, as well as the id for the default drive
        /// </summary>
        /// <notes>
        /// It would be possible to get all of the drives/lists for a site as well and allow a
        /// user to select which one they want to use. You could go even further and get all
        /// of the sites for the tenant and allowe the user to select the site they want to
        /// use and then use either the default drive for the site or allow them to select
        /// the one they want to use from the available drives/lists on the site.
        /// </notes>
        /// <returns>The id of the root item in the default drive of the default site</returns>
        private static async Task<DriveItem> GetTenantRootREST()
        {
            DriveItem root = null;
            var httpClient = new HttpClient();
            HttpResponseMessage response;
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, EndPoints.DefaultSiteDefaultDriveRootItem);
                //Add the token in Authorization header
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthenticationManager.AccessToken);
                response = await httpClient.SendAsync(request);
                string strContent = await response.Content.ReadAsStringAsync();
                root = (DriveItem)JsonConvert.DeserializeObject(strContent, typeof(DriveItem));
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
            }

            return root;
        }

        /// <summary>
        /// Get the children for a specified parentId (or the root item of the drive, if empty) using the Graph REST api.
        /// </summary>
        /// <remarks>
        /// If parentId is omitted, the root item of the drive is used. If fileTypes is omitted, all types are returned.
        /// If fileTypes are specified, folder items will also be returned to allow for drill-down.
        /// </remarks>
        /// <TODO>
        ///     * return the collection of children in a meaningful way
        ///     * investigate if there's 'chunking' for large collections so as not to bog the apps down
        /// </TODO>
        /// <param name="parentId">Id of the parent item to query for contents</param>
        /// <param name="fileTypes">List of mime type strings to filter by</param>
        /// <returns>List of <see cref="T:DivocCommon.DataModel.DriveItem">DriveItem</see> objects found</returns>
        public async Task<List<DriveItem>> GetDocumentsREST(string parentId = "", List<string> fileTypes = null)
        {
            List<DriveItem> items = null;

            var httpClient = new HttpClient();
            HttpResponseMessage response;
            try
            {
                // Default to the root item in the root drive if parentId is empty
                var request = new HttpRequestMessage(HttpMethod.Get, EndPoints.ChildrenForItem(Root.ParentReference.DriveId, (parentId.Length > 0) ? parentId : Root.Id));
                //Add the token in Authorization header
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthenticationManager.AccessToken);
                response = await httpClient.SendAsync(request);
                string strContent = await response.Content.ReadAsStringAsync();
                DriveItemResultSet results = (DriveItemResultSet)JsonConvert.DeserializeObject(strContent, typeof(DriveItemResultSet));

                items = results.Items.Where(i => (i.Folder != null) || ((fileTypes != null) ? fileTypes.Contains(i.File.MimeType) : (i.File != null))).ToList();
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
            }

            return items;
        }

        /// <summary>
        /// Perform a save operation with UI showing progress and allowing user cancellation.
        /// </summary>
        /// <notes>
        /// First pass for UI feedback mentioned in TODO for SaveDocumentsREST below.
        /// 
        /// This, along with SaveDocuments, gives the flexibility of doing the save operation with or
        /// without UI. In some cases, such as the Send event handler in Outlook Inspector, the 
        /// headless version can get 'lost' due to threading limitations (can't put 'async' on a
        /// method with a ref parameter, which makes the syncronizationcontext stuff not work).
        /// </notes>
        /// <param name="fileInfoList">List of KeyValuePairs [file name - file path on disk]</param>
        /// <param name="parentId">Id of the parent item to save the documents under</param>
        /// <returns>List of WebDav Urls for objects saved</returns>
        public List<(string, string)> SaveWithProgress(List<KeyValuePair<string, string>> fileInfoList, string parentId = "")
        {
            List<(string, string)> webDavUrls = null;

            SaveWithProgressForm saveDlg = new SaveWithProgressForm(this, fileInfoList, parentId);

            if (Forms.DialogResult.OK == saveDlg.ShowDialog())
            {
                webDavUrls = saveDlg.WebDavUrls;
            }

            return webDavUrls;
        }

        /// <summary>
        /// Upload documents to the drive
        /// </summary>
        /// <notes>
        /// Public call. Can switch guts of this to call GraphAPI or Graph REST as needed
        /// </notes>
        /// <param name="fileInfoList">List of KeyValuePairs [file name - file path on disk]</param>
        /// <param name="parentId">Id of the parent item to save the documents under</param>
        /// <returns>List of WebDav Urls for objects saved</returns>
        public async Task<List<(string, string)>> SaveDocuments(List<KeyValuePair<string, string>> fileInfoList, string parentId = "", IProgress<KeyValuePair<int, string>> progress = null)
        {
            List<(string, string)> webDavUrls = new List<(string, string)>();

            List<DriveItem> items = await SaveDocumentsREST(fileInfoList, parentId, progress);

            items.ForEach(item => webDavUrls.Add((item.Name, item.WebDavUrl)));

            return webDavUrls;
        }

        /// <summary>
        /// Upload the documents using the Graph REST api
        /// </summary>
        /// <TODO>
        ///     * Needs UI feedback mechanism to show progress, completion (with resultant info for new item) and errors.
        ///     * Basic method has a file size limit, so convert to use the 'chunking' version
        /// </TODO>
        /// <param name="fileInfoList">List of KeyValuePairs [file name - file path on disk]</param>
        /// <param name="parentId">Id of the parent item to save the documents under</param>
        protected async Task<List<DriveItem>> SaveDocumentsREST(List<KeyValuePair<string, string>> fileInfoList, string parentId = "", IProgress<KeyValuePair<int, string>> progress = null)
        {
            // Local function to deal with reporting progress if optional progress handler was passed in
            void ReportProgress(IProgress<KeyValuePair<int, string>> prog, int val, string name = "")
            {
                if(prog != null)
                {
                    prog.Report(new KeyValuePair<int, string>(val, name));
                }
            }

            int filesSaved = 0;

            List<DriveItem> items = new List<DriveItem>();

            HttpResponseMessage response;
            try
            {
                foreach(KeyValuePair<string, string> info in fileInfoList)
                {
                    using (FileStream fileStream = System.IO.File.OpenRead(info.Value))
                    {
                        long lbytes = fileStream.Length;

                        using (var streamContent = new StreamContent(fileStream, System.Convert.ToInt32(lbytes)))
                        {
                            using (var httpClient = new HttpClient())
                            {
                                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + AuthenticationManager.AccessToken);

                                response = await httpClient.PutAsync(EndPoints.NewItem(Root.ParentReference.DriveId, (parentId.Length > 0) ? parentId : Root.Id, info.Key), streamContent);
                                string strContent = await response.Content.ReadAsStringAsync();

                                DriveItem newItem = (DriveItem)JsonConvert.DeserializeObject(strContent, typeof(DriveItem));

                                if (newItem != null)
                                {
                                    items.Add(newItem);

                                    ReportProgress(progress, ++filesSaved, newItem.Name);
                                }
                            }
                        }
                    }

                    try
                    {
                        File.Delete(info.Value);
                    }
                    catch (Exception fileEx)
                    {
                        // Possible to have file delete exceptions...handle it.
                        LogManager.LogException(fileEx);
                    }
                }

                ReportProgress(progress, -1);
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
            }

            return items;
        }
    }
}
