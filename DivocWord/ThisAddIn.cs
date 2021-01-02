using DivocCommon;
using System;
using System.Threading.Tasks;
using Office = Microsoft.Office.Core;
using Word = Microsoft.Office.Interop.Word;

namespace DivocWord
{
    public partial class ThisAddIn
    {
        static WordRibbonManager ribbonManager = null;
        AuthenticationManager auth = new AuthenticationManager();
        public static ContentManager ContentManager { get; private set; }

        private async void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            LogManager.LogMethod();

            // Set up Application event handlers...
            Word.ApplicationEvents4_Event events = (Word.ApplicationEvents4_Event)this.Application;
            events.DocumentOpen += Events_DocumentOpen;
            events.NewDocument += Events_NewDocument;

            await auth.Authenticate(IntPtr.Zero);
            ContentManager = new ContentManager();
        }

        private void Events_NewDocument(Word.Document Doc)
        {
        }

        private void Events_DocumentOpen(Word.Document Doc)
        {
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
                return ribbonManager = new WordRibbonManager();
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
