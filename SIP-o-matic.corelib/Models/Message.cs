using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SIP_o_matic.corelib.Models
{
    public class Message
    {
		[XmlAttribute]
		public required uint Index
		{
			get;
			set;
		}

		[XmlAttribute]
		public required DateTime Timestamp
        {
            get;
            set;
        }

        [XmlIgnore]
        public required string Content
        {
            get;
            set;
        }

        public string EncodedContent
        {
            get => Convert.ToBase64String( Encoding.UTF8.GetBytes(Content));
            set => Content=Encoding.UTF8.GetString(Convert.FromBase64String(value));
		}

		[XmlAttribute]
		public required string SourceAddress
        {
            get;
            set;
        }

		[XmlAttribute]
		public required string DestinationAddress
        {
            get;
            set;
        }

        public Message()
        {

        }
        [SetsRequiredMembers]
        public Message(uint Index, DateTime Timestamp, string SourceAddress, string DestinationAddress, string Content)
        {
            this.Index= Index;
            this.Timestamp = Timestamp;
            this.SourceAddress = SourceAddress;
            this.DestinationAddress = DestinationAddress;
            this.Content = Content;
        }
    }
}
