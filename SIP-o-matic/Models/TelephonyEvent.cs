using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.Models
{
	public enum TelephonyEventTypes {CallPlaced,CallReceived, CallRinging,CallHold,CallResumed,SessionRefreshed};

	public class TelephonyEvent
	{
		public required DateTime Timestamp
		{
			get;
			set;
		}
		public required string CallID
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
		public required Address FromURI
		{
			get;
			set;
		}
		public required Address ToURI
		{
			get;
			set;
		}

		public required uint MessageIndex
		{
			get;
			set;
		}

		public required	TelephonyEventTypes EventType
		{
			get; 
			set; 
		}

		public TelephonyEvent() 
		{ 
		}

		[SetsRequiredMembers]
		public TelephonyEvent(DateTime Timestamp,string CallID,string SourceAddress,string DestinationAddress, Address FromURI, Address ToURI, uint MessageIndex)
		{
			this.Timestamp = Timestamp;
			this.CallID = CallID;
			this.SourceAddress = SourceAddress;
			this.DestinationAddress = DestinationAddress;
			this.FromURI = FromURI;
			this.ToURI = ToURI;
			this.MessageIndex = MessageIndex;
		}

	}
}
