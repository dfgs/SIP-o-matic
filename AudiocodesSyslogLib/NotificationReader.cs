using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AudiocodesSyslogLib
{
	public  class NotificationReader: INotificationReader
	{
		private static Regex notificationRegex = new Regex(@"\(N +(?<NotificationID>\d+)\) +(?<Message>.+)");

		private ISyslogReader syslogReader;

		public NotificationReader(ISyslogReader SyslogReader)
		{
			if (SyslogReader == null) throw new ArgumentNullException(nameof(SyslogReader));
			this.syslogReader = SyslogReader;
		}

		public async IAsyncEnumerable<Notification> ReadNotificationsAsync(Stream Stream)
		{
			Match match;
			SessionSyslog? sessionSyslog;
			DateTime timeStamp;
			ulong notificationID;

			string? currentBlock = null;

			if (Stream == null) throw new ArgumentNullException(nameof(Stream));

			timeStamp = DateTime.MinValue;
			notificationID = 0;

			await foreach (Syslog syslog in syslogReader.ReadSyslogsAsync(Stream))
			{
				sessionSyslog = syslog as SessionSyslog;
				if (sessionSyslog == null) continue;
				if (sessionSyslog.Severity != "local0.notice") continue;
				
				foreach(string line in sessionSyslog.Content.Split("\r\n"))
				{
					if (line == null) continue;

					match = notificationRegex.Match(line);
					if (match.Success)
					{
						if (!string.IsNullOrEmpty(currentBlock)) yield return new Notification(timeStamp,notificationID, currentBlock);
						currentBlock = match.Groups["Message"].Value;
						notificationID = ulong.Parse(match.Groups["NotificationID"].Value);
						timeStamp = sessionSyslog.Timestamp;
					}
					else
					{
						if (!string.IsNullOrEmpty(currentBlock)) currentBlock += "\r\n"+line ;
					}
				}
	
			}
			if (!string.IsNullOrEmpty(currentBlock)) yield return new Notification(timeStamp, notificationID, currentBlock); ;
		}


		



	}
}
