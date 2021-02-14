﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Office = Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using DivocCommon;

// TODO:  Follow these steps to enable the Ribbon (XML) item:

// 1: Copy the following code block into the ThisAddin, ThisWorkbook, or ThisDocument class.

//  protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
//  {
//      return new PowerPointRibbonManager();
//  }

// 2. Create callback methods in the "Ribbon Callbacks" region of this class to handle user
//    actions, such as clicking a button. Note: if you have exported this Ribbon from the Ribbon designer,
//    move your code from the event handlers to the callback methods and modify the code to work with the
//    Ribbon extensibility (RibbonX) programming model.

// 3. Assign attributes to the control tags in the Ribbon XML file to identify the appropriate callback methods in your code.  

// For more information, see the Ribbon XML documentation in the Visual Studio Tools for Office Help.


namespace DivocPowerPoint
{
    [ComVisible(true)]
    public class PowerPointRibbonManager : OfficeRibbonManagerBase, Office.IRibbonExtensibility
    {
        public PowerPointRibbonManager()
        {
        }

        #region IRibbonExtensibility Members

        public override string GetCustomUI(string ribbonID)
        {
            LogManager.LogMethod(string.Format("Ribbon Id: {0}", ribbonID));

            string ribbonUI = null;

            switch (ribbonID)
            {
                case "Microsoft.PowerPoint.Presentation":
                    ribbonUI = GetResourceText("DivocPowerPoint.RibbonPowerPoint.xml");
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
                    case RibbonIDs.SAVE_PRESENTATION:
                        if (context != null)
                        {
                            enabled = true;
                        }

                        break;

                    case RibbonIDs.OPEN_PRESENTATION:
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
                    case RibbonIDs.SAVE_PRESENTATION:
                        PowerPoint.Presentation pres = control.Context.Presentation as PowerPoint.Presentation;
                        SavePresentation(pres);
                        break;

                    case RibbonIDs.OPEN_PRESENTATION:
                        OpenPresentation();
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

        private async void SavePresentation(PowerPoint.Presentation pres)
        {
            string fileName = string.Empty;

            fileName = pres.Name;

            // Possibly have invalid characters so fix that...
            fileName = Helpers.CleanFilename(fileName);

            string userTempPath = Path.GetTempPath();
            string filePath = userTempPath + fileName;

            string parentId = ThisAddIn.ContentManager.BrowseForLocation();

            if (!string.IsNullOrEmpty(parentId))
            {
                List<KeyValuePair<string, string>> fileInfoList = new List<KeyValuePair<string, string>>();

                pres.SaveAs(filePath);

                fileName = pres.Name;        // Making sure we have the for reals name
                filePath = pres.FullName;    // Making sure we have the for reals path

                pres.Close();

                fileInfoList.Add(new KeyValuePair<string, string>(fileName, filePath));

                List<(string, string)> savedItems = await ThisAddIn.ContentManager.SaveDocuments(fileInfoList, parentId);

                foreach ((string name, string webDavUrl) item in savedItems)
                {
                    ThisAddIn.Instance.Application.Presentations.Open(item.webDavUrl);
                }
            }
        }

        private void OpenPresentation()
        {
            List<string> types = new List<string>();
            types.Add(ItemMimeTypes.PPT_PRESENTATION);
            types.Add(ItemMimeTypes.PPT_TEMPLATE);

            string itemUrl = ThisAddIn.ContentManager.BrowseForItem(types);
            if (!string.IsNullOrEmpty(itemUrl))
            {
                PowerPoint.Presentation openpres = ThisAddIn.Instance.Application.Presentations.Open(itemUrl);
            }
        }

        #endregion
    }
}
