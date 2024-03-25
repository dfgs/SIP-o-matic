using SIPParserLib;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace SIP_o_matic.corelib.Models
{
    public class UDPStream: IEvent
	{
		

		[XmlAttribute]
		public required DateTime Timestamp
        {
            get;
            set;
        }

		[XmlAttribute]
		public required DateTime LastTimestamp
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

		[XmlAttribute]
		public required ushort DestinationPort
		{
			get;
			set;
		}
		public UDPStream()
        {
        }
        [SetsRequiredMembers]
        public UDPStream(DateTime Timestamp, Address SourceAddress, Address DestinationAddress,ushort DestinationPort)
        {
            this.Timestamp = Timestamp;
			this.LastTimestamp = Timestamp;
            this.SourceAddress = SourceAddress;
            this.DestinationAddress = DestinationAddress;
            this.DestinationPort = DestinationPort;
			
		}

		public bool Matches(UDPStream Transmission)
		{
			return this.SourceAddress.Equals(Transmission.SourceAddress) && this.DestinationAddress.Equals(Transmission.DestinationAddress) && (this.DestinationPort == Transmission.DestinationPort);
		}

		public override string ToString()
		{
			return $"{SourceAddress} -> {DestinationAddress}:{DestinationPort}";
		}

	}
}
