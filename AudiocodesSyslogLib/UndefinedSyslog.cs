using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudiocodesSyslogLib
{
	public class UndefinedSyslog:Syslog
	{
		

		public UndefinedSyslog(DateTime Timestamp, string Address, string Severity, ulong SequenceNumber,string Content):base(Timestamp, Address, Severity, SequenceNumber,Content)
		{
		}









	}
}
