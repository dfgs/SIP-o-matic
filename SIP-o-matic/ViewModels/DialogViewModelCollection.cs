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
	public class DialogViewModelCollection : GenericViewModelList<Dialog, DialogViewModel>
	{

		private IDeviceNameProvider deviceNameProvider;

		public DialogViewModelCollection(IList<Dialog> Source, IDeviceNameProvider DeviceNameProvider) : base(Source)
		{
			this.deviceNameProvider = DeviceNameProvider;
		}

		protected override DialogViewModel OnCreateItem(Dialog SourceItem)
		{
			return new DialogViewModel(SourceItem,deviceNameProvider);
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

			Source.Add(Dialog);

			dialogViewModel = new DialogViewModel(Dialog,deviceNameProvider);

			AddInternal(dialogViewModel);

		}
	}
}
