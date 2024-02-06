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

		public required string Source
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
		public TelephonyEvent(string Source,string Message,string CallID)
		{
			this.Source = Source;	
			this.Message = Message;
			this.CallID = CallID;
		}

	}
}
