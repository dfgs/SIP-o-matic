using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.DataSources
{
	public struct Event
	{
		public DateTime Timestamp
		{
			get;
			private set;
		}
		public string Message
		{
			get;
			private set;
		}

		public string SourceAddress
		{
			get;
			private set;
		}

		public string DestinationAddress
		{
			get;
			private set;
		}


		public Event(DateTime Timestamp, string SourceAddress, string DestinationAddress, string Message)
		{
			this.Timestamp = Timestamp;
			this.SourceAddress = SourceAddress;
			this.DestinationAddress = DestinationAddress;
			this.Message = Message;
		}
	}
}
