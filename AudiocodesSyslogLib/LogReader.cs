using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AudiocodesSyslogLib
{
	public  class LogReader: ILogReader
	{
		private static Regex eventRegex = new Regex(@"\(N +(?<EventID>\d+)\) +(?<Message>.+)");

		private ISyslogReader syslogReader;

		public LogReader(ISyslogReader SyslogReader)
		{
			if (SyslogReader == null) throw new ArgumentNullException(nameof(SyslogReader));
			this.syslogReader = SyslogReader;
		}

		public async IAsyncEnumerable<string> ReadLogsAsync(Stream Stream)
		{
			Match match;
			SessionSyslog? sessionSyslog;

			string? currentBlock = null;

			if (Stream == null) throw new ArgumentNullException(nameof(Stream));


			await foreach (Syslog syslog in syslogReader.ReadSyslogsAsync(Stream))
			{
				sessionSyslog = syslog as SessionSyslog;
				if (sessionSyslog == null) continue;
				if (sessionSyslog.Severity != "local0.notice") continue;
				
				foreach(string line in sessionSyslog.Content.Split("\r\n"))
				{
					if (line == null) continue;

					match = eventRegex.Match(line);
					if (match.Success)
					{
						if (!string.IsNullOrEmpty(currentBlock)) yield return currentBlock;
						currentBlock = line;
					}
					else
					{
						if (!string.IsNullOrEmpty(currentBlock)) currentBlock += "\r\n"+line ;
					}
				}
	
			}
			if (!string.IsNullOrEmpty(currentBlock)) yield return currentBlock;
		}


		



	}
}
