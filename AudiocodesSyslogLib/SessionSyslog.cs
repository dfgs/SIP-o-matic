using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudiocodesSyslogLib
{
	public class SessionSyslog:Syslog
	{
		public string SessionId 
		{ 
			get; 
			private set; 
		}

		public SessionSyslog(DateTime Timestamp, string Address, string Severity, ulong SequenceNumber,string SessionID,string Content):base(Timestamp, Address, Severity, SequenceNumber,Content)
		{
			this.SessionId = SessionID;
		}









	}
}
