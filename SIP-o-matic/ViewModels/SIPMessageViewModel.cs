using LogLib;
using SIP_o_matic.DataSources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.ViewModels
{
	public abstract class SIPMessageViewModel:ViewModel
	{
		public abstract string Display
		{
			get;
		}

		public DateTime Timestamp
		{
			get => Event.Timestamp;
		}
		public string From
		{
			get;
			private set;
		}
		public string To
		{
			get;
			private set;
		}

		public Event Event
		{
			get;
			private set;
		}

		public string RawMessage
		{
			get => Event.Message;
		}

		public ObservableCollection<FileViewModel> SourceFiles
		{
			get;
			private set;
		}

		public int Count
		{
			get => SourceFiles.Count;
		}

		public int UID
		{
			get => RawMessage.GetHashCode();
		}

		/*public abstract string CallID
		{
			get;
		}*/

		public SIPMessageViewModel(ILogger Logger,Event Event,string From,string To):base(Logger)
		{
			SourceFiles = new ObservableCollection<FileViewModel>();
			this.Event = Event;
			this.From = From;this.To = To;
		}


		public void AddSourceFile(FileViewModel FileViewModel)
		{
			SourceFiles.Add(FileViewModel);
			OnPropertyChanged("Count");
		}

		public void RemoveSourceFile(FileViewModel FileViewModel)
		{
			SourceFiles.Remove(FileViewModel);
			OnPropertyChanged("Count");
		}
	}
}
