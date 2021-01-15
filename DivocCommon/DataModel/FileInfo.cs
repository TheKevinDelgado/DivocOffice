using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DivocCommon.DataModel
{
    public class FileInfo
    {
        public class HashInfo
        {
            public string quickXorHash { get; set; }
        }

        public string mimeType { get; set; }

        public HashInfo hashes { get; set; }
    }
}
