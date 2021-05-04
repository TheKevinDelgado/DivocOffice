using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DivocCommon.DataModel.Teams
{
    public class ChannelResultSet : ResultSetBase
    {
        [JsonProperty("value")]
        public List<ChannelInfo> Items { get; set; }
    }
}
