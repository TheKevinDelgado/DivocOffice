using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Office = Microsoft.Office.Core;
using Outlook = Microsoft.Office.Interop.Outlook;
using DivocCommon;
using System.Windows.Forms;
using System.Drawing;

// TODO:  Follow these steps to enable the Ribbon (XML) item:

// 1: Copy the following code block into the ThisAddin, ThisWorkbook, or ThisDocument class.

//  protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
//  {
//      return new ExplorerRibbon();
//  }

// 2. Create callback methods in the "Ribbon Callbacks" region of this class to handle user
//    actions, such as clicking a button. Note: if you have exported this Ribbon from the Ribbon designer,
//    move your code from the event handlers to the callback methods and modify the code to work with the
//    Ribbon extensibility (RibbonX) programming model.

// 3. Assign attributes to the control tags in the Ribbon XML file to identify the appropriate callback methods in your code.  

// For more information, see the Ribbon XML documentation in the Visual Studio Tools for Office Help.


namespace DivocOutlook
{
    [ComVisible(true)]
    public class OutlookRibbonManager : OfficeRibbonManagerBase, Office.IRibbonExtensibility
    {
        public OutlookRibbonManager()
        {
        }

        #region IRibbonExtensibility Members

        public override string GetCustomUI(string ribbonID)
        {
            LogManager.LogMethod(string.Format("Ribbon Id: {0}", ribbonID));

            string ribbonUI;

            switch(ribbonID)
            {
                case "Microsoft.Outlook.Explorer":
                    ribbonUI = GetResourceText("DivocOutlook.RibbonExplorer.xml");
                    break;

                case "Microsoft.Outlook.Mail.Read":
                    ribbonUI = GetResourceText("DivocOutlook.RibbonInspectorRead.xml");
                    break;

                case "Microsoft.Outlook.Mail.Compose":
                    ribbonUI = GetResourceText("DivocOutlook.RibbonInspectorCompose.xml");
                    break;

                default:
                    ribbonUI = base.GetCustomUI(ribbonID);
                    break;
            }

            return ribbonUI;
        }

        #endregion

        #region Ribbon Callbacks
        //Create callback methods here. For more information about adding callback methods, visit https://go.microsoft.com/fwlink/?LinkID=271226

        public override void OnAction(Office.IRibbonControl control)
        {
            try
            {
                dynamic context = control.Context;

                if(context is Outlook.Explorer)
                {
                    HandleExplorerAction(control);
                }
                else if(context is Outlook.Inspector)
                {
                    HandleInspectorAction(control);
                }
                else
                {
                    base.OnAction(control);
                }
            }
            catch(Exception ex)
            {
                LogManager.LogException(ex);
            }
        }

        private void HandleExplorerAction(Office.IRibbonControl control)
        {
            try
            {
                LogManager.LogMethod(string.Format("Ribbon Control Id: {0}", control.Id));

                Outlook.Explorer expl = control.Context as Outlook.Explorer;

                if(expl != null)
                {
                    switch (control.Id)
                    {
                        case RibbonIDs.SAVE_MAIL:
                            SaveEmails(expl, ExplorerWrapper.GetHandleForExplorer(expl));
                            break;

                        case RibbonIDs.SAVE_ATTACHMENTS:
                            SaveAttachments(expl, ExplorerWrapper.GetHandleForExplorer(expl));
                            break;

                        case RibbonIDs.INSERT_ATTACHMENTS:
                            InsertAttachments(expl.ActiveInlineResponse, ExplorerWrapper.GetHandleForExplorer(expl));
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
            }
        }

        private static void SaveEmails(Outlook.Explorer expl, IntPtr wnd = default)
        {
            // * Get the selection of emails
            // * Save them to user's temp dir
            // * Pass list of file paths to the contentmanager for upload
            // * Content manager will delete the temps
            if (expl.Selection.Count > 0 && expl.Selection[1] is Outlook.MailItem)
            {
                string parentId = ThisAddIn.ContentManager.BrowseForLocation(wnd);

                if (!string.IsNullOrEmpty(parentId))
                {
                    string userTempPath = Path.GetTempPath();
                    List<KeyValuePair<string, string>> fileInfoList = new List<KeyValuePair<string, string>>();

                    foreach (Outlook.MailItem item in expl.Selection)
                    {
                        string fileName = item.Subject + ".msg";

                        // Possibly have invalid characters so fix that...
                        fileName = Helpers.CleanFilename(fileName);

                        string filePath = userTempPath + fileName;
                        item.SaveAs(filePath, Outlook.OlSaveAsType.olMSGUnicode);
                        fileInfoList.Add(new KeyValuePair<string, string>(fileName, filePath));
                    }

                    ThisAddIn.ContentManager.SaveWithProgress(fileInfoList, parentId);
                }
            }
        }

        private static void SaveAttachments(Outlook.Explorer expl, IntPtr wnd = default)
        {
            // * Get the selection of emails
            // * Get the attachments from the emails
            // * Save them to user's temp dir
            // * Pass list of file paths to the contentmanager for upload
            // * Content manager will delete the temps
            if (expl.Selection.Count > 0 && expl.Selection[1] is Outlook.MailItem)
            {
                string parentId = ThisAddIn.ContentManager.BrowseForLocation(wnd);

                if (!string.IsNullOrEmpty(parentId))
                {
                    string userTempPath = Path.GetTempPath();
                    List<KeyValuePair<string, string>> fileInfoList = new List<KeyValuePair<string, string>>();

                    foreach (Outlook.MailItem item in expl.Selection)
                    {
                        foreach (Outlook.Attachment attach in item.Attachments)
                        {
                            string fileName = attach.FileName;
                            string filePath = userTempPath + fileName;
                            attach.SaveAsFile(filePath);
                            fileInfoList.Add(new KeyValuePair<string, string>(fileName, filePath));
                        }
                    }

                    ThisAddIn.ContentManager.SaveWithProgress(fileInfoList, parentId);
                }
            }
        }

        private static void HandleInspectorAction(Office.IRibbonControl control)
        {
            try
            {
                LogManager.LogMethod(string.Format("Ribbon Control Id: {0}", control.Id));

                Outlook.Inspector insp = control.Context as Outlook.Inspector;

                if(insp != null)
                {
                    Outlook.MailItem mail = insp.CurrentItem as Outlook.MailItem;

                    switch (control.Id)
                    {
                        case RibbonIDs.INSERT_ATTACHMENTS:
                            InsertAttachments(mail, InspectorWrapper.GetHandleForInspector(insp));
                            break;

                        case RibbonIDs.SAVE_ATTACHMENTS:
                            SaveAttachments(mail, InspectorWrapper.GetHandleForInspector(insp));
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
            }
        }

        private static void SaveAttachments(Outlook.MailItem email, IntPtr wnd = default)
        {
            // Save the email's attachments. May need to check each attachment
            // and make sure it isn't a signature image or such. Don't save those.

            if(email.Attachments.Count > 0) // Should be filtered via enablement but just in case
            {
                string parentId = ThisAddIn.ContentManager.BrowseForLocation(wnd);

                if (!string.IsNullOrEmpty(parentId))
                {
                    string userTempPath = Path.GetTempPath();
                    List<KeyValuePair<string, string>> fileInfoList = new List<KeyValuePair<string, string>>();

                    foreach (Outlook.Attachment attach in email.Attachments)
                    {
                        string fileName = attach.FileName;
                        string filePath = userTempPath + fileName;
                        attach.SaveAsFile(filePath);
                        fileInfoList.Add(new KeyValuePair<string, string>(fileName, filePath));
                    }

                    ThisAddIn.ContentManager.SaveWithProgress(fileInfoList, parentId);
                }
            }
        }

        private static void InsertAttachments(Outlook.MailItem email, IntPtr wnd = default)
        {
            string itemUrl = ThisAddIn.ContentManager.BrowseForItem(wnd: wnd);
            if (!string.IsNullOrEmpty(itemUrl))
            {
                email.Attachments.Add(itemUrl);
            }
        }

        public override bool OnGetEnabled(Office.IRibbonControl control)
        {
            bool enabled = true;

            try
            {
                LogManager.LogMethod(string.Format("Ribbon Control Id: {0}", control.Id));

                dynamic context = control.Context;

                if (context is Outlook.Explorer)
                {
                    enabled = HandleExplorerEnablement(control);
                }
                else if(context is Outlook.Inspector)
                {
                    enabled = HandleInspectorEnablement(control);
                }
                else
                {
                    base.OnGetEnabled(control);
                }
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
            }

            return enabled;
        }

        private static bool HandleExplorerEnablement(Office.IRibbonControl control)
        {
            bool enabled = false;

            try
            {
                LogManager.LogMethod(string.Format("Ribbon Control Id: {0}", control.Id));

                Outlook.Explorer expl = control.Context as Outlook.Explorer;

                if (expl != null)
                {
                    if (control.Id == RibbonIDs.SETTINGS_LAUNCHER)
                        return enabled = false;

                    Outlook.Selection sel = expl.Selection;

                    if (sel is IEnumerable<Outlook.MailItem>)
                        MessageBox.Show("poing");

                    if(sel != null && sel[1] is Outlook.MailItem)
                    {
                        switch (control.Id)
                        {
                            case RibbonIDs.SAVE_MAIL:
                                if (sel.Count > 0)
                                    enabled = true;
                                break;

                            case RibbonIDs.SAVE_ATTACHMENTS:
                                if (sel.Count > 0)
                                {
                                    // Enable only when all selected items have attachments.
                                    // LINQ query to find items with attachments, compare count to selection count, et voila
                                    if (sel.Count == (from items in sel.Cast<Outlook.MailItem>() where items.Attachments.Count > 0 select items).Count())
                                        enabled = true;
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
            }

            return enabled;
        }

        private static bool HandleInspectorEnablement(Office.IRibbonControl control)
        {
            bool enabled = false;

            try
            {
                LogManager.LogMethod(string.Format("Ribbon Control Id: {0}", control.Id));

                Outlook.Inspector insp = control.Context as Outlook.Inspector;

                if (insp != null)
                {
                    switch(control.Id)
                    {
                        case RibbonIDs.SAVE_ATTACHMENTS:
                            Outlook.MailItem mail = insp.CurrentItem as Outlook.MailItem;

                            if (mail != null)
                            {
                                if (mail.Attachments.Count > 0)
                                    enabled = true;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
            }

            return enabled;
        }

        #endregion
    }
}
