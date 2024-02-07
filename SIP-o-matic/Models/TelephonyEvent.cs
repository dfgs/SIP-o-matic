using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.Models
{
	public class TelephonyEvent
	{
		public required DateTime Timestamp
		{
			get;
			set;
		}
		public required string SourceAddress
		{
			get;
			set;
		}
		public required string DestinationAdress
		{
			get;
			set;
		}
		public required string Message
		{
			get;
			set;
		}

		public required string CallID
		{
			get; 
			set;
		}

		public TelephonyEvent() 
		{ 
		}

		[SetsRequiredMembers]
		public TelephonyEvent(DateTime Timestamp,string CallID,string SourceAddress,string DestinationAddress,string Message)
		{
			this.Timestamp = Timestamp;
			this.CallID = CallID;
			this.SourceAddress = SourceAddress;
			this.DestinationAdress = DestinationAddress;
			this.Message = Message;
		}

	}
}
