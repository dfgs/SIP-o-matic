using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudiocodesSyslogLib
{
	public class Notification
	{
		public DateTime Timestamp
		{
			get;
			private set;
		}

		public ulong NotificationID
		{
			get;
			private set;
		}

		public string Content
		{
			get;
			private set;
		}

		public Notification(DateTime Timestamp, ulong NotificationID, string Content)
		{
			this.Timestamp = Timestamp;
			this.NotificationID = NotificationID;
			this.Content = Content;
		}


	}
}
