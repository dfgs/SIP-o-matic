using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.Models
{
    public struct Message
    {
        public DateTime Timestamp
        {
            get;
            private set;
        }
        public string Content
        {
            get;
            private set;
        }

        public string SourceAddress
        {
            get;
            private set;
        }

        public string DestinationAddress
        {
            get;
            private set;
        }


        public Message(DateTime Timestamp, string SourceAddress, string DestinationAddress, string Content)
        {
            this.Timestamp = Timestamp;
            this.SourceAddress = SourceAddress;
            this.DestinationAddress = DestinationAddress;
            this.Content = Content;
        }
    }
}
