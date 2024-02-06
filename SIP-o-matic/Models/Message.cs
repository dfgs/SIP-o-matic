using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.Models
{
    public class Message
    {
        public required DateTime Timestamp
        {
            get;
            set;
        }
        public required string Content
        {
            get;
            set;
        }

        public required string SourceAddress
        {
            get;
            set;
        }

        public required string DestinationAddress
        {
            get;
            set;
        }

        public Message()
        {

        }
		[SetsRequiredMembers]
		public Message(DateTime Timestamp, string SourceAddress, string DestinationAddress, string Content)
        {
            this.Timestamp = Timestamp;
            this.SourceAddress = SourceAddress;
            this.DestinationAddress = DestinationAddress;
            this.Content = Content;
        }
    }
}
