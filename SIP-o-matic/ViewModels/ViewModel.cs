using LogLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SIP_o_matic.ViewModels
{
	public abstract class ViewModel : DependencyObject, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		private ILogger logger;
		protected ILogger Logger
		{
			get => logger;
		}

		public ViewModel(ILogger Logger)
		{
			this.logger = Logger;
			
		}

		protected virtual void OnPropertyChanged(string PropertyName)
		{
			if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
		}

		
		protected void Log(LogLevels Level,string Message, [CallerMemberName] string CallerName = "")
		{
			logger.Log(0, GetType().Name, CallerName, Level, Message);
		}

	}
}
