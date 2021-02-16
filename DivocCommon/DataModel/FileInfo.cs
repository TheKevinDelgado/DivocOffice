using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DivocCommon.DataModel
{
    public class FileInfo
    {
        public class HashInfo
        {
            [JsonProperty("quickXorHash")]
            public string QuickXorHash { get; set; }
        }

        [JsonProperty("mimeType")]
        public string MimeType { get; set; }

        [JsonProperty("hashes")]
        public HashInfo Hashes { get; set; }
    }
}
