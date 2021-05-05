using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DivocCommon;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using ExcelTools = Microsoft.Office.Tools.Excel;

namespace DivocExcel
{
    class WorksheetWrapper : ViewWrapperBase, IWin32Window
    {
        public Excel.Worksheet Worksheet { get; private set; }

        ExcelTools.Worksheet ToolsWorksheet { get; set; }

        public WorksheetWrapper(Excel.Worksheet worksheet)
        {
            Worksheet = worksheet;

            ((Excel.DocEvents_Event)Worksheet).Activate += WorksheetWrapper_Activate;
            ((Excel.DocEvents_Event)Worksheet).SelectionChange += WorksheetWrapper_SelectionChange;
            ((Excel.DocEvents_Event)Worksheet).Change += WorksheetWrapper_Change;
            ((Excel.DocEvents_Event)Worksheet).Calculate += WorksheetWrapper_Calculate;

            ToolsWorksheet = Globals.Factory.GetVstoObject(Worksheet);

            ToolsWorksheet.Controls.AddControl(new TextBox() { Text = "Data1", Dock = DockStyle.Fill }, Worksheet.Range["A5"], "CustomCell1");
            Worksheet.Range["A5"].Formula = "=SUM(A1,B1)";
            ToolsWorksheet.Controls.AddControl(new TextBox() { Text = "Data2", BorderStyle = BorderStyle.None, Dock = DockStyle.Left, Width = 25, MaximumSize = new System.Drawing.Size(25, 25) }, Worksheet.Range["A6"], "CustomCell2");
            Worksheet.Range["A6"].Formula = "=SUM(A1,B1)";
        }

        private void WorksheetWrapper_Calculate()
        {
            System.Diagnostics.Debug.WriteLine("---------------------------Calculate");
        }

        private void WorksheetWrapper_Change(Excel.Range Target)
        {
            System.Diagnostics.Debug.WriteLine("---------------------------Change");
        }

        private void WorksheetWrapper_SelectionChange(Excel.Range Target)
        {
            System.Diagnostics.Debug.WriteLine("---------------------------Selection Change, Formula: " + ((object)Target.Formula).ToString());
        }

        private void WorksheetWrapper_Activate()
        {
            System.Diagnostics.Debug.WriteLine("---------------------------Worksheet Activated");
        }

        public IntPtr Handle
        {
            get
            {
                return GetHandleForWorksheet(Worksheet);
            }
        }

        public static IntPtr GetHandleForWorksheet(Excel.Worksheet worksheet)
        {
            IntPtr wnd = IntPtr.Zero;

            try
            {
                ((IOleWindow)worksheet).GetWindow(out wnd);
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
            }

            return wnd;
        }
        public void DetachWrapper()
        {
            ((Excel.DocEvents_Event)Worksheet).Activate -= WorksheetWrapper_Activate;
            ((Excel.DocEvents_Event)Worksheet).SelectionChange -= WorksheetWrapper_SelectionChange;

            Worksheet = null;
        }
    }
}
