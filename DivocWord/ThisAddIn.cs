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
        public static ContentManager ContentManager { get; private set; }
        public static ThisAddIn Instance { get; private set; }

        private Word.ApplicationEvents4_Event _AppEvents = null;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            LogManager.LogMethod();

            // Set up Application event handlers...
            _AppEvents = (Word.ApplicationEvents4_Event)this.Application;
            _AppEvents.DocumentOpen += Events_DocumentOpen;
            _AppEvents.NewDocument += Events_NewDocument;

            ContentManager = new ContentManager();
            Instance = this;
        }

        public static void InvalidateRibbon()
        {
            if (ribbonManager != null && ThisAddIn.ribbonManager.Ribbon != null)
                ribbonManager.Ribbon.Invalidate();
        }

        private void Events_NewDocument(Word.Document Doc)
        {
            Word.DocumentEvents2_Event docEvents = (Word.DocumentEvents2_Event)Doc;

            docEvents.Close += Events_Close;

            InvalidateRibbon();
        }

        private void Events_Close()
        {
            InvalidateRibbon();
        }

        private void Events_DocumentOpen(Word.Document Doc)
        {
            Word.DocumentEvents2_Event docEvents = (Word.DocumentEvents2_Event)Doc;

            docEvents.Close += Events_Close;

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
