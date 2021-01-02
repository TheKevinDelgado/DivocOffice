using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Drawing;
using Office = Microsoft.Office.Core;

namespace DivocCommon
{
    /// <summary>
    /// Base class to provide some common functionality needed for all add-ins.
    /// Each add-in will should add a derived class to handle application 
    /// specific stuff. The derived class will need to implement IRibbonExtensibility
    /// and call the base class helpers as needed.
    /// </summary>
    [ComVisible(true)]
    public abstract class OfficeRibbonManagerBase : Office.IRibbonExtensibility
    {
        protected Assembly _asm = null;

        protected OfficeRibbonManagerBase()
        {
            _asm = Assembly.GetCallingAssembly();
        }

        public Office.IRibbonUI Ribbon { get; private set; }

        #region Ribbon Callbacks
        //Create callback methods here. For more information about adding callback methods, visit https://go.microsoft.com/fwlink/?LinkID=271226

        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            Ribbon = ribbonUI;
        }

        public string OnGetLabel(Office.IRibbonControl control)
        {
            string label = string.Empty;

            try
            {
                LogManager.LogMethod(string.Format("Ribbon Control Id: {0}", control.Id));

                string id = control.Id;

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
                    case RibbonIDs.SAVE_DOCUMENT:
                    case RibbonIDs.SAVE_PRESENTATION:
                    case RibbonIDs.SAVE_WORKBOOK:
                        label = ResourceBroker.GetString(ResourceBroker.ResourceID.SAVE_LABEL);
                        break;

                    case RibbonIDs.SAVE_ATTACHMENTS:
                        label = ResourceBroker.GetString(ResourceBroker.ResourceID.SAVE_ATTACHMENTS_LABEL);
                        break;

                    case RibbonIDs.OPEN_DOCUMENT:
                    case RibbonIDs.OPEN_PRESENTATION:
                    case RibbonIDs.OPEN_WORKBOOK:
                        label = ResourceBroker.GetString(ResourceBroker.ResourceID.OPEN_LABEL);
                        break;
                }
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
            }

            return label;
        }

        public Bitmap OnGetImage(Office.IRibbonControl control)
        {
            Bitmap img = null;

            try
            {
                LogManager.LogMethod(string.Format("Ribbon Control Id: {0}", control.Id));

                string id = control.Id;

                switch (id)
                {
                    case RibbonIDs.DIVOC_GROUP:
                    case RibbonIDs.DIVOC_GROUP_INLINE:
                        // When ribbon is squished, a group can be collapse into a single drop-down
                        // In this case it will request an image for the group to display
                        img = ResourceBroker.GetImage(ResourceBroker.ResourceID.PRODUCT_IMAGE);
                        break;

                    case RibbonIDs.SAVE_MAIL:
                    case RibbonIDs.SAVE_DOCUMENT:
                    case RibbonIDs.SAVE_PRESENTATION:
                    case RibbonIDs.SAVE_WORKBOOK:
                        img = ResourceBroker.GetImage(ResourceBroker.ResourceID.SAVE_IMAGE);
                        break;

                    case RibbonIDs.SAVE_ATTACHMENTS:
                        img = ResourceBroker.GetImage(ResourceBroker.ResourceID.SAVE_ATTACHMENTS_IMAGE);
                        break;

                    case RibbonIDs.INSERT_ATTACHMENTS:
                        img = ResourceBroker.GetImage(ResourceBroker.ResourceID.INSERT_ATTACHMENTS_IMAGE);
                        break;

                    case RibbonIDs.OPEN_DOCUMENT:
                    case RibbonIDs.OPEN_PRESENTATION:
                    case RibbonIDs.OPEN_WORKBOOK:
                        img = ResourceBroker.GetImage(ResourceBroker.ResourceID.OPEN_IMAGE);
                        break;
                }
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
            }

            return img;
        }

        public string OnGetSuperTip(Office.IRibbonControl control)
        {
            string tip = string.Empty;

            try
            {
                LogManager.LogMethod(string.Format("Ribbon Control Id: {0}", control.Id));

                string id = control.Id;

                switch (id)
                {
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
            }

            return tip;
        }
        public virtual bool OnGetEnabled(Office.IRibbonControl control)
        {
            LogManager.LogMethod(string.Format("Unhandled Ribbon Control Context. Id: {0}", control.Id));

            return false;
        }

        public virtual void OnAction(Office.IRibbonControl control)
        {
            LogManager.LogMethod(string.Format("Unhandled Ribbon Control Context. Id: {0}", control.Id));
        }

        #endregion

        protected string GetResourceText(string resourceName)
        {
            if(_asm != null)
            {
                string[] resourceNames = _asm.GetManifestResourceNames();
                for (int i = 0; i < resourceNames.Length; ++i)
                {
                    if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        using (StreamReader resourceReader = new StreamReader(_asm.GetManifestResourceStream(resourceNames[i])))
                        {
                            if (resourceReader != null)
                            {
                                return resourceReader.ReadToEnd();
                            }
                        }
                    }
                }

            }

            return null;
        }

        #region IRibbonExtensibility Members

        public virtual string GetCustomUI(string RibbonID)
        {
            LogManager.LogMethod(string.Format("Unhandled Ribbon Id: {0}", RibbonID));

            return null;
        }

        #endregion
    }
}
