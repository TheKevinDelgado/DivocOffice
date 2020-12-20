using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace DivocOutlook
{
    class ExplorerWrapper : OLViewWrapperBase
    {
        public Outlook.Explorer Explorer { get; private set; }

        public ExplorerWrapper(Outlook.Explorer explorer)
        {
            //ThisAddIn.Log.Information("New Explorer created: {Id}", Id);
            ThisAddIn.LogMethod(string.Format("Explorer Id: {0}", Id));

            Explorer = explorer;

            AddEventHandlers();
        }

        void AddEventHandlers()
        {
            ((Outlook.ExplorerEvents_10_Event)Explorer).Close += ExplorerWrapper_Close;
            ((Outlook.ExplorerEvents_10_Event)Explorer).Activate += ExplorerWrapper_Activate;
            Explorer.Deactivate += ExplorerWrapper_Deactivate;
            Explorer.SelectionChange += Explorer_SelectionChange;
        }

        void RemoveEventHandlers()
        {
            ((Outlook.ExplorerEvents_10_Event)Explorer).Close -= ExplorerWrapper_Close;
            ((Outlook.ExplorerEvents_10_Event)Explorer).Activate -= ExplorerWrapper_Activate;
            Explorer.Deactivate -= ExplorerWrapper_Deactivate;
            Explorer.SelectionChange -= Explorer_SelectionChange;
        }

        private void ExplorerWrapper_Close()
        {
            ThisAddIn.LogMethod(string.Format("Explorer Id: {0}", Id));

            RemoveEventHandlers();

            Explorer = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // Trigger the wrapper closed event
            OnClosed();
        }

        private void ExplorerWrapper_Activate()
        {
            // Activate is called a lot, only log if needed
            ThisAddIn.LogMethod(string.Format("Explorer Id: {0}", Id));
        }

        private void ExplorerWrapper_Deactivate()
        {
            // Deactivate is called a lot, only log if needed
            ThisAddIn.LogMethod(string.Format("Explorer Id: {0}", Id));
        }

        private void Explorer_SelectionChange()
        {
            ThisAddIn.LogMethod(string.Format("Explorer Id: {0}", Id));

            // Tell ribbon to update
            ThisAddIn.InvalidateRibbon();
        }
    }
}
