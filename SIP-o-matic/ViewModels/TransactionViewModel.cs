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


		public static readonly DependencyProperty StartTimeProperty = DependencyProperty.Register("StartTime", typeof(DateTime), typeof(TransactionViewModel));
		public DateTime StartTime
		{
			get { return (DateTime)GetValue(StartTimeProperty); }
			set { SetValue(StartTimeProperty, value); }
		}

		public static readonly DependencyProperty StopTimeProperty = DependencyProperty.Register("StopTime", typeof(DateTime), typeof(TransactionViewModel));
		public DateTime StopTime
		{
			get { return (DateTime)GetValue(StopTimeProperty); }
			set { SetValue(StopTimeProperty, value); }
		}

		public static readonly DependencyProperty SourceAddressProperty = DependencyProperty.Register("SourceAddress", typeof(string), typeof(TransactionViewModel));
		public string SourceAddress
		{
			get { return (string)GetValue(SourceAddressProperty); }
			set { SetValue(SourceAddressProperty, value); }
		}

		public static readonly DependencyProperty DestinationAddressProperty = DependencyProperty.Register("DestinationAddress", typeof(string), typeof(TransactionViewModel));
		public string DestinationAddress
		{
			get { return (string)GetValue(DestinationAddressProperty); }
			set { SetValue(DestinationAddressProperty, value); }
		}

		public static readonly DependencyProperty FromTagProperty = DependencyProperty.Register("FromTag", typeof(string), typeof(TransactionViewModel));
		public string FromTag
		{
			get { return (string)GetValue(FromTagProperty); }
			set { SetValue(FromTagProperty, value); }
		}

		public static readonly DependencyProperty ToTagProperty = DependencyProperty.Register("ToTag", typeof(string), typeof(TransactionViewModel));
		public string ToTag
		{
			get { return (string)GetValue(ToTagProperty); }
			set { SetValue(ToTagProperty, value); }
		}

		public static readonly DependencyProperty CSeqProperty = DependencyProperty.Register("CSeq", typeof(string), typeof(TransactionViewModel));
		public string CSeq
		{
			get { return (string)GetValue(CSeqProperty); }
			set { SetValue(CSeqProperty, value); }
		}

		public static readonly DependencyProperty ViaBranchProperty = DependencyProperty.Register("ViaBranch", typeof(string), typeof(TransactionViewModel));
		public string ViaBranch
		{
			get { return (string)GetValue(ViaBranchProperty); }
			set { SetValue(ViaBranchProperty, value); }
		}

		public static readonly DependencyProperty DisplayProperty = DependencyProperty.Register("Display", typeof(string), typeof(TransactionViewModel));
		public string Display
		{
			get { return (string)GetValue(DisplayProperty); }
			set { SetValue(DisplayProperty, value); }
		}

		public static readonly DependencyProperty ShortDisplayProperty = DependencyProperty.Register("ShortDisplay", typeof(string), typeof(TransactionViewModel));
		public string ShortDisplay
		{
			get { return (string)GetValue(ShortDisplayProperty); }
			set { SetValue(ShortDisplayProperty, value); }
		}

		public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(Statuses), typeof(TransactionViewModel));
		public Statuses Status
		{
			get { return (Statuses)GetValue(StatusProperty); }
			set { SetValue(StatusProperty, value); }
		}

		public ObservableCollection<SIPMessageViewModel> SIPMessages
		{
			get;
			private set;
		}

	
		public SessionTrigger? SessionTrigger
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



		/*protected void OnPropertiesChanged()
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
		}*/

		public SIPMessageViewModel? FindMessageByUID(int UID)
		{
			return SIPMessages.FirstOrDefault(item => item.UID == UID);
		}
			

		public void AddSIPMessage(FileViewModel FileViewModel, Event Event, SIPMessage SIPMessage, SDP? SDP)
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
						sipMessageViewModel = new ResponseViewModel(Logger, Event, response,SDP);
						break;
					case Request request:
						sipMessageViewModel = new RequestViewModel(Logger, Event, request, SDP);
						break;
					default:
						return;
				}

				SIPMessages.Add(sipMessageViewModel);
			}

			sipMessageViewModel.AddSourceFile(FileViewModel);
			//OnPropertiesChanged();
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
			//OnPropertiesChanged();
		}


		public void Analyze()
		{
			RequestViewModel[] requests;
			ResponseViewModel[] responses;
			RequestViewModel? inviteRequest, byeRequest,ackRequest;
			ResponseViewModel? okResponse;

			requests = SIPMessages.OfType<RequestViewModel>().ToArray();
			responses = SIPMessages.OfType<ResponseViewModel>().ToArray();

			foreach(ResponseViewModel response in responses)
			{
				response.Analyze();
			}

			StartTime = SIPMessages.FirstOrDefault()?.Timestamp ?? DateTime.MinValue;
			StopTime = SIPMessages.LastOrDefault()?.Timestamp ?? DateTime.MaxValue;
			SourceAddress = SIPMessages.FirstOrDefault()?.SourceAddress ?? "Undefined";
			DestinationAddress = SIPMessages.FirstOrDefault()?.DestinationAddress ?? "Undefined";
			FromTag = SIPMessages.FirstOrDefault()?.FromTag ?? "Undefined";
			ToTag = SIPMessages.FirstOrDefault()?.ToTag ?? "Undefined";
			CSeq = SIPMessages.FirstOrDefault()?.CSeq ?? "Undefined";
			ViaBranch = SIPMessages.FirstOrDefault()?.ViaBranch ?? "Undefined";
			Display = SIPMessages.FirstOrDefault()?.Display ?? "Undefined";
			ShortDisplay = SIPMessages.FirstOrDefault()?.ShortDisplay ?? "Undefined";


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

			if (Status==Statuses.Success)
			{
				inviteRequest = requests.LastOrDefault(item => item.HasBody && item.Request.RequestLine.Method == "INVITE");
				byeRequest = requests.LastOrDefault(item => item.Request.RequestLine.Method == "BYE");
				ackRequest = requests.LastOrDefault(item => item.Request.RequestLine.Method == "ACK");
				okResponse = responses.LastOrDefault(item => item.Response.StatusLine.StatusCode == "200");
				
				if (ackRequest!=null)
				{
					SessionTrigger = new StartSessionTrigger(this.StartTime);
				}
				else if ((inviteRequest!=null) && (okResponse!=null))
				{
					SessionTrigger = new SetupSessionTrigger(this.StartTime, 
						inviteRequest.SDP?.GetField<ConnectionField>()?.Address ?? "Undefined", inviteRequest.SDP?.GetField<MediaField>()?.Port??0,
						okResponse.SDP?.GetField<ConnectionField>()?.Address ?? "Undefined", okResponse.SDP?.GetField<MediaField>()?.Port ?? 0,
						okResponse.SDP?.GetCodec()??"Undefined");
				} 
				else if ((byeRequest != null)  && (okResponse != null))
				{
					SessionTrigger = new StopSessionTrigger(this.StartTime);
				}

			}



			//OnPropertyChanged(nameof(Status));
		}




	}
}
