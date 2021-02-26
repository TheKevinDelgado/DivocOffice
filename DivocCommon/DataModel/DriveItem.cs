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
        [JsonProperty("createdDateTime")]
        public DateTime CreatedDateTime { get; set; }
        [JsonProperty("eTag")]
        public string ETag { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("lastModifiedDateTime")]
        public DateTime LastModifiedDateTime { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("webUrl")]
        public string WebUrl { get; set; }
        [JsonProperty("cTag")]
        public string CTag { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
        [JsonProperty("createdBy")]
        public ActionInfo CreatedBy { get; set; }
        [JsonProperty("lastModifiedBy")]
        public ActionInfo LastModifiedBy { get; set; }
        [JsonProperty("parentReference")]
        public ParentInfo ParentReference { get; set; }
        [JsonProperty("file")]
        public FileInfo File { get; set; }
        [JsonProperty("fileSystemInfo")]
        public FileSystemInfo FileSystemInfo { get; set; }
        [JsonProperty("folder")]
        public FolderInfo Folder { get; set; }
        [JsonProperty("webDavUrl")]
        public string WebDavUrl { get; set; }
        [JsonProperty("root")]
        public dynamic Root { get; set; }
    }
}
