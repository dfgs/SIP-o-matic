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
	public class CallViewModel:ViewModel
	{


		public static readonly DependencyProperty StartTimeProperty = DependencyProperty.Register("StartTime", typeof(DateTime), typeof(CallViewModel), new PropertyMetadata(DateTime.MaxValue));
		public DateTime StartTime
		{
			get { return (DateTime)GetValue(StartTimeProperty); }
			set { SetValue(StartTimeProperty, value); }
		}

		public static readonly DependencyProperty StopTimeProperty = DependencyProperty.Register("StopTime", typeof(DateTime), typeof(CallViewModel), new PropertyMetadata(DateTime.MinValue));
		public DateTime StopTime
		{
			get { return (DateTime)GetValue(StopTimeProperty); }
			set { SetValue(StopTimeProperty, value); }
		}



		public static readonly DependencyProperty SelectedDialogProperty = DependencyProperty.Register("SelectedDialog", typeof(DialogViewModel), typeof(CallViewModel), new PropertyMetadata(null));
		public DialogViewModel SelectedDialog
		{
			get { return (DialogViewModel)GetValue(SelectedDialogProperty); }
			set { SetValue(SelectedDialogProperty, value); }
		}




		public ObservableCollection<DialogViewModel> Dialogs
		{
			get;
			private set;
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


		public string CallID
		{
			get;
			private set;
		}

		public int UID
		{
			get;
			private set;
		}

		public CallViewModel(ILogger Logger, int UID):base(Logger)
		{
			From = "Undefined";To = "Undefined";CallID = "Undefined";
			this.UID = UID;
			Dialogs = new ObservableCollection<DialogViewModel>();
		}

		public DialogViewModel? FindDialogByUID(int UID)
		{
			return Dialogs.FirstOrDefault(item => item.UID == UID);
		}

		public void AddSIPMessage(FileViewModel FileViewModel,Event Event, SIPMessage SIPMessage)
		{
			DialogViewModel? dialogViewModel;
			int dialogUID;


			switch (SIPMessage)
			{
				case Request request:
					switch (request.RequestLine.Method)
					{
						case "INVITE":
							From = request.GetHeader<FromHeader>()?.Value.ToShortString() ?? "Undefined";
							To = request.GetHeader<ToHeader>()?.Value.ToShortString() ?? "Undefined";
							CallID = request.GetHeader<CallIDHeader>()?.Value ?? "Undefined";
							if (this.StartTime > Event.Timestamp) this.StartTime = Event.Timestamp;
							break;
						case "BYE":
							if (this.StopTime < Event.Timestamp) this.StopTime = Event.Timestamp;
							break;
					}
					break;
			}

			dialogUID = SIPUtils.GetDialogUID(SIPMessage, Event.SourceAddress, Event.DestinationAddress);
			dialogViewModel = FindDialogByUID(dialogUID);

			if (dialogViewModel == null)
			{
				dialogViewModel = new DialogViewModel(Logger, dialogUID);
				Dialogs.Add(dialogViewModel);
			}
			dialogViewModel.AddSIPMessage(FileViewModel, Event, SIPMessage);

		}



	}
}
