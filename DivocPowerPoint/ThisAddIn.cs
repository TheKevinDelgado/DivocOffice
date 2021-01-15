using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Office = Microsoft.Office.Core;
using DivocCommon;
using System.Threading;
using System.Threading.Tasks;

namespace DivocPowerPoint
{
    public partial class ThisAddIn
    {
        static PowerPointRibbonManager ribbonManager = null;
        public static ContentManager ContentManager { get; private set; }
        public static ThisAddIn Instance { get; private set; }

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            LogManager.LogMethod();

            // Set up Application event handlers...
            PowerPoint.EApplication_Event events = (PowerPoint.EApplication_Event)this.Application;
            events.NewPresentation += Events_NewPresentation;
            events.PresentationOpen += Events_PresentationOpen;
            events.PresentationClose += Events_PresentationClose;

            ContentManager = new ContentManager();
            Instance = this;
        }

        public static void InvalidateRibbon()
        {
            if (ribbonManager != null && ThisAddIn.ribbonManager.Ribbon != null)
                ribbonManager.Ribbon.Invalidate();
        }

        private void Events_PresentationOpen(PowerPoint.Presentation Pres)
        {
            InvalidateRibbon();
        }

        private void Events_NewPresentation(PowerPoint.Presentation Pres)
        {
            InvalidateRibbon();
        }

        private void Events_PresentationClose(PowerPoint.Presentation Pres)
        {
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
                return ribbonManager = new PowerPointRibbonManager();
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
