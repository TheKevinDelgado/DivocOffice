using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace DivocOutlook
{
    class InspectorWrapper : OLViewWrapperBase
    {
        public Outlook.Inspector Inspector { get; private set; }

        public InspectorWrapper(Outlook.Inspector inspector)
        {
            ThisAddIn.LogMethod(string.Format("Inspector Id: {0}", Id));

            Inspector = inspector;

            AddEventHandlers();
        }

        void AddEventHandlers()
        {
            ((Outlook.InspectorEvents_10_Event)Inspector).Close += InspectorWrapper_Close;
            ((Outlook.InspectorEvents_10_Event)Inspector).Activate += InspectorWrapper_Activate;
            Inspector.Deactivate += InspectorWrapper_Deactivate;
        }

        void RemoveEventHandlers()
        {
            ((Outlook.InspectorEvents_10_Event)Inspector).Close -= InspectorWrapper_Close;
            ((Outlook.InspectorEvents_10_Event)Inspector).Activate -= InspectorWrapper_Activate;
            Inspector.Deactivate -= InspectorWrapper_Deactivate;
        }

        private void InspectorWrapper_Close()
        {
            ThisAddIn.LogMethod(string.Format("Inspector Id: {0}", Id));

            RemoveEventHandlers();

            Inspector = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // Trigger the wrapper closed event
            OnClosed();
        }

        private void InspectorWrapper_Activate()
        {
            // Activate is called a lot, only log if needed
            ThisAddIn.LogMethod(string.Format("Inspector Id: {0}", Id));
            ThisAddIn.InvalidateRibbon();
        }

        private void InspectorWrapper_Deactivate()
        {
            // Deactivate is called a lot, only log if needed
            ThisAddIn.LogMethod(string.Format("Inspector Id: {0}", Id));
        }
    }
}
