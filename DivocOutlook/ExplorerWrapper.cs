using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;
using DivocCommon;
using System.Windows.Forms;

namespace DivocOutlook
{
    class ExplorerWrapper : OLViewWrapperBase, IWin32Window
    {
        public Outlook.Explorer Explorer { get; private set; }

        AttachmentsHandler _attachmentsHandler = null;
        Outlook.MailItem _email = null;

        public ExplorerWrapper(Outlook.Explorer explorer)
        {
            LogManager.LogMethod(string.Format("Explorer Id: {0}", Id));

            Explorer = explorer;

            AddEventHandlers();
        }

        void AddEventHandlers()
        {
            ((Outlook.ExplorerEvents_10_Event)Explorer).Close += ExplorerWrapper_Close;
            ((Outlook.ExplorerEvents_10_Event)Explorer).Activate += ExplorerWrapper_Activate;
            Explorer.Deactivate += ExplorerWrapper_Deactivate;
            Explorer.SelectionChange += Explorer_SelectionChange;
            ((Outlook.ExplorerEvents_10_Event)Explorer).InlineResponse += ExplorerWrapper_InlineResponse;
            ((Outlook.ExplorerEvents_10_Event)Explorer).InlineResponseClose += ExplorerWrapper_InlineResponseClose;
        }

        public IntPtr Handle
        {
            get
            {
                return GetHandleForExplorer(Explorer);
            }
        }

        public static IntPtr GetHandleForExplorer(Outlook.Explorer explorer)
        {
            IntPtr wnd = IntPtr.Zero;

            try
            {
                ((IOleWindow)explorer).GetWindow(out wnd);
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
            }

            return wnd;
        }

        private void ExplorerWrapper_InlineResponse(object Item)
        {
            // Item should be the same object as Explorer.ActiveInlineResponse.
            // Use that in the send handler to manipulate the email we create here.
            if(Item is Outlook.MailItem)
            {
                _email = Item as Outlook.MailItem;

                _attachmentsHandler = new AttachmentsHandler(this, ref _email);

                Outlook.MailItem email = Item as Outlook.MailItem;

                ((Outlook.ItemEvents_10_Event)_email).Send += ExplorerWrapper_Send;
            }
        }

        private void ExplorerWrapper_InlineResponseClose()
        {
            _email = null;
            _attachmentsHandler = null;

            ((Outlook.ItemEvents_10_Event)Explorer.ActiveInlineResponse).Send -= ExplorerWrapper_Send;
        }

        private void ExplorerWrapper_Send(ref bool Cancel)
        {
            if (_attachmentsHandler != null)
            {
                Cancel = _attachmentsHandler.UploadAndLink();
            }
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
            LogManager.LogMethod(string.Format("Explorer Id: {0}", Id));

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
            LogManager.LogMethod(string.Format("Explorer Id: {0}", Id));
        }

        private void ExplorerWrapper_Deactivate()
        {
            // Deactivate is called a lot, only log if needed
            LogManager.LogMethod(string.Format("Explorer Id: {0}", Id));
        }

        private void Explorer_SelectionChange()
        {
            LogManager.LogMethod(string.Format("Explorer Id: {0}", Id));

            // Tell ribbon to update
            ThisAddIn.InvalidateRibbon();
        }
    }
}
