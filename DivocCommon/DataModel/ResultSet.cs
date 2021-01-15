using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DivocCommon.DataModel
{
    public class ResultSet
    {
        [JsonProperty("@odata.context")]
        public string Context { get; set; }
        [JsonProperty("value")]
        public List<DriveItem> Items { get; set; }
    }
}
