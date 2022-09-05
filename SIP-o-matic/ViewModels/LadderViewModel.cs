using LogLib;
using SIP_o_matic.Views;
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
	public class LadderViewModel : ViewModel
	{

		public static readonly DependencyProperty SelectedEventProperty = DependencyProperty.Register("SelectedEvent", typeof(LadderEventViewModel), typeof(LadderViewModel), new PropertyMetadata(null));
		public LadderEventViewModel SelectedEvent
		{
			get { return (LadderEventViewModel)GetValue(SelectedEventProperty); }
			set { SetValue(SelectedEventProperty, value); }
		}
		public static readonly DependencyProperty EventsProperty = DependencyProperty.Register("Events", typeof(ObservableCollection<LadderEventViewModel>), typeof(LadderViewModel), new PropertyMetadata(null));
		public ObservableCollection<LadderEventViewModel> Events
		{
			get { return (ObservableCollection<LadderEventViewModel>)GetValue(EventsProperty); }
			set { SetValue(EventsProperty, value); }
		}

		public static readonly DependencyProperty DevicesProperty = DependencyProperty.Register("Devices", typeof(ObservableCollection<DeviceViewModel>), typeof(LadderViewModel), new PropertyMetadata(null));
		public ObservableCollection<DeviceViewModel> Devices
		{
			get { return (ObservableCollection<DeviceViewModel>)GetValue(DevicesProperty); }
			set { SetValue(DevicesProperty, value); }
		}


		public static readonly DependencyProperty SessionEventsProperty = DependencyProperty.Register("SessionEvents", typeof(ObservableCollection<SessionEventViewModel>), typeof(LadderViewModel), new PropertyMetadata(null));
		public ObservableCollection<SessionEventViewModel> SessionEvents
		{
			get { return (ObservableCollection<SessionEventViewModel>)GetValue(SessionEventsProperty); }
			set { SetValue(SessionEventsProperty, value); }
		}

		public static readonly DependencyProperty SessionTimestampsProperty = DependencyProperty.Register("SessionTimestamps", typeof(ObservableCollection<TimestampViewModel>), typeof(LadderViewModel), new PropertyMetadata(null));
		public ObservableCollection<TimestampViewModel> SessionTimestamps
		{
			get { return (ObservableCollection<TimestampViewModel>)GetValue(SessionTimestampsProperty); }
			set { SetValue(SessionTimestampsProperty, value); }
		}

		public LadderViewModel() : base(NullLogger.Instance)
		{
			Devices=new ObservableCollection<DeviceViewModel>();
			Devices.Add(TestData.DeviceA);
			Devices.Add(TestData.DeviceB);
			Devices.Add(TestData.DeviceC);
			Devices.Add(TestData.DeviceD);
			Events = new ObservableCollection<LadderEventViewModel>();
			Events.Add(TestData.Event1);
			Events.Add(TestData.Event2);
			Events.Add(TestData.Event3);
			Events.Add(TestData.Event4);
			Events.Add(TestData.Event5);
			Events.Add(TestData.Event6);
			Events.Add(TestData.Event7);

			SessionTimestamps = new ObservableCollection<TimestampViewModel>();
			foreach (TimestampViewModel timestamp in TestData.SessionTimestamps)
			{
				SessionTimestamps.Add(timestamp);
			}

			SessionEvents = new ObservableCollection<SessionEventViewModel>();
			SessionEvents.Add(TestData.Session1);
			SessionEvents.Add(TestData.Session2);
			SessionEvents.Add(TestData.Session3);
			
			
		}
		public LadderViewModel(ILogger Logger) : base(Logger)
		{
			Devices = new ObservableCollection<DeviceViewModel>();
			Events = new ObservableCollection<LadderEventViewModel>();


			SessionTimestamps = new ObservableCollection<TimestampViewModel>();
			/*foreach (TimestampViewModel timestamp in TestData.SessionTimestamps)
			{
				SessionTimestamps.Add(timestamp);
			}//*/

			SessionEvents = new ObservableCollection<SessionEventViewModel>();
			//SessionEvents.Add(TestData.Session1);
			//SessionEvents.Add(TestData.Session2);
			//SessionEvents.Add(TestData.Session3);
		}

		private TimestampViewModel FindTimestamp(IEnumerable<TimestampViewModel> TimeStamps, DateTime Timestamp)
		{
			TimestampViewModel? newTimestamp;
			DateTime roundedDateTime;

			roundedDateTime= new DateTime(Timestamp.Year, Timestamp.Month, Timestamp.Day, Timestamp.Hour, Timestamp.Minute, Timestamp.Second);
			newTimestamp = TimeStamps.FirstOrDefault(item => item.Value == roundedDateTime);
			if (newTimestamp==null)
			{
				newTimestamp = new TimestampViewModel();
				newTimestamp.Value= roundedDateTime;
			}

			return newTimestamp;
		}
		private void AddTimestamp(TimestampViewModel Timestamp)
		{
			if (!SessionTimestamps.Contains(Timestamp)) this.SessionTimestamps.Add(Timestamp);
		}

		private DeviceViewModel FindDevice(IEnumerable<DeviceViewModel> ProjectDevices,string? Address)
		{
			DeviceViewModel newDevice;

			if (Address != null)
			{
				foreach (DeviceViewModel device in ProjectDevices)
				{
					if (device.Addresses.Contains(Address)) return device;
				}
			}

			newDevice = new DeviceViewModel(Address ?? "Undefined");
			newDevice.Addresses.Add(Address ?? "Undefined");
			return newDevice;
		}
		private void AddDevice(DeviceViewModel Device)
		{
			if (!Devices.Contains(Device)) this.Devices.Add(Device);
		}

		#region create ladder events
		private DialogEventViewModel CreateLadderEvent(IEnumerable<DeviceViewModel> ProjectDevices, DialogViewModel Dialog,string Color)
		{
			DeviceViewModel sourceDevice, destinationDevice;
			DialogEventViewModel dialogEvent;

			sourceDevice = FindDevice(ProjectDevices, Dialog.SourceAddress);
			destinationDevice = FindDevice(ProjectDevices, Dialog.DestinationAddress);

			AddDevice(sourceDevice);
			AddDevice(destinationDevice);

			dialogEvent = new DialogEventViewModel()
			{
				Timestamp = Dialog.StartTime ?? DateTime.MinValue,
				SourceDevice = sourceDevice,
				DestinationDevice = destinationDevice,
				Display = Dialog.Transactions.FirstOrDefault()?.ShortDisplay ?? "Undefined",
				EventColor = Color,
				Data = Dialog,
				Status = Dialog.Status,
			};

			return dialogEvent;
		}
		private TransactionEventViewModel CreateLadderEvent(IEnumerable<DeviceViewModel> ProjectDevices, TransactionViewModel Transaction, string Color)
		{
			DeviceViewModel sourceDevice, destinationDevice;
			TransactionEventViewModel transactionEvent;

			sourceDevice = FindDevice(ProjectDevices, Transaction.SourceAddress);
			destinationDevice = FindDevice(ProjectDevices, Transaction.DestinationAddress);

			AddDevice(sourceDevice);
			AddDevice(destinationDevice);

			transactionEvent = new TransactionEventViewModel()
			{
				Timestamp = Transaction.StartTime ?? DateTime.MinValue,
				SourceDevice = sourceDevice,
				DestinationDevice = destinationDevice,
				Display = Transaction.ShortDisplay ?? "Undefined",
				EventColor = Color,
				Data = Transaction,
				Status = Transaction.Status,
				HasRetransmissions=Transaction.HasRetransmissions,
				
			};

			return transactionEvent;
		}
		private SIPMessageEventViewModel CreateLadderEvent(IEnumerable<DeviceViewModel> ProjectDevices, SIPMessageViewModel SIPMessage,string Color)
		{
			DeviceViewModel sourceDevice, destinationDevice;
			SIPMessageEventViewModel SIPMessageEvent;

			sourceDevice = FindDevice(ProjectDevices, SIPMessage.SourceAddress);
			destinationDevice = FindDevice(ProjectDevices, SIPMessage.DestinationAddress);

			AddDevice(sourceDevice);
			AddDevice(destinationDevice);

			SIPMessageEvent = new SIPMessageEventViewModel()
			{
				Timestamp = SIPMessage.Timestamp,
				SourceDevice = sourceDevice,
				DestinationDevice = destinationDevice,
				Display = SIPMessage.ShortDisplay ?? "Undefined",
				EventColor = Color,
				Data = SIPMessage,
				HasBody = SIPMessage.HasBody,
			};

			switch(SIPMessage)
			{
				case ResponseViewModel response: SIPMessageEvent.Status = response.Status;break;
				default: SIPMessageEvent.Status = Statuses.Success; break;
			}

			return SIPMessageEvent;
		}

		private SessionEventViewModel CreateLadderEvent(IEnumerable<DeviceViewModel> ProjectDevices, SessionViewModel Session)
		{
			TimestampViewModel startTime, stopTime;
			SessionEventViewModel sessionEvent;

			startTime = FindTimestamp(SessionTimestamps, Session.StartTime);
			stopTime = FindTimestamp(SessionTimestamps, Session.StopTime);

			AddTimestamp(startTime);
			AddTimestamp(stopTime);

			sessionEvent = new SessionEventViewModel()
			{
				StartTime = startTime,
				StopTime=stopTime,
				SourceAddress=Session.SourceAddress,
				SourcePort=Session.SourcePort,
				DestinationAddress=Session.DestinationAddress,
				DestinationPort=Session.DestinationPort,
				Codec=Session.Codec,
			};


			return sessionEvent;
		}
		#endregion

		private void AddEvent(LadderEventViewModel Event)
		{
			for (int t=0;t<Events.Count;t++)
			{
				if (Events[t].Timestamp>Event.Timestamp)
				{
					Events.Insert(t, Event);
					return;
				}
			}
			Events.Add(Event);
		}
		private void RemoveEvent(LadderEventViewModel Event)
		{
			Events.Remove(Event);
		}

		public void Refresh(IEnumerable<DeviceViewModel> ProjectDevices,IEnumerable<CallViewModel> Calls)
		{
			DialogEventViewModel dialogEvent;
			TransactionEventViewModel transactionEvent;
			SessionEventViewModel sessionEvent;
			SIPMessageEventViewModel messageEvent;
			ColorManager colorManager;
			int dialogCount, transactionCount;


			Events.Clear();
			Devices.Clear();
			SessionTimestamps.Clear();
			SessionEvents.Clear();


			dialogCount = Calls.Where(item => item.IsSelected).SelectMany(call => call.Dialogs).Count();
			transactionCount = Calls.Where(item => item.IsSelected).SelectMany(call => call.Dialogs).SelectMany(dialog => dialog.Transactions).Count();

			colorManager = new ColorManager(dialogCount + transactionCount); ;

			foreach (CallViewModel call in Calls.Where(item=>item.IsSelected))
			{
				foreach(DialogViewModel dialog in call.Dialogs)
				{
					dialogEvent = CreateLadderEvent(ProjectDevices, dialog,colorManager.GetColorString());
					AddEvent(dialogEvent);

					foreach(TransactionViewModel transaction in dialog.Transactions)
					{
						transactionEvent = CreateLadderEvent(ProjectDevices, transaction, colorManager.GetColorString());
						dialogEvent.AddEvent(transactionEvent);
						foreach(SIPMessageViewModel message in transaction.SIPMessages)
						{
							messageEvent = CreateLadderEvent(ProjectDevices, message, transactionEvent.EventColor);
							transactionEvent.AddEvent(messageEvent);

						}
					}

					foreach(SessionViewModel session in dialog.Sessions)
					{
						sessionEvent = CreateLadderEvent(ProjectDevices, session);
						sessionEvent.DialogEvent = dialogEvent;
						SessionEvents.Add(sessionEvent);
					}


					dialogEvent.HasRetransmissions = dialogEvent.TransactionEvents.Any(item => item.HasRetransmissions);




				}
			}
		}


		#region ZoomIn
		private void ZoomIn(IEnumerable<DeviceViewModel> ProjectDevices, IEnumerable<CallViewModel> Calls,DialogEventViewModel DialogEvent)
		{
			RemoveEvent(DialogEvent);

			foreach(TransactionEventViewModel transactionEvent in DialogEvent.TransactionEvents)
			{
				AddEvent(transactionEvent);
			}
		}

		private void ZoomIn(IEnumerable<DeviceViewModel> ProjectDevices, IEnumerable<CallViewModel> Calls, TransactionEventViewModel TransactionEvent)
		{

			RemoveEvent(TransactionEvent);

			foreach (SIPMessageEventViewModel messageEvent in TransactionEvent.SIPMessageEvents)
			{
				AddEvent(messageEvent);
			}
		}

		public void ZoomIn(IEnumerable<DeviceViewModel> ProjectDevices, IEnumerable<CallViewModel> Calls)
		{
			if (SelectedEvent == null) return;
			switch(SelectedEvent)
			{
				case DialogEventViewModel dialogEvent:
					ZoomIn(ProjectDevices, Calls, dialogEvent);
					break;
				case TransactionEventViewModel transactionEvent:
					ZoomIn(ProjectDevices, Calls, transactionEvent);
					break;
				default:
					break;
			}
		}
		#endregion

		#region ZoomOut
		private void ZoomOut(IEnumerable<DeviceViewModel> ProjectDevices, IEnumerable<CallViewModel> Calls, TransactionEventViewModel TransactionEvent)
		{
			DialogEventViewModel? dialogEvent;

			dialogEvent = TransactionEvent.DialogEvent;
			if (dialogEvent == null) return;
			foreach(TransactionEventViewModel transactionEvent in dialogEvent.TransactionEvents)
			{
				RemoveEvent(transactionEvent);
			}

			AddEvent(dialogEvent);
		}
		private void ZoomOut(IEnumerable<DeviceViewModel> ProjectDevices, IEnumerable<CallViewModel> Calls, SIPMessageEventViewModel MessageEvent)
		{
			TransactionEventViewModel? transactionEvent;

			transactionEvent = MessageEvent.TransactionEvent;
			if (transactionEvent == null) return;
			foreach (SIPMessageEventViewModel messageEvent in transactionEvent.SIPMessageEvents)
			{
				RemoveEvent(messageEvent);
			}

			AddEvent(transactionEvent);
		}
		public void ZoomOut(IEnumerable<DeviceViewModel> ProjectDevices, IEnumerable<CallViewModel> Calls)
		{
			if (SelectedEvent == null) return;
			switch (SelectedEvent)
			{
				case TransactionEventViewModel transactionEvent:
					ZoomOut(ProjectDevices, Calls, transactionEvent);
					break;
				case SIPMessageEventViewModel messageEvent:
					ZoomOut(ProjectDevices, Calls, messageEvent);
					break;
				default:
					break;
			}
		}
		#endregion

	}
}
