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
	public class MessagesFrameViewModel : ViewModel<string>
	{
	

		public static readonly DependencyProperty PinnedMessagesProperty = DependencyProperty.Register("PinnedMessages", typeof(ObservableCollection<MessageViewModel>), typeof(MessagesFrameViewModel), new PropertyMetadata(null));
		public ObservableCollection<MessageViewModel> PinnedMessages
		{
			get { return (ObservableCollection<MessageViewModel>)GetValue(PinnedMessagesProperty); }
			set { SetValue(PinnedMessagesProperty, value); }
		}




		public ObservableCollection<string> Devices
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

		public MessagesFrameViewModel(ILogger Logger, IDeviceNameProvider DeviceNameProvider) : base(Logger)
		{
			this.deviceNameProvider = DeviceNameProvider;
			Messages = new MessageViewModelCollection(Logger,deviceNameProvider);
			Devices = new ObservableCollection<string>();
			PinnedMessages = new ObservableCollection<MessageViewModel>();

		}
		protected override void OnLoaded()
		{
			base.OnLoaded();
			Messages.Load(new List<Message>());
		}
		public void Clear()
		{
			Messages.Clear();
		}

		

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
