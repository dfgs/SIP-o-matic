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
		public required uint MessageIndex
		{
			get;
			set;
		}

		

		public TelephonyEvent() 
		{ 
		}

		[SetsRequiredMembers]
		public TelephonyEvent(DateTime Timestamp,string CallID,string SourceAddress,string DestinationAddress,uint MessageIndex)
		{
			this.Timestamp = Timestamp;
			this.CallID = CallID;
			this.SourceAddress = SourceAddress;
			this.DestinationAddress = DestinationAddress;
			this.MessageIndex = MessageIndex;
		}

	}
}
