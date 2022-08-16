using LogLib;
using SIP_o_matic.DataSources;
using SIPParserLib;
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
		public abstract string? Display
		{
			get;
		}
		public abstract string? ShortDisplay
		{
			get;
		}
		public DateTime Timestamp
		{
			get => Event.Timestamp;
		}
	

		public string SourceAddress
		{
			get => Event.SourceAddress;
		}

		public string DestinationAddress
		{
			get => Event.DestinationAddress;
		}

		public abstract string? From
		{
			get;
		}

		public abstract string? To
		{
			get;
		}

		public abstract string? CSeq
		{
			get;
		}

		public abstract string? ViaBranch
		{
			get;
		}


		public abstract string? CallID
		{
			get;
		}
		public abstract string? FromTag
		{
			get;
		}
		public abstract string? ToTag
		{
			get;
		}

		public Event Event
		{
			get;
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

		

		public SIPMessageViewModel(ILogger Logger,Event Event):base(Logger)
		{
			SourceFiles = new ObservableCollection<FileViewModel>();
			this.Event = Event;
			//this.From = From;this.To = To;
		}

		protected void OnPropertiesChanged()
		{
			OnPropertyChanged(nameof(Count));

		}

		public void AddSourceFile(FileViewModel FileViewModel)
		{
			SourceFiles.Add(FileViewModel);
			OnPropertiesChanged();
		}

		public void RemoveSourceFile(FileViewModel FileViewModel)
		{
			SourceFiles.Remove(FileViewModel);
			OnPropertiesChanged();
		}
	}
}
