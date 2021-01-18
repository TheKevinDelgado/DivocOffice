using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DivocCommon.DataModel.Teams
{
    public class ChannelMessageBody
    {
        public string contentType { get; set; }
        /// <summary>
        /// Content of the message.
        /// </summary>
        /// <notes>
        /// This may need to be dynamic, based on the contentType. Test.
        /// </notes>
        public string content { get; set; }
    }
}
