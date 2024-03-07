using LogLib;
using SIP_o_matic.corelib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class MessagesFrameViewModel : ViewModel<string>
	{

		public IEnumerable<string> Devices
		{
			get => Messages.SelectMany(message => message.Devices).Distinct();
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

	}
}
