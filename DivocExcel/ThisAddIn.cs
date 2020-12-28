using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;
using DivocCommon;
using System.Threading.Tasks;

namespace DivocExcel
{
    public partial class ThisAddIn
    {
        static ExcelRibbonManager ribbonManager = null;
        AuthenticationManager auth = new AuthenticationManager();

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            LogManager.LogMethod();

            // Set up Application event handlers...
            Excel.AppEvents_Event events = (Excel.AppEvents_Event)this.Application;
            events.NewWorkbook += Events_NewWorkbook;
            events.WorkbookOpen += Events_WorkbookOpen;
        }

        private async void Events_WorkbookOpen(Excel.Workbook Wb)
        {
            await DoAuthenticate();
        }

        private async void Events_NewWorkbook(Excel.Workbook Wb)
        {
            await DoAuthenticate();
        }

        private async Task<bool> DoAuthenticate()
        {
            return await auth.Authenticate(new IntPtr(this.Application.ActiveWindow.Hwnd));
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            LogManager.LogMethod();
        }

        protected override Office.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            LogManager.LogMethod();

            if (ribbonManager != null)
                return ribbonManager;
            else
                return ribbonManager = new ExcelRibbonManager();
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
