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
	public class DialogViewModel:ViewModel
	{

		public static readonly DependencyProperty StartTimeProperty = DependencyProperty.Register("StartTime", typeof(DateTime), typeof(DialogViewModel), new PropertyMetadata(DateTime.MaxValue));
		public DateTime StartTime
		{
			get { return (DateTime)GetValue(StartTimeProperty); }
			set { SetValue(StartTimeProperty, value); }
		}

		public static readonly DependencyProperty StopTimeProperty = DependencyProperty.Register("StopTime", typeof(DateTime), typeof(DialogViewModel), new PropertyMetadata(DateTime.MinValue));
		public DateTime StopTime
		{
			get { return (DateTime)GetValue(StopTimeProperty); }
			set { SetValue(StopTimeProperty, value); }
		}

		public string SourceAddress
		{
			get;
			private set;
		}

		public string DestinationAddress
		{
			get;
			private set;
		}
		
		public string FromTag
		{
			get;
			private set;
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

		public DialogViewModel(ILogger Logger, int UID) : base(Logger)
		{
			SourceAddress = "Undefined"; DestinationAddress = "Undefined";
			this.UID = UID;FromTag = "Undefined";
			SIPMessages = new ObservableCollection<SIPMessageViewModel>();
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
						if (this.StartTime > Event.Timestamp) this.StartTime = Event.Timestamp;
						break;
					case Request request:
						sipMessageViewModel = new RequestViewModel(Logger, Event, request);
						if (this.StopTime < Event.Timestamp) this.StopTime = Event.Timestamp;
						switch (request.RequestLine.Method)
						{
							case "INVITE":
								SourceAddress = Event.SourceAddress ?? "Undefined";
								DestinationAddress = Event.DestinationAddress?? "Undefined";
								FromTag = request.GetHeader<FromHeader>()?.Value.Tag ?? "Undefined";
								break;
						}

						break;
					default:
						return;
				}



				SIPMessages.Add(sipMessageViewModel);
			}

			sipMessageViewModel.AddSourceFile(FileViewModel);

		}

	}
}
