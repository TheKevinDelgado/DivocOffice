using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DivocCommon.DataModel.Teams
{
    public class TeamInfo
    {
        public string id { get; set; }
        public DateTime? createdDateTime { get; set; }
        public string displayName { get; set; }
        public string description { get; set; }
        public string internalId { get; set; }
        public string classification { get; set; }
        public string specialization { get; set; }
        public string visibility { get; set; }
        public string webUrl { get; set; }
        public bool? isArchived { get; set; }
        public bool? isMembershipLimitedToOwners { get; set; }
        public string memberSettings { get; set; }
        public string guestSettings { get; set; }
        public string messagingSettings { get; set; }
        public string funSettings { get; set; }
        public string discoverySettings { get; set; }

        public List<ChannelInfo> Channels { get; set; }
    }
}
