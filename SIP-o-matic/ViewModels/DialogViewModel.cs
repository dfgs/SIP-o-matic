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

		public static readonly DependencyProperty StartTimeProperty = DependencyProperty.Register("StartTime", typeof(DateTime), typeof(DialogViewModel));
		public DateTime StartTime
		{
			get { return (DateTime)GetValue(StartTimeProperty); }
			set { SetValue(StartTimeProperty, value); }
		}

		public static readonly DependencyProperty StopTimeProperty = DependencyProperty.Register("StopTime", typeof(DateTime), typeof(DialogViewModel));
		public DateTime StopTime
		{
			get { return (DateTime)GetValue(StopTimeProperty); }
			set { SetValue(StopTimeProperty, value); }
		}

		public static readonly DependencyProperty SourceAddressProperty = DependencyProperty.Register("SourceAddress", typeof(string), typeof(DialogViewModel));
		public string SourceAddress
		{
			get { return (string)GetValue(SourceAddressProperty); }
			set { SetValue(SourceAddressProperty, value); }
		}

		public static readonly DependencyProperty DestinationAddressProperty = DependencyProperty.Register("DestinationAddress", typeof(string), typeof(DialogViewModel));
		public string DestinationAddress
		{
			get { return (string)GetValue(DestinationAddressProperty); }
			set { SetValue(DestinationAddressProperty, value); }
		}

		public static readonly DependencyProperty FromTagProperty = DependencyProperty.Register("FromTag", typeof(string), typeof(DialogViewModel));
		public string FromTag
		{
			get { return (string)GetValue(FromTagProperty); }
			set { SetValue(FromTagProperty, value); }
		}

		public static readonly DependencyProperty ToTagProperty = DependencyProperty.Register("ToTag", typeof(string), typeof(DialogViewModel));
		public string ToTag
		{
			get { return (string)GetValue(ToTagProperty); }
			set { SetValue(ToTagProperty, value); }
		}

		public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(Statuses), typeof(DialogViewModel));
		public Statuses Status
		{
			get { return (Statuses)GetValue(StatusProperty); }
			set { SetValue(StatusProperty, value); }
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


		public DialogViewModel(ILogger Logger, int UID1) : base(Logger)
		{
			this.UID1 = UID1;this.UID2 = 0;
			Transactions = new ObservableCollection<TransactionViewModel>();
			Sessions = new ObservableCollection<SessionViewModel>();
			//currentSession = null;previousSession = null;
		}
		public void UpdateUID2(int UID2)
		{
			this.UID2 = UID2;
		}

		

		public TransactionViewModel? FindTransactionByUID(int UID)
		{
			return Transactions.FirstOrDefault(item => item.UID == UID);
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
			

			//OnPropertiesChanged();
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
			//OnPropertiesChanged();

		}


		public void Analyze()
		{
			SessionViewModel? currentSession=null,nextSession=null;

			
			Sessions.Clear();

			foreach(TransactionViewModel transaction in Transactions)
			{
				
				transaction.Analyze();

				switch(transaction.SessionTrigger)
				{
					case SetupSessionTrigger setupSessionTrigger:
						nextSession = new SessionViewModel()
						{
							SourceAddress=setupSessionTrigger.SourceAddress,
							SourcePort=setupSessionTrigger.SourcePort,
							DestinationAddress=setupSessionTrigger.DestinationAddress,
							DestinationPort=setupSessionTrigger.DestinationPort,
							Codec=setupSessionTrigger.Codec,
							SetupTransaction=transaction,
						};
						break;
					case StartSessionTrigger startSessionTrigger:
						if (currentSession != null) currentSession.StopTime = startSessionTrigger.Timestamp;
						if (nextSession!= null) 
						{
							currentSession = nextSession;
							nextSession = null;
							currentSession.StartTime = startSessionTrigger.Timestamp;
							Sessions.Add(currentSession);
						}
						break;
					case StopSessionTrigger stopSessionTrigger:
						if (currentSession != null) 
						{
							currentSession.StopTime = stopSessionTrigger.Timestamp;
						}
						currentSession = null;
						break;

				}
				
			}
			StartTime = Transactions.FirstOrDefault()?.StartTime??DateTime.MinValue;
			StopTime = Transactions.FirstOrDefault()?.StopTime ?? DateTime.MaxValue;
			SourceAddress= Transactions.FirstOrDefault()?.SourceAddress??"Undefined";
			DestinationAddress = Transactions.FirstOrDefault()?.DestinationAddress ?? "Undefined";
			FromTag= Transactions.FirstOrDefault()?.SIPMessages.FirstOrDefault()?.FromTag ?? "Undefined";
			ToTag= Transactions.FirstOrDefault()?.SIPMessages.FirstOrDefault(item => !string.IsNullOrEmpty(item.ToTag))?.ToTag ?? "Undefined";

			Status = Transactions.Max(item => item.Status);
		}

	}
}
