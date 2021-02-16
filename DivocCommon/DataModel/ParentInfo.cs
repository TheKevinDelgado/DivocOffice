using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DivocCommon.DataModel
{
    public class ParentInfo
    {
        [JsonProperty("driveId")]
        public string DriveId { get; set; }
        [JsonProperty("driveType")]
        public string DriveType { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("path")]
        public string Path { get; set; }
    }
}
