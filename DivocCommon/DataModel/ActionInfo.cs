using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DivocCommon.DataModel
{
    public class ActionInfo
    {
        [JsonProperty("user")]
        public IdentityInfo User { get; set; }
    }
}
