using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Office = Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;
using DivocCommon;

// TODO:  Follow these steps to enable the Ribbon (XML) item:

// 1: Copy the following code block into the ThisAddin, ThisWorkbook, or ThisDocument class.

//  protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
//  {
//      return new ExcelRibbonManager();
//  }

// 2. Create callback methods in the "Ribbon Callbacks" region of this class to handle user
//    actions, such as clicking a button. Note: if you have exported this Ribbon from the Ribbon designer,
//    move your code from the event handlers to the callback methods and modify the code to work with the
//    Ribbon extensibility (RibbonX) programming model.

// 3. Assign attributes to the control tags in the Ribbon XML file to identify the appropriate callback methods in your code.  

// For more information, see the Ribbon XML documentation in the Visual Studio Tools for Office Help.


namespace DivocExcel
{
    [ComVisible(true)]
    public class ExcelRibbonManager : OfficeRibbonManagerBase, Office.IRibbonExtensibility
    {
        public ExcelRibbonManager()
        {
        }

        #region IRibbonExtensibility Members

        public override string GetCustomUI(string ribbonID)
        {
            LogManager.LogMethod(string.Format("Ribbon Id: {0}", ribbonID));

            string ribbonUI;

            switch (ribbonID)
            {
                case "Microsoft.Excel.Workbook":
                    ribbonUI = GetResourceText("DivocExcel.RibbonExcel.xml");
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
                    case RibbonIDs.SAVE_WORKBOOK:
                        if (context != null)
                        {
                            enabled = true;
                        }

                        break;

                    case RibbonIDs.OPEN_WORKBOOK:
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
                    case RibbonIDs.SAVE_WORKBOOK:
                        Excel.Application app = control.Context.Application as Excel.Application;
                        Excel.Workbook book = app.ActiveWorkbook;
                        SaveWorkbook(book);
                        break;

                    case RibbonIDs.OPEN_WORKBOOK:
                        OpenWorkbook();
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

        /// <summary>
        /// Present user with location selection UI and save the workbook
        /// </summary>
        /// <notes>
        /// When opening the webdav url after saving, it opens it in read-only mode.
        /// Not sure if this is an excel issue or something on the sharepoint side, 
        /// but so far only excel has this issue. Investigate.
        /// </notes>
        /// <param name="book">The workbook to save</param>
        private static async void SaveWorkbook(Excel.Workbook book)
        {
            string fileName = book.Name;

            // Possibly have invalid characters so fix that...
            fileName = Helpers.CleanFilename(fileName);

            string userTempPath = Path.GetTempPath();
            string filePath = userTempPath + fileName;

            string parentId = ThisAddIn.ContentManager.BrowseForLocation();

            if (!string.IsNullOrEmpty(parentId))
            {
                List<KeyValuePair<string, string>> fileInfoList = new List<KeyValuePair<string, string>>();

                book.SaveAs(filePath);

                fileName = book.Name;        // Making sure we have the for reals name
                filePath = book.FullName;    // Making sure we have the for reals path

                book.Close();

                fileInfoList.Add(new KeyValuePair<string, string>(fileName, filePath));

                List<(string, string)> savedItems = await ThisAddIn.ContentManager.SaveDocuments(fileInfoList, parentId);

                foreach ((_, string webDavUrl) in savedItems)
                {
                    ThisAddIn.Instance.Application.Workbooks.Open(webDavUrl);
                }
            }
        }

        private static void OpenWorkbook()
        {
            List<string> types = new List<string>
            {
                ItemMimeTypes.EXCEL_SPREADSHEET,
                ItemMimeTypes.EXCEL_TEMPLATE
            };

            string itemUrl = ThisAddIn.ContentManager.BrowseForItem(types);
            if (!string.IsNullOrEmpty(itemUrl))
            {
                _ = ThisAddIn.Instance.Application.Workbooks.Open(itemUrl);
            }
        }

        #endregion
    }
}
