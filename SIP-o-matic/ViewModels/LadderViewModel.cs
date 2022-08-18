using LogLib;
using SIP_o_matic.Views.TestData;
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
		}
		public LadderViewModel(ILogger Logger) : base(Logger)
		{
			Devices = new ObservableCollection<DeviceViewModel>();
			Events = new ObservableCollection<LadderEventViewModel>();
		}

		private DeviceViewModel FindDevice(IEnumerable<DeviceViewModel> ProjectDevices,string? Address)
		{
			if (Address != null)
			{
				foreach (DeviceViewModel device in ProjectDevices)
				{
					if (device.Addresses.Contains(Address)) return device;
				}
			}

			return new DeviceViewModel(Address??"Undefined");
		}
		private void AddDevice(DeviceViewModel Device)
		{
			if (!Devices.Contains(Device)) this.Devices.Add(Device);

		}
		#region create ladder events
		private DialogEventViewModel CreateLadderEvent(IEnumerable<DeviceViewModel> ProjectDevices, DialogViewModel Dialog)
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
				Dialog=Dialog
			};

			return dialogEvent;
		}
		private TransactionEventViewModel CreateLadderEvent(IEnumerable<DeviceViewModel> ProjectDevices, TransactionViewModel Transaction)
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
				Transaction=Transaction
			};

			return transactionEvent;
		}
		private SIPMessageEventViewModel CreateLadderEvent(IEnumerable<DeviceViewModel> ProjectDevices, SIPMessageViewModel SIPMessage)
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
				Message = SIPMessage
			};

			return SIPMessageEvent;
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

			Events.Clear();
			Devices = new ObservableCollection<DeviceViewModel>();

			foreach (CallViewModel call in Calls.Where(item=>item.IsSelected))
			{
				foreach(DialogViewModel dialog in call.Dialogs)
				{
					dialogEvent = CreateLadderEvent(ProjectDevices, dialog);
					AddEvent(dialogEvent);
				}
			}
		}


		#region ZoomIn
		private void ZoomIn(IEnumerable<DeviceViewModel> ProjectDevices, IEnumerable<CallViewModel> Calls,DialogEventViewModel DialogEvent)
		{
			TransactionEventViewModel transactionEvent;

			if (DialogEvent.Dialog == null) return;
			RemoveEvent(DialogEvent);

			foreach(TransactionViewModel transaction in DialogEvent.Dialog.Transactions)
			{
				transactionEvent = CreateLadderEvent(ProjectDevices, transaction);
				AddEvent(transactionEvent);
			}
		}

		private void ZoomIn(IEnumerable<DeviceViewModel> ProjectDevices, IEnumerable<CallViewModel> Calls, TransactionEventViewModel TransactionEvent)
		{
			SIPMessageEventViewModel messageEvent;

			if (TransactionEvent.Transaction == null) return;
			RemoveEvent(TransactionEvent);

			foreach (SIPMessageViewModel message in TransactionEvent.Transaction.SIPMessages)
			{
				messageEvent = CreateLadderEvent(ProjectDevices, message);
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
			DialogEventViewModel dialogEvent;
			DialogViewModel? dialog;
			TransactionEventViewModel? transactionEvent;

			if (TransactionEvent.Transaction == null) return;
			dialog = Calls.Where(call => call.IsSelected).SelectMany(call => call.Dialogs).FirstOrDefault(item => item.Transactions.Contains(TransactionEvent.Transaction));
			if (dialog == null) return;

			foreach(TransactionViewModel transaction in dialog.Transactions)
			{
				transactionEvent = Events.OfType<TransactionEventViewModel>().FirstOrDefault(item => item.Transaction == transaction);
				if (transactionEvent == null) continue;
				RemoveEvent(transactionEvent);
			}

			dialogEvent = CreateLadderEvent(ProjectDevices, dialog);
			AddEvent(dialogEvent);
		}
		private void ZoomOut(IEnumerable<DeviceViewModel> ProjectDevices, IEnumerable<CallViewModel> Calls, SIPMessageEventViewModel MessageEvent)
		{
			TransactionEventViewModel transactionEvent;
			TransactionViewModel? transaction;
			SIPMessageEventViewModel? messageEvent;

			if (MessageEvent.Message == null) return;
			transaction = Calls.Where(call => call.IsSelected).SelectMany(call => call.Dialogs).SelectMany(dialog=>dialog.Transactions).FirstOrDefault(item => item.SIPMessages.Contains(MessageEvent.Message));
			if (transaction == null) return;

			foreach (SIPMessageViewModel message in transaction.SIPMessages)
			{
				messageEvent = Events.OfType<SIPMessageEventViewModel>().FirstOrDefault(item => item.Message == message);
				if (messageEvent == null) continue;
				RemoveEvent(messageEvent);
			}

			transactionEvent = CreateLadderEvent(ProjectDevices, transaction);
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
