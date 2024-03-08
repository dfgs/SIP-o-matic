using LogLib;
using SIP_o_matic.corelib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class MessageViewModelCollection : ListViewModel<Message, MessageViewModel>
	{

		private IDeviceNameProvider deviceNameProvider;

		

		public MessageViewModelCollection(ILogger Logger, IDeviceNameProvider DeviceNameProvider) : base(Logger)
		{
			this.deviceNameProvider = DeviceNameProvider;
		}


		protected override MessageViewModel OnCreateItem()
		{
			return new MessageViewModel(Logger,deviceNameProvider);
		}
		public void Clear()
		{
			Model.Clear();
			ClearInternal();
		}

		public void Add(Message Message)
		{
			MessageViewModel messageViewModel;

			Model.Add(Message);

			messageViewModel=new MessageViewModel(Logger, deviceNameProvider);
			messageViewModel.Load(Message);
			
			AddInternal(messageViewModel);

		}

		public void Add(MessageViewModel Message)
		{
			AddInternal(Message);
		}

	



	}
}
