using LogLib;
using SIP_o_matic.DataSources;
using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
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
		public string? FromTag
		{
			get => SIPMessages.FirstOrDefault()?.FromTag;
		}
		public string? ToTag
		{
			get => SIPMessages.FirstOrDefault(item=>!string.IsNullOrEmpty(item.ToTag))?.ToTag;
		}
		public string? CSeq
		{
			get => SIPMessages.FirstOrDefault()?.CSeq;
		}
		public string? ViaBranch
		{
			get => SIPMessages.FirstOrDefault()?.ViaBranch;
		}

		public string? Display
		{
			get => SIPMessages.FirstOrDefault()?.Display;
		}

		public string? ShortDisplay
		{
			get => SIPMessages.FirstOrDefault()?.ShortDisplay;
		}

		public ObservableCollection<SIPMessageViewModel> SIPMessages
		{
			get;
			private set;
		}

		public Statuses Status
		{
			get;
			private set;
		}

		public bool HasRetransmissions
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
			OnPropertyChanged(nameof(ShortDisplay));
			OnPropertyChanged(nameof(FromTag));
			OnPropertyChanged(nameof(ToTag));
			OnPropertyChanged(nameof(CSeq));
			OnPropertyChanged(nameof(ViaBranch));
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
			sipMessageViewModel = null;// FindMessageByUID(messageUID);

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


		public void Analyze()
		{
			RequestViewModel[] requests;
			ResponseViewModel[] responses;


			requests = SIPMessages.OfType<RequestViewModel>().ToArray();
			responses = SIPMessages.OfType<ResponseViewModel>().ToArray();

			foreach(ResponseViewModel response in responses)
			{
				response.Analyze();
			}
			

			HasRetransmissions = requests.Length > 1;
			if (requests.Length == 0) this.Status = Statuses.Incomplete;
			else
			{
				if (responses.Length==0)
				{
					if (requests[0].Request.RequestLine.Method == "ACK") this.Status = Statuses.Success;
					else this.Status = Statuses.Incomplete;
				}
				else
				{
					this.Status = responses.Last().Status;
				}
			}
			OnPropertyChanged(nameof(Status));
		}




	}
}
