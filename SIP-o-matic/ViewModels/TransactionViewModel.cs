using LogLib;
using SIP_o_matic.DataSources;
using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SIP_o_matic.ViewModels
{
	public class TransactionViewModel:ViewModel
	{

		public DateTime? StartTime
		{
			get => SIPMessages.FirstOrDefault()?.Timestamp;
		}

		public DateTime? StopTime
		{
			get => SIPMessages.LastOrDefault()?.Timestamp;
		}

		public string? SourceAddress
		{
			get => SIPMessages.FirstOrDefault()?.SourceAddress;
		}

		public string? DestinationAddress
		{
			get => SIPMessages.FirstOrDefault()?.DestinationAddress;
		}

		public string? Display
		{
			get => SIPMessages.FirstOrDefault()?.Display;
		}

		public ObservableCollection<SIPMessageViewModel> SIPMessages
		{
			get;
			private set;
		}

		public int UID
		{
			get;
			private set;
		}

		public TransactionViewModel(ILogger Logger, int UID) : base(Logger)
		{
			this.UID = UID;
			SIPMessages = new ObservableCollection<SIPMessageViewModel>();
		}



		protected void OnPropertiesChanged()
		{
			OnPropertyChanged(nameof(StartTime));
			OnPropertyChanged(nameof(StopTime));
			OnPropertyChanged(nameof(SourceAddress));
			OnPropertyChanged(nameof(DestinationAddress));
			OnPropertyChanged(nameof(Display));
		}

		public SIPMessageViewModel? FindMessageByUID(int UID)
		{
			return SIPMessages.FirstOrDefault(item => item.UID == UID);
		}
			

		public void AddSIPMessage(FileViewModel FileViewModel, Event Event, SIPMessage SIPMessage)
		{
			SIPMessageViewModel? sipMessageViewModel;
			int messageUID;

			messageUID = SIPUtils.GetMessageUID(Event.Message);
			sipMessageViewModel = FindMessageByUID(messageUID);

			if (sipMessageViewModel == null)
			{
				switch (SIPMessage)
				{
					case Response response:
						sipMessageViewModel = new ResponseViewModel(Logger, Event, response);
						break;
					case Request request:
						sipMessageViewModel = new RequestViewModel(Logger, Event, request);
						break;
					default:
						return;
				}

				SIPMessages.Add(sipMessageViewModel);
			}

			sipMessageViewModel.AddSourceFile(FileViewModel);
			OnPropertiesChanged();
		}
		public void RemoveSIPMessage(FileViewModel FileViewModel, Event Event, SIPMessage SIPMessage)
		{
			SIPMessageViewModel? sipMessageViewModel;
			int messageUID;

			messageUID = SIPUtils.GetMessageUID(Event.Message);
			sipMessageViewModel = FindMessageByUID(messageUID);

			if (sipMessageViewModel == null) return;
			

			sipMessageViewModel.RemoveSourceFile(FileViewModel);
			if (sipMessageViewModel.SourceFiles.Count == 0) SIPMessages.Remove(sipMessageViewModel);
			OnPropertiesChanged();
		}



	}
}
