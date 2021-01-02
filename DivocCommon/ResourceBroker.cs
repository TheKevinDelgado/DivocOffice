using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using System.Drawing;

namespace DivocCommon
{
    /// <summary>
    /// Handler for getting resources from the common dll to the add-ins to facilitate
    /// localization efforts being centralized.
    /// </summary>
    public static class ResourceBroker
    {
        /// <summary>
        /// Identifiers used by callers to map to a resource. 
        /// </summary>
        public enum ResourceID
        {
            PRODUCT_NAME,
            PRODUCT_IMAGE,

            SAVE_LABEL,
            SAVE_IMAGE,

            SAVE_ATTACHMENTS_LABEL,
            SAVE_ATTACHMENTS_IMAGE,

            INSERT_ATTACHMENTS_LABEL,
            INSERT_ATTACHMENTS_IMAGE,

            OPEN_LABEL,
            OPEN_IMAGE,
        }
           
        public static string GetString(ResourceID id)
        {
            string str = string.Empty;

            switch(id)
            {
                case ResourceID.PRODUCT_NAME:
                    str = Properties.Resource.ProductName;
                    break;

                case ResourceID.INSERT_ATTACHMENTS_LABEL:
                    str = Properties.Resource.InsertAttachmentsLabel;
                    break;

                case ResourceID.SAVE_LABEL:
                    str = Properties.Resource.SaveLabel;
                    break;

                case ResourceID.SAVE_ATTACHMENTS_LABEL:
                    str = Properties.Resource.SaveAttachmentsLabel;
                    break;

                case ResourceID.OPEN_LABEL:
                    str = Properties.Resource.OpenLabel;
                    break;
            }

            return str;
        }

        public static Bitmap GetImage(ResourceID id)
        {
            Bitmap img = null;

            switch(id)
            {
                case ResourceID.PRODUCT_IMAGE:
                    img = Properties.Resource.ProductLogo;
                    break;

                case ResourceID.SAVE_IMAGE:
                    img = Properties.Resource.SaveIcon;
                    break;

                case ResourceID.SAVE_ATTACHMENTS_IMAGE:
                    img = Properties.Resource.SaveAttachmentsIcon;
                    break;

                case ResourceID.INSERT_ATTACHMENTS_IMAGE:
                    img = Properties.Resource.InsertAttachmentsIcon;
                    break;

                case ResourceID.OPEN_IMAGE:
                    img = Properties.Resource.OpenIcon;
                    break;
            }

            return img;
        }
    }
}
