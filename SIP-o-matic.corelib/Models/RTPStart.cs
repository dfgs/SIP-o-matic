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

        public RTPStart()
        {
           
        }
        [SetsRequiredMembers]
        public RTPStart(DateTime Timestamp, Address SourceAddress, Address DestinationAddress)
        {
            this.Timestamp = Timestamp;
            this.SourceAddress = SourceAddress;
            this.DestinationAddress = DestinationAddress;
			
		}

		

	}
}
