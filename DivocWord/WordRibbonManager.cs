using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Office = Microsoft.Office.Core;
using Word = Microsoft.Office.Interop.Word;
using DivocCommon;
using System.Windows.Forms;

// TODO:  Follow these steps to enable the Ribbon (XML) item:

// 1: Copy the following code block into the ThisAddin, ThisWorkbook, or ThisDocument class.

//  protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
//  {
//      return new Ribbon1();
//  }

// 2. Create callback methods in the "Ribbon Callbacks" region of this class to handle user
//    actions, such as clicking a button. Note: if you have exported this Ribbon from the Ribbon designer,
//    move your code from the event handlers to the callback methods and modify the code to work with the
//    Ribbon extensibility (RibbonX) programming model.

// 3. Assign attributes to the control tags in the Ribbon XML file to identify the appropriate callback methods in your code.  

// For more information, see the Ribbon XML documentation in the Visual Studio Tools for Office Help.


namespace DivocWord
{
    [ComVisible(true)]
    public class WordRibbonManager : OfficeRibbonManagerBase, Office.IRibbonExtensibility
    {
        public WordRibbonManager()
        {
        }

        #region IRibbonExtensibility Members

        public override string GetCustomUI(string ribbonID)
        {
            LogManager.LogMethod(string.Format("Ribbon Id: {0}", ribbonID));

            string ribbonUI;

            switch (ribbonID)
            {
                case "Microsoft.Word.Document":
                    ribbonUI = GetResourceText("DivocWord.RibbonWord.xml");
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

        public override bool OnGetEnabled(Office.IRibbonControl control)
        {
            bool enabled = false;

            try
            {
                LogManager.LogMethod(string.Format("Ribbon Control Id: {0}", control.Id));

                dynamic context = control.Context;

                switch (control.Id)
                {
                    case RibbonIDs.SAVE_DOCUMENT:
                        if (context != null)
                        {
                            enabled = true;
                        }

                        break;

                    case RibbonIDs.OPEN_DOCUMENT:
                        enabled = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
            }

            return enabled;
        }

        public override void OnAction(Office.IRibbonControl control)
        {     
            try
            {
                switch (control.Id)
                {
                    case RibbonIDs.SAVE_DOCUMENT:
                        Word.Document doc = control.Context.Document as Word.Document;
                        SaveDocument(doc);
                        break;

                    case RibbonIDs.OPEN_DOCUMENT:
                        OpenDocument();
                        break;
                }
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
            }
        }

        #endregion

        #region Internal Methods

        private static async void SaveDocument(Word.Document doc)
        {
            string fileName = string.Empty;

            // Try to get a default name similar to how Word does natively:
            // * Document title first (if populated from a template)
            // * First sentence of first paragraph in the document (minus period)
            // * Default file name if no title and empty content (Doc1.docx)
            Office.DocumentProperties docProps = doc.BuiltInDocumentProperties;

            // Try title...
            if(docProps != null)
            {
                string title = docProps["title"]?.Value;
                if (!string.IsNullOrEmpty(title))
                    fileName = title;
            }

            // If title didn't work, try first sentence...
            if(string.IsNullOrEmpty(fileName))
            {
                fileName = doc.Paragraphs?.First?.Range?.Sentences?.First?.Text?.Trim();
            }

            // If the document is empty, use the default document name.
            // This will be Document1, etc, but Word itself will use
            // the shorter form Doc1.docx in its save process. Note that
            // we only get a name, not an extension. But, the save call
            // will append the default extension. So, at the end we need 
            // to ensure that we send along the actual file name with
            // extension and path that as created.
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = doc.FullName;
            }

            // Possibly have invalid characters so fix that...
            fileName = Helpers.CleanFilename(fileName);

            string userTempPath = Path.GetTempPath();
            string filePath = userTempPath + fileName;

            string parentId = ThisAddIn.ContentManager.BrowseForLocation();

            if(!string.IsNullOrEmpty(parentId))
            {
                List<KeyValuePair<string, string>> fileInfoList = new List<KeyValuePair<string, string>>();

                doc.SaveAs2(filePath);

                fileName = doc.Name;        // Making sure we have the for reals name
                filePath = doc.FullName;    // Making sure we have the for reals path

                doc.Close();

                fileInfoList.Add(new KeyValuePair<string, string>(fileName, filePath));

                List<(string, string)> savedItems = await ThisAddIn.ContentManager.SaveDocuments(fileInfoList, parentId);

                foreach((string name, string webDavUrl) in savedItems)
                {
                    // Attempt to send a message to Teams:
                    string html = string.Format("A <a href='{0}'>new document: {1}</a> has been added!", webDavUrl, name);
                    ThisAddIn.ContentManager.SendMessageToTeams(html);
                    ThisAddIn.Instance.Application.Documents.Open(webDavUrl);
                }
            }
        }

        private static void OpenDocument()
        {
            List<string> types = new List<string>
            {
                ItemMimeTypes.WORD_DOCUMENT,
                ItemMimeTypes.WORD_TEMPLATE
            };

            string itemUrl = ThisAddIn.ContentManager.BrowseForItem(types);
            if (!string.IsNullOrEmpty(itemUrl))
            {
                _ = ThisAddIn.Instance.Application.Documents.Open(itemUrl);
            }
        }

        #endregion
    }
}
