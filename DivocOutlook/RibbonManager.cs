using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Office = Microsoft.Office.Core;
using Outlook = Microsoft.Office.Interop.Outlook;
using DivocCommon;
using System.Windows.Forms;
using System.Drawing;

// TODO:  Follow these steps to enable the Ribbon (XML) item:

// 1: Copy the following code block into the ThisAddin, ThisWorkbook, or ThisDocument class.

//  protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
//  {
//      return new ExplorerRibbon();
//  }

// 2. Create callback methods in the "Ribbon Callbacks" region of this class to handle user
//    actions, such as clicking a button. Note: if you have exported this Ribbon from the Ribbon designer,
//    move your code from the event handlers to the callback methods and modify the code to work with the
//    Ribbon extensibility (RibbonX) programming model.

// 3. Assign attributes to the control tags in the Ribbon XML file to identify the appropriate callback methods in your code.  

// For more information, see the Ribbon XML documentation in the Visual Studio Tools for Office Help.


namespace DivocOutlook
{
    [ComVisible(true)]
    public class RibbonManager : Office.IRibbonExtensibility
    {
        public Office.IRibbonUI Ribbon { get; private set; }

        public RibbonManager()
        {
        }

        #region IRibbonExtensibility Members

        public string GetCustomUI(string ribbonID)
        {
            ThisAddIn.LogMethod(string.Format("Ribbon Id: {0}", ribbonID));

            string ribbonUI = null;

            switch(ribbonID)
            {
                case "Microsoft.Outlook.Explorer":
                    ribbonUI = GetResourceText("DivocOutlook.RibbonExplorer.xml");
                    break;

                case "Microsoft.Outlook.Mail.Read":
                    ribbonUI = GetResourceText("DivocOutlook.RibbonInspectorRead.xml");
                    break;

                case "Microsoft.Outlook.Mail.Compose":
                    ribbonUI = GetResourceText("DivocOutlook.RibbonInspectorCompose.xml");
                    break;
            }

            return ribbonUI;
        }

        #endregion

        #region Ribbon Callbacks
        //Create callback methods here. For more information about adding callback methods, visit https://go.microsoft.com/fwlink/?LinkID=271226

        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            Ribbon = ribbonUI;
        }

        public string OnGetLabel(Office.IRibbonControl control)
        {
            string label =  string.Empty;

            try
            {
                string id = control.Id;

                ThisAddIn.LogMethod(string.Format("Ribbon Control Id: {0}", control.Id));

                switch (id)
                {
                    case RibbonIDs.DIVOC_GROUP:
                    case RibbonIDs.DIVOC_GROUP_INLINE:
                        label = ResourceBroker.GetString(ResourceBroker.ResourceID.PRODUCT_NAME);
                        break;

                    case RibbonIDs.INSERT_ATTACHMENTS:
                        label = ResourceBroker.GetString(ResourceBroker.ResourceID.INSERT_ATTACHMENTS_LABEL);
                        break;

                    case RibbonIDs.SAVE_MAIL:
                        label = ResourceBroker.GetString(ResourceBroker.ResourceID.SAVE_MAIL_LABEL);
                        break;

                    case RibbonIDs.SAVE_ATTACHMENTS:
                        label = ResourceBroker.GetString(ResourceBroker.ResourceID.SAVE_ATTACHMENTS_LABEL);
                        break;
                }
            }
            catch(Exception ex)
            {
                ThisAddIn.Log.Error("{@ex}", ex);
            }

            return label;
        }

        public Bitmap OnGetImage(Office.IRibbonControl control)
        {
            Bitmap img = null;

            try
            {
                string id = control.Id;

                ThisAddIn.LogMethod(string.Format("Ribbon Control Id: {0}", control.Id));

                switch (id)
                {
                    case RibbonIDs.SAVE_MAIL:
                        img = ResourceBroker.GetImage(ResourceBroker.ResourceID.SAVE_MAIL_IMAGE);
                        break;

                    case RibbonIDs.SAVE_ATTACHMENTS:
                        img = ResourceBroker.GetImage(ResourceBroker.ResourceID.SAVE_ATTACHMENTS_IMAGE);
                        break;

                    case RibbonIDs.INSERT_ATTACHMENTS:
                        img = ResourceBroker.GetImage(ResourceBroker.ResourceID.INSERT_ATTACHMENTS_IMAGE);
                        break;
                }
            }
            catch (Exception ex)
            {
                ThisAddIn.Log.Error("{@ex}", ex);
            }

            return img;
        }

        public string OnGetSuperTip(Office.IRibbonControl control)
        {
            string tip = string.Empty;

            try
            {
                ThisAddIn.LogMethod(string.Format("Ribbon Control Id: {0}", control.Id));

                dynamic context = control.Context;

            }
            catch (Exception ex)
            {
                ThisAddIn.Log.Error("{@ex}", ex);
            }

            return tip;
        }

        public void OnAction(Office.IRibbonControl control)
        {
            try
            {
                dynamic context = control.Context;

                if(context is Outlook.Explorer)
                {
                    HandleExplorerAction(control);
                }
                else if(context is Outlook.Inspector)
                {
                    HandleInspectorAction(control);
                }
                else
                {
                    ThisAddIn.LogMethod(string.Format("Non-Explorer/Inspector Ribbon Control Context Id: {0}", control.Id));
                }
            }
            catch(Exception ex)
            {
                ThisAddIn.Log.Error("{@ex}", ex);
            }
        }

        private void HandleExplorerAction(Office.IRibbonControl control)
        {
            try
            {
                ThisAddIn.LogMethod(string.Format("Ribbon Control Id: {0}", control.Id));

                Outlook.Explorer expl = control.Context as Outlook.Explorer;

                if(expl != null)
                {
                    switch (control.Id)
                    {
                        case RibbonIDs.SAVE_MAIL:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ThisAddIn.Log.Error("{@ex}", ex);
            }
        }

        private void HandleInspectorAction(Office.IRibbonControl control)
        {
            try
            {
                ThisAddIn.LogMethod(string.Format("Ribbon Control Id: {0}", control.Id));

                Outlook.Inspector insp = control.Context as Outlook.Inspector;

                if(insp != null)
                {
                    switch(control.Id)
                    {
                        case RibbonIDs.INSERT_ATTACHMENTS:
                            Outlook.MailItem mail = insp.CurrentItem as Outlook.MailItem;

                            if (mail != null)
                            {
                                if (mail.Sent)
                                {
                                    MessageBox.Show("Woo woo Inspector read mode");
                                }
                                else
                                {
                                    MessageBox.Show("Woo woo Inspector compose mode");
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ThisAddIn.Log.Error("{@ex}", ex);
            }
        }

        public bool OnGetEnabled(Office.IRibbonControl control)
        {
            bool enabled = true;

            try
            {
                ThisAddIn.LogMethod(string.Format("Ribbon Control Id: {0}", control.Id));

                dynamic context = control.Context;

                if (context is Outlook.Explorer)
                {
                    enabled = HandleExplorerEnablement(control);
                }
                else if(context is Outlook.Inspector)
                {
                    enabled = HandleInspectorEnablement(control);
                }
                else
                {
                    ThisAddIn.LogMethod(string.Format("Non-Explorer/Inspector Ribbon Control Context Id: {0}", control.Id));
                }
            }
            catch (Exception ex)
            {
                ThisAddIn.Log.Error("{@ex}", ex);
            }

            return enabled;
        }

        private bool HandleExplorerEnablement(Office.IRibbonControl control)
        {
            bool enabled = false;

            try
            {
                ThisAddIn.LogMethod(string.Format("Ribbon Control Id: {0}", control.Id));

                Outlook.Explorer expl = control.Context as Outlook.Explorer;

                if (expl != null)
                {
                    if (control.Id == RibbonIDs.SETTINGS_LAUNCHER)
                        return enabled = false;

                    Outlook.Selection sel = expl.Selection;

                    if (sel is IEnumerable<Outlook.MailItem>)
                        MessageBox.Show("poing");

                    if(sel != null && sel[1] is Outlook.MailItem)
                    {
                        switch (control.Id)
                        {
                            case RibbonIDs.SAVE_MAIL:
                                if (sel.Count > 0)
                                    enabled = true;
                                break;

                            case RibbonIDs.SAVE_ATTACHMENTS:
                                if (sel.Count > 0)
                                {
                                    // Enable only when all selected items have attachments.
                                    // LINQ query to find items with attachments, compare count to selection count, et voila
                                    if (sel.Count == (from items in sel.Cast<Outlook.MailItem>() where items.Attachments.Count > 0 select items).Count())
                                        enabled = true;
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ThisAddIn.Log.Error("{@ex}", ex);
            }

            return enabled;
        }

        private bool HandleInspectorEnablement(Office.IRibbonControl control)
        {
            bool enabled = false;

            try
            {
                ThisAddIn.LogMethod(string.Format("Ribbon Control Id: {0}", control.Id));

                Outlook.Inspector insp = control.Context as Outlook.Inspector;

                if (insp != null)
                {
                    switch(control.Id)
                    {
                        case RibbonIDs.SAVE_ATTACHMENTS:
                            Outlook.MailItem mail = insp.CurrentItem as Outlook.MailItem;

                            if (mail != null)
                            {
                                if (mail.Attachments.Count > 0)
                                    enabled = true;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ThisAddIn.Log.Error("{@ex}", ex);
            }

            return enabled;
        }

        #endregion

        #region Helpers

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
