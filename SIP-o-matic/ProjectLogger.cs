using LogLib;
using SIP_o_matic.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic
{
	public class ProjectLogger : BaseLogger
	{

		public ObservableCollection<LogViewModel> Logs
		{
			get;
			private set;
		}

		public ProjectLogger()
		{
			Logs = new ObservableCollection<LogViewModel>();
		}
		
		public override void Log(Log Log)
		{
			Logs.Add(new LogViewModel(Log.DateTime, Log.ComponentID, Log.ComponentName, Log.MethodName, Log.Level, Log.Message));
		}

	}
}
