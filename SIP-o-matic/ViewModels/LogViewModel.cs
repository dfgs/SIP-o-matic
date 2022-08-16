using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.ViewModels
{
	public class LogViewModel : ViewModel
	{
		public DateTime DateTime
		{
			get;
			private set;
		}
		public int ComponentID
		{
			get;
			private set;
		}
		public string ComponentName
		{
			get;
			private set;
		}
		public string MethodName
		{
			get;
			private set;
		}
		public LogLevels Level
		{
			get;
			private set;
		}
		public string Message
		{
			get;
			private set;
		}

		public LogViewModel(DateTime DateTime, int ComponentID, string ComponentName, string MethodName, LogLevels Level, string Message) : base(NullLogger.Instance)
		{
			this.DateTime = DateTime;this.ComponentID = ComponentID;this.ComponentName = ComponentName;this.MethodName = MethodName;this.Level = Level;this.Message = Message;
		}

	}
}
