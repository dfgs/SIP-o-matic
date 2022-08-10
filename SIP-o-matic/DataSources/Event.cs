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

		public Event(DateTime Timestamp,string Message)
		{
			this.Timestamp = Timestamp;this.Message = Message;
		}
	}
}
