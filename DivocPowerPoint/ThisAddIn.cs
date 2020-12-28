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
        AuthenticationManager auth = new AuthenticationManager();

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            LogManager.LogMethod();

            // Set up Application event handlers...
            PowerPoint.EApplication_Event events = (PowerPoint.EApplication_Event)this.Application;
            events.NewPresentation += Events_NewPresentation;
            events.PresentationOpen += Events_PresentationOpen;
        }

        private async void Events_PresentationOpen(PowerPoint.Presentation Pres)
        {
            await DoAuthenticate();
        }

        private async void Events_NewPresentation(PowerPoint.Presentation Pres)
        {
            await DoAuthenticate();
        }

        private async Task<bool> DoAuthenticate()
        {
            return await auth.Authenticate(new IntPtr(this.Application.ActiveWindow.HWND));
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
