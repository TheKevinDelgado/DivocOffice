using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using System.Drawing;

namespace DivocCommon
{
    public static class ResourceBroker
    {
        public sealed class ResourceID
        {
            public const string PRODUCT_NAME = "ProductName";
            public const string INSERT_ATTACHMENTS_LABEL = "InsertAttachmentsLabel";
            public const string SAVE_MAIL_LABEL = "SaveMailLabel";
            public const string SAVE_ATTACHMENTS_LABEL = "SaveAttachmentsLabel";
            public const string SAVE_MAIL_IMAGE = "SaveMailImage";
            public const string SAVE_ATTACHMENTS_IMAGE = "SaveAttachmentsImage";
            public const string INSERT_ATTACHMENTS_IMAGE = "InsertAttachmentsImage";
        }

        public static string GetString(string id)
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

                case ResourceID.SAVE_MAIL_LABEL:
                    str = Properties.Resource.SaveMailLabel;
                    break;

                case ResourceID.SAVE_ATTACHMENTS_LABEL:
                    str = Properties.Resource.SaveAttachmentsLabel;
                    break;
            }

            return str;
        }

        public static Bitmap GetImage(string id)
        {
            Bitmap img = null;

            switch(id)
            {
                case ResourceID.SAVE_MAIL_IMAGE:
                    img = Properties.Resource.SaveMailIcon;
                    break;

                case ResourceID.SAVE_ATTACHMENTS_IMAGE:
                    img = Properties.Resource.SaveAttachmentsIcon;
                    break;

                case ResourceID.INSERT_ATTACHMENTS_IMAGE:
                    img = Properties.Resource.InsertAttachmentsIcon;
                    break;
            }

            return img;
        }
    }
}
