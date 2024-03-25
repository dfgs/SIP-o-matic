using SIPParserLib;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace SIP_o_matic.corelib.Models
{
    public class RTPStart: IEvent
	{
		

		[XmlAttribute]
		public required DateTime Timestamp
        {
            get;
            set;
        }

		[XmlIgnore]
		public string DialogColor
		{
			get;
			set;
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
		public required ushort DestinationPort
		{
			get;
			set;
		}

		public RTPStart()
        {
			this.DialogColor = "Black";

		}
		[SetsRequiredMembers]
        public RTPStart(DateTime Timestamp, Address SourceAddress, Address DestinationAddress, ushort DestinationPort)
        {
            this.Timestamp = Timestamp;
            this.SourceAddress = SourceAddress;
            this.DestinationAddress = DestinationAddress;
			this.DestinationPort = DestinationPort;
			this.DialogColor = "Black";

		}



	}
}
