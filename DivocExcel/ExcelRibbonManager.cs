﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Office = Microsoft.Office.Core;
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

            string ribbonUI = null;

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
            bool enabled = true;

            try
            {
                LogManager.LogMethod(string.Format("Ribbon Control Id: {0}", control.Id));

                dynamic context = control.Context;

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
