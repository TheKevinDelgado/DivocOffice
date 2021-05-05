using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DivocCommon;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace DivocExcel
{
    class WorkbookWrapper : ViewWrapperBase, IWin32Window
    {
        public Excel.Workbook Workbook { get; private set; }
        Dictionary<Guid, WorksheetWrapper> _WrappedWorksheets = new Dictionary<Guid, WorksheetWrapper>();

        public WorkbookWrapper(Excel.Workbook workbook)
        {
            Workbook = workbook;

            Workbook.BeforeClose += Workbook_BeforeClose;
            Workbook.NewSheet += Workbook_NewSheet;

            // Wrap any existing sheets...
            foreach(Excel.Worksheet worksheet in Workbook.Worksheets)
            {
                WrapSheet(worksheet);
            }
        }

        private void Workbook_NewSheet(object Sh)
        {
            WrapSheet((Excel.Worksheet)Sh);
        }

        private void Workbook_BeforeClose(ref bool Cancel)
        {
            Workbook.BeforeClose -= new Excel.WorkbookEvents_BeforeCloseEventHandler(Workbook_BeforeClose);

            Workbook.NewSheet -= new Excel.WorkbookEvents_NewSheetEventHandler(Workbook_NewSheet);

            foreach (WorksheetWrapper wrapper in _WrappedWorksheets.Values)
            {

                wrapper.DetachWrapper();

            }

            _WrappedWorksheets.Clear();

            Workbook = null;

            OnClosed();
        }

        void WrapSheet(Excel.Worksheet worksheet)
        {

            WorksheetWrapper wrapper = new WorksheetWrapper(worksheet);

            _WrappedWorksheets[wrapper.Id] = wrapper;
        }

        public IntPtr Handle
        {
            get
            {
                return GetHandleForWorkbook(Workbook);
            }
        }

        public static IntPtr GetHandleForWorkbook(Excel.Workbook workbook)
        {
            IntPtr wnd = IntPtr.Zero;

            try
            {
                ((IOleWindow)workbook).GetWindow(out wnd);
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
            }

            return wnd;
        }
    }
}
