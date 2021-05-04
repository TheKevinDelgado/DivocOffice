using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DivocCommon.DataModel
{
    public class FolderInfo
    {
        [JsonProperty("childCount")]
        public int ChildCount { get; set; }
    }
}
