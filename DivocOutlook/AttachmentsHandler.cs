using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Windows.Forms;
using System.IO;
using DivocCommon;

namespace DivocOutlook
{
    public class AttachmentsHandler
    {
        private readonly Outlook.MailItem _mailItem;
        readonly Outlook.Attachments _attachments;
        private readonly IWin32Window _parentWindow;

        public AttachmentsHandler(IWin32Window window, ref Outlook.MailItem mailItem)
        {
            if (window != null) _parentWindow = window;

            if (mailItem != null)
            {
                _mailItem = mailItem;
                _attachments = _mailItem.Attachments;
            }
        }

        /// <summary>
        /// Upload attachments from new email to drive and replace the attachments with links to the saved items.
        /// </summary>
        /// <notes>
        /// If there are attachments...
        /// * Prompt the user to save them and replace with links
        /// * If they chose to do so, upload the attachments, remove attachments from email, replace with links html
        /// </notes>
        /// <returns>Flag if user has cancelled.</returns>
        public bool UploadAndLink()
        {
            if (_attachments != null && _attachments.Count == 0)    // Nothing to do if there are no attachments.
                return false;

            bool cancel = false;

            try
            {
                string prompt = ResourceBroker.GetString(ResourceBroker.ResourceID.UPLOAD_AND_LINK_PROMPT);
                string caption = ResourceBroker.GetString(ResourceBroker.ResourceID.UPLOAD_AND_LINK_CAPTION);

                DialogResult dlgRes = MessageBox.Show(_parentWindow, prompt, caption, MessageBoxButtons.YesNoCancel);

                if (DialogResult.Yes == dlgRes)
                {
                    string parentId = ThisAddIn.ContentManager.BrowseForLocation();

                    if (!string.IsNullOrEmpty(parentId))
                    {
                        string userTempPath = Path.GetTempPath();
                        List<KeyValuePair<string, string>> fileInfoList = new List<KeyValuePair<string, string>>();

                        foreach (Outlook.Attachment attach in _mailItem.Attachments)
                        {
                            string fileName = attach.FileName;
                            string filePath = userTempPath + fileName;
                            attach.SaveAsFile(filePath);    // This will except with embedded images
                            fileInfoList.Add(new KeyValuePair<string, string>(fileName, filePath));
                        }

                        List<(string, string)> savedItems = ThisAddIn.ContentManager.SaveWithProgress(fileInfoList, parentId);

                        if (savedItems != null && savedItems.Count > 0)
                        {
                            string emlTemplate = ResourceBroker.GetString(ResourceBroker.ResourceID.EMAIL_LINKS_BLOCK_TEMPLATE);

                            StringBuilder strBldr = new StringBuilder();

                            foreach ((string name, string webDavUrl) in savedItems)
                            {
                                strBldr.AppendLine("<tr><td><a href=\"" + webDavUrl + "\">" + name + "</a></td></tr>");
                            }

                            _mailItem.HTMLBody = emlTemplate.Replace("{{webDavUrls}}", strBldr.ToString()) + _mailItem.HTMLBody;
                            _mailItem.Save();

                            // Shoud be OK now to remove the attachments...
                            int count = _attachments.Count;

                            while(count > 0)
                            {
                                _attachments.Remove(count);
                                count--;
                            }
                        }
                    }
                }
                else if (DialogResult.Cancel == dlgRes)
                {
                    cancel = true;
                }
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
                cancel = true;
            }

            return cancel;
        }
    }
}
