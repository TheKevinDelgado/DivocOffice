using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;
using DivocCommon;
using System.Windows.Forms;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Runtime.InteropServices;

namespace DivocOutlook
{
    class InspectorWrapper : OLViewWrapperBase, IWin32Window
    {
        public Outlook.Inspector Inspector { get; private set; }

        private readonly Outlook.MailItem _email;
        readonly AttachmentsHandler _attachmentsHandler = null;

        public InspectorWrapper(Outlook.Inspector inspector)
        {
            LogManager.LogMethod(string.Format("Inspector Id: {0}", Id));

            Inspector = inspector;

            // Only worry about setting up an attachment handler for emails, not appointments etc
            if(Inspector.CurrentItem is Outlook.MailItem)
            {
                _email = Inspector.CurrentItem;
                _attachmentsHandler = new AttachmentsHandler(this, ref _email);
            }

            AddEventHandlers();
        }

        void AddEventHandlers()
        {
            ((Outlook.InspectorEvents_10_Event)Inspector).Close += InspectorWrapper_Close;
            ((Outlook.InspectorEvents_10_Event)Inspector).Activate += InspectorWrapper_Activate;
            Inspector.Deactivate += InspectorWrapper_Deactivate;
            ((Outlook.ItemEvents_10_Event)Inspector.CurrentItem).Send += InspectorWrapper_Send;
            ((Outlook.ItemEvents_10_Event)Inspector.CurrentItem).Close += InspectorWrapper_MailItem_Close;
        }

        public IntPtr Handle
        {
            get 
            {
                return GetHandleForInspector(Inspector);
            }
        }

        public static IntPtr GetHandleForInspector(Outlook.Inspector inspector)
        {
            IntPtr wnd = IntPtr.Zero;

            try
            {
                ((IOleWindow)inspector).GetWindow(out wnd);
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
            }

            return wnd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <notes>
        /// The inspector is used for email, appointments, tasks, etc, which is why the
        /// CurrentItem property is dynamic. For this first go, we're just going to implement
        /// logic for when an email is sent with attachments. We can circle back on the
        /// other item types later, but additional ribbon xml work will be needed.
        /// 
        /// We will need to offload work for the attachments outside of this class because 
        /// we will also want to handle inline replies from the explorer window.
        /// 
        /// For this iteration we will also query the user to ask if they want to upload
        /// their attachments and send links to the saved versions instead. This could
        /// later be a configurable option to either act as normal, ask, or always force
        /// upload.
        /// </notes>
        /// <param name="Cancel"></param>
        private void InspectorWrapper_Send(ref bool Cancel)
        {
            LogManager.LogMethod(string.Format("Inspector Id: {0}", Id));

            if(_attachmentsHandler != null)
            {
                Cancel = _attachmentsHandler.UploadAndLink();
            }
        }

        void RemoveEventHandlers()
        {
            ((Outlook.InspectorEvents_10_Event)Inspector).Close -= InspectorWrapper_Close;
            ((Outlook.InspectorEvents_10_Event)Inspector).Activate -= InspectorWrapper_Activate;
            Inspector.Deactivate -= InspectorWrapper_Deactivate;
        }

        private void InspectorWrapper_Close()
        {
            LogManager.LogMethod(string.Format("Inspector Id: {0}", Id));

            RemoveEventHandlers();

            Inspector = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // Trigger the wrapper closed event
            OnClosed();
        }

        private void InspectorWrapper_MailItem_Close(ref bool Cancel)
        {
            LogManager.LogMethod(string.Format("Inspector Id: {0}", Id));
        }

        private void InspectorWrapper_Activate()
        {
            // Activate is called a lot, only log if needed
            LogManager.LogMethod(string.Format("Inspector Id: {0}", Id));
            ThisAddIn.InvalidateRibbon();
        }

        private void InspectorWrapper_Deactivate()
        {
            // Deactivate is called a lot, only log if needed
            LogManager.LogMethod(string.Format("Inspector Id: {0}", Id));
        }
    }
}
