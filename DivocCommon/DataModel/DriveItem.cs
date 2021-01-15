using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DivocCommon.DataModel
{
    public class DriveItem
    {
        [JsonProperty("@microsoft.graph.downloadUrl")]
        public string GraphDownloadURL { get; set; }
        public DateTime createdDateTime { get; set; }
        public string eTag { get; set; }
        public string id { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
        public string name { get; set; }
        public string webUrl { get; set; }
        public string cTag { get; set; }
        public int size { get; set; }
        public ActionInfo createdBy { get; set; }
        public ActionInfo lastModifiedBy { get; set; }
        public ParentInfo parentReference { get; set; }
        public FileInfo file { get; set; }
        public FileSystemInfo fileSystemInfo { get; set; }
        public FolderInfo folder { get; set; }
        public string webDavUrl { get; set; }
    }
}
