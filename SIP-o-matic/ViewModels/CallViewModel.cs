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
	public class CallViewModel:ViewModel
	{



		public static readonly DependencyProperty StartTimeProperty = DependencyProperty.Register("StartTime", typeof(DateTime), typeof(CallViewModel));
		public DateTime StartTime
		{
			get { return (DateTime)GetValue(StartTimeProperty); }
			set { SetValue(StartTimeProperty, value); }
		}
		public static readonly DependencyProperty StopTimeProperty = DependencyProperty.Register("StopTime", typeof(DateTime), typeof(CallViewModel));
		public DateTime StopTime
		{
			get { return (DateTime)GetValue(StopTimeProperty); }
			set { SetValue(StopTimeProperty, value); }
		}

		public static readonly DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(string), typeof(CallViewModel));
		public string From
		{
			get { return (string)GetValue(FromProperty); }
			set { SetValue(FromProperty, value); }
		}

		public static readonly DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(string), typeof(CallViewModel));
		public string To
		{
			get { return (string)GetValue(ToProperty); }
			set { SetValue(ToProperty, value); }
		}

		public static readonly DependencyProperty CallIDProperty = DependencyProperty.Register("CallID", typeof(string), typeof(CallViewModel));
		public string CallID
		{
			get { return (string)GetValue(CallIDProperty); }
			set { SetValue(CallIDProperty, value); }
		}


		public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(Statuses), typeof(CallViewModel));
		public Statuses Status
		{
			get { return (Statuses)GetValue(StatusProperty); }
			set { SetValue(StatusProperty, value); }
		}



		public static readonly DependencyProperty SelectedDialogProperty = DependencyProperty.Register("SelectedDialog", typeof(DialogViewModel), typeof(CallViewModel), new PropertyMetadata(null));
		public DialogViewModel SelectedDialog
		{
			get { return (DialogViewModel)GetValue(SelectedDialogProperty); }
			set { SetValue(SelectedDialogProperty, value); }
		}



		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(CallViewModel), new PropertyMetadata(false));
		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}


		


		public ObservableCollection<DialogViewModel> Dialogs
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
			this.UID = UID;
			Dialogs = new ObservableCollection<DialogViewModel>();
		}

		/*protected void OnPropertiesChanged()
		{
			OnPropertyChanged(nameof(StartTime));
			OnPropertyChanged(nameof(StopTime));
			OnPropertyChanged(nameof(From));
			OnPropertyChanged(nameof(To));
			OnPropertyChanged(nameof(CallID));
			OnPropertyChanged(nameof(Status));
		}*/


		public DialogViewModel? FindDialogByUID1(int UID1)
		{
			return Dialogs.FirstOrDefault(item => item.UID1 == UID1);
		}
		public DialogViewModel? FindDialogByUID2(int UID2)
		{
			return Dialogs.FirstOrDefault(item => item.UID2 == UID2);
		}

		public void AddSIPMessage(FileViewModel FileViewModel,Event Event, SIPMessage SIPMessage,SDP? SDP)
		{
			DialogViewModel? dialogViewModel;
			int dialogUIDSecondStage,dialogUIDFirstStage;



			dialogUIDSecondStage = SIPUtils.GetDialogUIDSecondStage(SIPMessage, Event.SourceAddress, Event.DestinationAddress);
			dialogViewModel = FindDialogByUID2(dialogUIDSecondStage);

			if (dialogViewModel==null)
			{
				dialogUIDFirstStage = SIPUtils.GetDialogUIDFirstStage(SIPMessage, Event.SourceAddress, Event.DestinationAddress);
				dialogViewModel = FindDialogByUID1(dialogUIDFirstStage);
				if (dialogViewModel == null)
				{
					dialogViewModel = new DialogViewModel(Logger, dialogUIDFirstStage);
					Dialogs.Add(dialogViewModel);
				}
				else
				{
					dialogViewModel.UpdateUID2(dialogUIDSecondStage);
				}
			}

			
			dialogViewModel.AddSIPMessage(FileViewModel, Event, SIPMessage,SDP);
			//OnPropertiesChanged();
		}
		public void RemoveSIPMessage(FileViewModel FileViewModel, Event Event, SIPMessage SIPMessage)
		{
			DialogViewModel? dialogViewModel;
			int dialogUIDSecondStage, dialogUIDFirstStage;

			dialogUIDSecondStage = SIPUtils.GetDialogUIDSecondStage(SIPMessage, Event.SourceAddress, Event.DestinationAddress);
			dialogViewModel = FindDialogByUID2(dialogUIDSecondStage);

			if (dialogViewModel == null)
			{
				dialogUIDFirstStage = SIPUtils.GetDialogUIDFirstStage(SIPMessage, Event.SourceAddress, Event.DestinationAddress);
				dialogViewModel = FindDialogByUID1(dialogUIDFirstStage);
				if (dialogViewModel == null) return;
			}

			dialogViewModel.RemoveSIPMessage(FileViewModel,Event, SIPMessage);
			if (dialogViewModel.Transactions.Count == 0) Dialogs.Remove(dialogViewModel);

			//OnPropertiesChanged();


		}

		public void Analyze()
		{
			foreach (DialogViewModel dialog in Dialogs)
			{
				dialog.Analyze();
			}

			StartTime = Dialogs.LastOrDefault()?.StartTime?? DateTime.MinValue;
			StopTime = Dialogs.LastOrDefault()?.StopTime?? DateTime.MaxValue;
			From = Dialogs.FirstOrDefault()?.Transactions.FirstOrDefault()?.SIPMessages.FirstOrDefault()?.From ?? "Undefined";
			To = Dialogs.FirstOrDefault()?.Transactions.FirstOrDefault()?.SIPMessages.FirstOrDefault()?.To ?? "Undefined";
			CallID = Dialogs.FirstOrDefault()?.Transactions.FirstOrDefault()?.SIPMessages.FirstOrDefault()?.CallID ?? "Undefined";

			Status = Dialogs.Max(item => item.Status);
		}


	}
}
