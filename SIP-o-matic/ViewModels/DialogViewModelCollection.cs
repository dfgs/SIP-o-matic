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
	public class DialogViewModelCollection : ListViewModel<Dialog, DialogViewModel>
	{

		private IDeviceNameProvider deviceNameProvider;

		public DialogViewModelCollection(ILogger Logger, IDeviceNameProvider DeviceNameProvider) : base(Logger)
		{
			this.deviceNameProvider = DeviceNameProvider;
		}

		protected override DialogViewModel OnCreateItem()
		{
			return new DialogViewModel(Logger,deviceNameProvider);
		}

		public bool ContainsDialogForMessage(MessageViewModel Message)
		{
			return this.FirstOrDefault(item => item.Match(Message.SIPMessage)) != null;
		}
		public bool ContainsCheckedDialogForMessage(MessageViewModel Message)
		{
			DialogViewModel? item;

			item=this.FirstOrDefault(item => item.Match(Message.SIPMessage));
			if (item == null) return false;
			return item.IsChecked;
		}

		public override int GetNewItemIndex(DialogViewModel Item)
		{
			for(int t=0;t<Count;t++)
			{
				if (Item.TimeStamp < this[t].TimeStamp) return t;
			}
			return Count ;
		}

		public void Add(Dialog Dialog)
		{
			DialogViewModel dialogViewModel;

			Model.Add(Dialog);

			dialogViewModel = new DialogViewModel(Logger,deviceNameProvider);
			dialogViewModel.Load(Dialog);

			AddInternal(dialogViewModel);

		}
	}
}
