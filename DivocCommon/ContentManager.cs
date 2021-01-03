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
    ///     * Improve relationship with authentication.This is all dependent on the user being authenticated and needs hardening. Save for after proxy?
    ///     * Investigate filtering by file type.No reason to open PPT files from within Word, etc.Currently not available in the API because Microsoft.
    /// </TODO>
    public class ContentManager
    {

        string rootDriveId = string.Empty;  // id of the default drive of the default site of the tenant
        string rootItemId = string.Empty;   // id of the root item in the default drive of the default site of the tenant

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

        private async void Init()
        {
            rootItemId = await GetTenantRootREST();
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
        private async Task<string> GetTenantRootREST()
        {
            string root = string.Empty;
            var httpClient = new HttpClient();
            HttpResponseMessage response;
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, EndPoints.DefaultSiteDefaultDriveRootItem);
                //Add the token in Authorization header
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthenticationManager.AccessToken);
                response = await httpClient.SendAsync(request);
                string strContent = await response.Content.ReadAsStringAsync();
                dynamic content = JsonConvert.DeserializeObject(strContent);
                root = content.id;
                rootDriveId = content.parentReference.driveId;
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
            }

            return root;
        }

        /// <summary>
        /// Get the children for a specified parentId (or the root item of the drive, if empty) using the Graph REST api
        /// </summary>
        /// <TODO>
        ///     * return the collection of children in a meaningful way
        ///     * investigate if there's 'chunking' for large collections so as not to bog the apps down
        /// </TODO>
        /// <param name="parentId">Id of the parent item to query for contents</param>
        public async void GetDocumentsREST(string parentId = "")
        {
            var httpClient = new HttpClient();
            HttpResponseMessage response;
            try
            {
                // Default to the root item in the root drive if parentId is empty
                var request = new HttpRequestMessage(HttpMethod.Get, EndPoints.ChildrenForItem(rootDriveId, (parentId.Length > 0) ? parentId : rootItemId));
                //Add the token in Authorization header
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthenticationManager.AccessToken);
                response = await httpClient.SendAsync(request);
                string strContent = await response.Content.ReadAsStringAsync();
                dynamic content = JsonConvert.DeserializeObject(strContent);
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
            }
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
        public async void SaveDocumentsREST(List<KeyValuePair<string, string>> fileInfoList, string parentId = "")
        {
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

                                response = await httpClient.PutAsync(EndPoints.NewItem(rootDriveId, (parentId.Length > 0) ? parentId : rootItemId, info.Key), streamContent);
                                string strContent = await response.Content.ReadAsStringAsync();
                                dynamic content = JsonConvert.DeserializeObject(strContent);
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
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
            }
        }
    }
}
