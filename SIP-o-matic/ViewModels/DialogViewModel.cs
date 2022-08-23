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
		public string? ToTag
		{
			get => Transactions.FirstOrDefault()?.SIPMessages.FirstOrDefault(item=>!string.IsNullOrEmpty(item.ToTag))?.ToTag;
		}
		public ObservableCollection<TransactionViewModel> Transactions
		{
			get;
			private set;
		}
		public ObservableCollection<SessionViewModel> Sessions
		{
			get;
			private set;
		}
		public IEnumerable<string?> Devices
		{
			get
			{
				return Transactions.Select(item => item.SourceAddress).Union(Transactions.Select(item=>item.DestinationAddress));
			}
		}

		public static readonly DependencyProperty SelectedTransactionProperty = DependencyProperty.Register("SelectedTransaction", typeof(TransactionViewModel), typeof(DialogViewModel), new PropertyMetadata(null));
		public TransactionViewModel SelectedTransaction
		{
			get { return (TransactionViewModel)GetValue(SelectedTransactionProperty); }
			set { SetValue(SelectedTransactionProperty, value); }
		}

		public Statuses Status
		{
			get;
			private set;
		}


		public int UID1
		{
			get;
			private set;
		}
		public int UID2
		{
			get;
			private set;
		}

		private SessionViewModel? currentSession;

		public DialogViewModel(ILogger Logger, int UID1) : base(Logger)
		{
			this.UID1 = UID1;this.UID2 = 0;
			Transactions = new ObservableCollection<TransactionViewModel>();
			Sessions = new ObservableCollection<SessionViewModel>();
			currentSession = null;
		}
		public void UpdateUID2(int UID2)
		{
			this.UID2 = UID2;
		}

		protected void OnPropertiesChanged()
		{
			OnPropertyChanged(nameof(StartTime));
			OnPropertyChanged(nameof(StopTime));
			OnPropertyChanged(nameof(SourceAddress));
			OnPropertyChanged(nameof(DestinationAddress));
			OnPropertyChanged(nameof(FromTag));
			OnPropertyChanged(nameof(ToTag));
		}

		public TransactionViewModel? FindTransactionByUID(int UID)
		{
			return Transactions.FirstOrDefault(item => item.UID == UID);
		}


		private void ProcessMessageSDP(Event Event, Request Request,SDP? SDP)
		{
			switch (Request.RequestLine.Method)
			{
				case "INVITE":

					// reinvite
					if (currentSession != null)
					{
						currentSession.StopTime = Event.Timestamp;
					}
					currentSession = new SessionViewModel();
					if (SDP != null)
					{
						currentSession.SourceAddress = SDP.GetField<ConnectionField>()?.Address ?? "Undefined";
						currentSession.SourcePort = SDP.GetField<MediaField>()?.Port ?? 0;
					}

					break;
				case "BYE":

					if (currentSession != null)
					{
						currentSession.StopTime = Event.Timestamp;
					}
					currentSession = null;
					break;
			}

		}
		private void ProcessMessageSDP(Event Event, Response Response, SDP? SDP)
		{
			switch (Response.StatusLine.StatusCode)
			{
				case "200":
					if (currentSession != null)
					{
						currentSession.StartTime = Event.Timestamp;
						Sessions.Add(currentSession);
						if (SDP != null)
						{
							currentSession.DestinationAddress = SDP.GetField<ConnectionField>()?.Address ?? "Undefined";
							currentSession.DestinationPort = SDP.GetField<MediaField>()?.Port ?? 0;
						}
					}
					break;
			}
		}

		public void AddSIPMessage(FileViewModel FileViewModel, Event Event, SIPMessage SIPMessage, SDP? SDP)
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

			transactionViewModel.AddSIPMessage(FileViewModel, Event, SIPMessage,SDP);
			switch(SIPMessage)
			{
				case Request request:
					ProcessMessageSDP(Event, request,SDP);
					break;
				case Response response:
					ProcessMessageSDP(Event, response,  SDP);
					break;
			}

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


		public void Analyze()
		{
			foreach(TransactionViewModel transaction in Transactions)
			{
				transaction.Analyze();
			}
			this.Status = Transactions.Max(item => item.Status);
			OnPropertyChanged(nameof(Status));
		}

	}
}
