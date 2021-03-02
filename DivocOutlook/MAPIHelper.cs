using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DivocOutlook
{
    /// <summary>
    /// Utility class with constants for MAPI property schema strings and custom property values.
    /// </summary>
    public sealed class MAPIHelper
    {
        public const string Prop_String = "http://schemas.microsoft.com/mapi/string/{FFF40745-D92F-4C11-9E14-92701F001EB3}/Divoc";

        public const string Value_Attachment = "Divoc.Attachment";
    }
}
