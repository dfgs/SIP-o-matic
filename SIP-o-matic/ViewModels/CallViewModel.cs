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

		public DateTime? StartTime
		{
			get => Dialogs.FirstOrDefault()?.StartTime;
		}

		public DateTime? StopTime
		{
			get => Dialogs.LastOrDefault()?.StopTime;
		}
		

		public string? From
		{
			get => Dialogs.FirstOrDefault()?.Transactions.FirstOrDefault()?.SIPMessages.FirstOrDefault()?.From;
		}
		public string? To
		{
			get => Dialogs.FirstOrDefault()?.Transactions.FirstOrDefault()?.SIPMessages.FirstOrDefault(item=>item.ToTag!=null)?.To;
		}


		public string? CallID
		{
			get => Dialogs.FirstOrDefault()?.Transactions.FirstOrDefault()?.SIPMessages.FirstOrDefault()?.CallID;
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

		protected void OnPropertiesChanged()
		{
			OnPropertyChanged(nameof(StartTime));
			OnPropertyChanged(nameof(StopTime));
			OnPropertyChanged(nameof(From));
			OnPropertyChanged(nameof(To));
			OnPropertyChanged(nameof(CallID));
		}


		public DialogViewModel? FindDialogByUID1(int UID1)
		{
			return Dialogs.FirstOrDefault(item => item.UID1 == UID1);
		}
		public DialogViewModel? FindDialogByUID2(int UID2)
		{
			return Dialogs.FirstOrDefault(item => item.UID2 == UID2);
		}

		public void AddSIPMessage(FileViewModel FileViewModel,Event Event, SIPMessage SIPMessage)
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

			
			dialogViewModel.AddSIPMessage(FileViewModel, Event, SIPMessage);
			OnPropertiesChanged();
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

			OnPropertiesChanged();


		}


	}
}
