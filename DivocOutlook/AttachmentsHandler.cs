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

        public static bool IsDivocAttachment(Outlook.Attachment attachment)
        {
            bool isDivocAttachment = false;

            dynamic divocProp = null;

            try
            {
                divocProp = attachment.PropertyAccessor.GetProperty(MAPIHelper.Prop_String);
            }
            catch(Exception ex)
            {
                // GetProperty can throw an exeption if the queried property doesn't exist. Fun. Just ignore it.

                LogManager.LogException(ex);
            }

            if (divocProp != null && MAPIHelper.Value_Attachment.Equals(divocProp))
                isDivocAttachment = true;

            return isDivocAttachment;
        }

        /// <summary>
        /// Upload attachments from new email to drive and replace the attachments with links to the saved items.
        /// </summary>
        /// <notes>
        /// If there are attachments...
        /// * Prompt the user to save them and replace with links
        /// * If they chose to do so, upload the attachments, remove attachments from email, replace with links html
        ///     * Must first make sure the attachments are not from Divoc - check MAPI property and filter those out first
        ///     * Any remaining attachments (user added from someplace other than Divoc) get processed
        /// </notes>
        /// <returns>Flag if user has cancelled.</returns>
        public bool UploadAndLink()
        {
            if (_attachments != null && _attachments.Count == 0)    // Nothing to do if there are no attachments.
                return false;

            bool cancel = false;

            try
            {
                // First, check to see if we have any attachments that actually need to be addressed.
                // If all the attachments were inserted from Divoc, we can just go on our merry way.
                // If not, then we need to address ONLY the ones that were not inserted from Divoc.

                List<int> NonDivocAttachments = new List<int>();

                foreach(Outlook.Attachment att in _mailItem.Attachments)
                {
                    if(!IsDivocAttachment(att))
                    {
                        if(att.Size > 0 || att.Type == Outlook.OlAttachmentType.olEmbeddeditem)
                        {
                            // Embedded pictures will have size = 0, we don't want them.
                            // Embedded emails will also have size 0 but we DO want them.

                            NonDivocAttachments.Add(att.Index);
                        }
                    }
                }

                // If there's nothing for us to potentially work on, we can just let the send go now...
                if (NonDivocAttachments.Count == 0)
                    return cancel;

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

                        foreach(int idx in NonDivocAttachments)
                        {
                            try
                            {
                                Outlook.Attachment unmanagedAtt = _mailItem.Attachments[idx];

                                if (unmanagedAtt != null)    // Should never be null but never trust MS
                                {
                                    string fileName = unmanagedAtt.FileName;
                                    string filePath = userTempPath + fileName;
                                    unmanagedAtt.SaveAsFile(filePath);    // This will except with embedded images
                                    fileInfoList.Add(new KeyValuePair<string, string>(fileName, filePath));
                                }
                            }
                            catch (Exception ex)
                            {
                                // Should never get here but never trust MS
                                LogManager.LogException(ex);
                            }
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

                            // Should be OK now to remove the attachments which have been uploaded and replaced with a link.
                            // But do not remove any other attachments! Work backwards due to index shifting.
                            while(NonDivocAttachments.Count > 0)
                            {
                                _attachments.Remove(NonDivocAttachments[NonDivocAttachments.Count - 1]);
                                NonDivocAttachments.RemoveAt(NonDivocAttachments.Count - 1);
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
