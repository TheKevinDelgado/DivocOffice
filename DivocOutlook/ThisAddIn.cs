using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using DivocCommon;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DivocOutlook
{
    public partial class ThisAddIn
    {
        Outlook.Explorers _explorers;
        Outlook.Inspectors _inspectors;
        Dictionary<Guid, OLViewWrapperBase> _WrappedViews;

        static OutlookRibbonManager ribbonManager = null;
        public static ContentManager ContentManager { get; private set; }

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            LogManager.LogMethod();

            _WrappedViews = new Dictionary<Guid, OLViewWrapperBase>();

            _explorers = Application.Explorers;
            _inspectors = Application.Inspectors;

            // At least one explorer should exist when addins are loaded, keep track of it and any others
            foreach(Outlook.Explorer expl in _explorers)
            {
                WrapExplorer(expl);
            }
            // rare condition that an inspector can exist when addins are loaded, track those too
            foreach (Outlook.Inspector insp in _inspectors)
            {
                WrapInspector(insp);
            }

            _explorers.NewExplorer += _explorers_NewExplorer;
            _inspectors.NewInspector += _inspectors_NewInspector;

            ContentManager = new ContentManager();
        }

        private void _explorers_NewExplorer(Outlook.Explorer Explorer)
        {
            WrapExplorer(Explorer);
        }

        private void _inspectors_NewInspector(Outlook.Inspector Inspector)
        {
            WrapInspector(Inspector);
        }

        void WrapExplorer(Outlook.Explorer explorer)
        {
            LogManager.LogMethod();

            ExplorerWrapper wrappedExplorer = new ExplorerWrapper(explorer);
            wrappedExplorer.Closed += new WindowWrapperClosedDelegate(WrappedView_Closed);
            _WrappedViews[wrappedExplorer.Id] = wrappedExplorer;
        }
        void WrapInspector(Outlook.Inspector inspector)
        {
            LogManager.LogMethod();

            InspectorWrapper wrappedInspector = new InspectorWrapper(inspector);
            wrappedInspector.Closed += new WindowWrapperClosedDelegate(WrappedView_Closed);
            _WrappedViews[wrappedInspector.Id] = wrappedInspector;
        }

        void WrappedView_Closed(Guid id)
        {
            _WrappedViews.Remove(id);
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            LogManager.LogMethod();

            // Note: Outlook no longer raises this event. If you have code that 
            //    must run when Outlook shuts down, see https://go.microsoft.com/fwlink/?LinkId=506785
        }

        protected override Office.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            LogManager.LogMethod();

            if (ribbonManager != null)
                return ribbonManager;
            else
                return ribbonManager = new OutlookRibbonManager();
        }

        public static void InvalidateRibbon()
        {
            if (ribbonManager != null && ThisAddIn.ribbonManager.Ribbon != null)
                ribbonManager.Ribbon.Invalidate();
        }

        public static void InvalidateRibbonControl(string id)
        {
            if (ribbonManager != null && ThisAddIn.ribbonManager.Ribbon != null)
                ribbonManager.Ribbon.InvalidateControl(id);
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
