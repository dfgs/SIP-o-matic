using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AudiocodesSyslogLib
{
	public class BoardSyslog:Syslog
	{
		public string BoardId 
		{
			get;
			private set;
		}

		public BoardSyslog(DateTime Timestamp, string Address, string Severity, ulong SequenceNumber,string BoardID,string Content) :base(Timestamp, Address, Severity, SequenceNumber,Content)
		{
			this.BoardId = BoardID;
		}









	}
}
