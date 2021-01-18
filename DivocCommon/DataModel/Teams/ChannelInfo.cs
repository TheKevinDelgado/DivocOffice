using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DivocCommon.DataModel.Teams
{
    public class ChannelInfo
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string description { get; set; }
        public bool? isFavoriteByDefault { get; set; }
        public string email { get; set; }
        public string webUrl { get; set; }
        public string membershipType { get; set; }
    }
}
