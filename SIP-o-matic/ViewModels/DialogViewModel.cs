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

		public DateTime? StartTime
		{
			get => Transactions.FirstOrDefault()?.StartTime;
		}

		public DateTime? StopTime
		{
			get => Transactions.LastOrDefault()?.StopTime;
		}

		public string? SourceAddress
		{
			get => Transactions.FirstOrDefault()?.SourceAddress;
		}

		public string? DestinationAddress
		{
			get => Transactions.FirstOrDefault()?.DestinationAddress;
		}

		public string? FromTag
		{
			get => Transactions.FirstOrDefault()?.SIPMessages.FirstOrDefault()?.FromTag;
		}

		public ObservableCollection<TransactionViewModel> Transactions
		{
			get;
			private set;
		}
		public static readonly DependencyProperty SelectedTransactionProperty = DependencyProperty.Register("SelectedTransaction", typeof(TransactionViewModel), typeof(DialogViewModel), new PropertyMetadata(null));
		public TransactionViewModel SelectedTransaction
		{
			get { return (TransactionViewModel)GetValue(SelectedTransactionProperty); }
			set { SetValue(SelectedTransactionProperty, value); }
		}


		public int UID
		{
			get;
			private set;
		}

		public DialogViewModel(ILogger Logger, int UID) : base(Logger)
		{
			this.UID = UID;
			Transactions = new ObservableCollection<TransactionViewModel>();
		}

		protected void OnPropertiesChanged()
		{
			OnPropertyChanged(nameof(StartTime));
			OnPropertyChanged(nameof(StopTime));
			OnPropertyChanged(nameof(SourceAddress));
			OnPropertyChanged(nameof(DestinationAddress));
			OnPropertyChanged(nameof(FromTag));
		}

		public TransactionViewModel? FindTransactionByUID(int UID)
		{
			return Transactions.FirstOrDefault(item => item.UID == UID);
		}

		public void AddSIPMessage(FileViewModel FileViewModel, Event Event, SIPMessage SIPMessage)
		{
			TransactionViewModel? transactionViewModel;
			int transactionUID;

			transactionUID = SIPUtils.GetTransactionUID(SIPMessage);
			transactionViewModel = FindTransactionByUID(transactionUID);

			if (transactionViewModel == null)
			{
				transactionViewModel = new TransactionViewModel(Logger, transactionUID);
				Transactions.Add(transactionViewModel);
			}

			transactionViewModel.AddSIPMessage(FileViewModel, Event, SIPMessage);
			OnPropertiesChanged();
		}
		public void RemoveSIPMessage(FileViewModel FileViewModel, Event Event, SIPMessage SIPMessage)
		{
			TransactionViewModel? transactionViewModel;
			int transactionUID;

			transactionUID = SIPUtils.GetTransactionUID(SIPMessage);
			transactionViewModel = FindTransactionByUID(transactionUID);

			if (transactionViewModel == null) return;
			transactionViewModel.RemoveSIPMessage(FileViewModel, Event, SIPMessage);
			if (transactionViewModel.SIPMessages.Count == 0) Transactions.Remove(transactionViewModel);
			OnPropertiesChanged();

		}



	}
}
