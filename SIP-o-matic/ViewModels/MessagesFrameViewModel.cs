using LogLib;
using SIP_o_matic.corelib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class MessagesFrameViewModel : GenericViewModel<MessagesFrame>
	{
	

		public static readonly DependencyProperty PinnedMessagesProperty = DependencyProperty.Register("PinnedMessages", typeof(ObservableCollection<MessageViewModel>), typeof(MessagesFrameViewModel), new PropertyMetadata(null));
		public ObservableCollection<MessageViewModel> PinnedMessages
		{
			get { return (ObservableCollection<MessageViewModel>)GetValue(PinnedMessagesProperty); }
			set { SetValue(PinnedMessagesProperty, value); }
		}


				


		public ObservableCollection<DeviceViewModel> Devices
		{
			get ;
			private set;
		}

		public MessageViewModelCollection Messages
		{
			get;
			private set;
		}

		private IDeviceNameProvider deviceNameProvider;

		public MessagesFrameViewModel(MessagesFrame Model, IDeviceNameProvider DeviceNameProvider) : base(Model)
		{
			this.deviceNameProvider = DeviceNameProvider;
			//this.deviceNameProvider.DeviceNameUpdated += DeviceNameProvider_DeviceNameUpdated;
			Messages = new MessageViewModelCollection(Model.Messages,deviceNameProvider);
			Devices = new ObservableCollection<DeviceViewModel>(Messages.SelectMany(message=>message.Devices).Distinct());
			PinnedMessages = new ObservableCollection<MessageViewModel>();

		}

		/*private void DeviceNameProvider_DeviceNameUpdated(object? sender, EventArgs e)
		{
			Devices = new ObservableCollection<Device>(Model.Devices);
		}*/

		public void PinMessage(MessageViewModel Message)
		{
			if (PinnedMessages.Contains(Message))
			{
				PinnedMessages.Remove(Message);
			}
			else
			{
				PinnedMessages.Add(Message);
			}
		}

	}
}
