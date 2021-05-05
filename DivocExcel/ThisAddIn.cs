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
        Dictionary<Guid, ViewWrapperBase> _WrappedWorkbooks;

        static ExcelRibbonManager ribbonManager = null;
        public static ContentManager ContentManager { get; private set; }
        public static ThisAddIn Instance { get; private set; }

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            LogManager.LogMethod();

            // Wrap any existing workbooks...
            _WrappedWorkbooks = new Dictionary<Guid, ViewWrapperBase>();

            foreach (Excel.Workbook workbook in Application.Workbooks)
            {
                WrapWorkbook(workbook);
            }

            // Set up Application event handlers...
            Excel.AppEvents_Event events = (Excel.AppEvents_Event)this.Application;
            events.NewWorkbook += Events_NewWorkbook;
            events.WorkbookOpen += Events_WorkbookOpen;
            events.WorkbookBeforeClose += Events_WorkbookBeforeClose;

            ContentManager = new ContentManager();
            Instance = this;
        }

        void WrapWorkbook(Excel.Workbook workbook)
        {

            WorkbookWrapper wrapper = new WorkbookWrapper(workbook);

            wrapper.Closed += new ViewWrapperClosedDelegate(WrappedWorkbook_Closed);

            _WrappedWorkbooks[wrapper.Id] = wrapper;

        }

        void WrappedWorkbook_Closed(Guid id)
        {
            _WrappedWorkbooks.Remove(id);
        }

        public static void InvalidateRibbon()
        {
            if (ribbonManager != null && ThisAddIn.ribbonManager.Ribbon != null)
                ribbonManager.Ribbon.Invalidate();
        }

        private void Events_WorkbookOpen(Excel.Workbook Wb)
        {
            InvalidateRibbon();
            WrapWorkbook(Wb);
        }

        private void Events_NewWorkbook(Excel.Workbook Wb)
        {
            InvalidateRibbon();
            WrapWorkbook(Wb);
        }

        private void Events_WorkbookBeforeClose(Excel.Workbook Wb, ref bool Cancel)
        {
            Cancel = false;
            InvalidateRibbon();
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
