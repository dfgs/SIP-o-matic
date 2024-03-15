using SIPParserLib;
using System.Diagnostics.CodeAnalysis;
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

		/*[XmlIgnore]
		public SIPMessage? SIPMessage
		{
			get;
			private set;
		}*/

		[XmlIgnore]
		public string DialogColor
		{
			get;
			set;
		}

		[XmlIgnore]
		public string TransactionColor
		{
			get;
			set;
		}

		public string EncodedContent
        {
            get => Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Content));
            set => Content= System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value));
		}

		public required Address SourceAddress
        {
            get;
            set;
        }

		public required Address DestinationAddress
        {
            get;
            set;
        }

        public Message()
        {
            this.DialogColor = "Black";
            this.TransactionColor = "Black";
			this.Content = "";
        }
        [SetsRequiredMembers]
        public Message(uint Index, DateTime Timestamp, Address SourceAddress, Address DestinationAddress, string Content)
        {
            this.Index= Index;
            this.Timestamp = Timestamp;
            this.SourceAddress = SourceAddress;
            this.DestinationAddress = DestinationAddress;
			this.Content = Content;
			this.DialogColor = "Black";
			this.TransactionColor = "Black";
			
		}

		

	}
}
