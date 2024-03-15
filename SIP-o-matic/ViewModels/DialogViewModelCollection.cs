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


		public DialogViewModelCollection(IList<Dialog> Source, IDeviceNameProvider DeviceNameProvider) : base(Source, (SourceItem) => new DialogViewModel(SourceItem, DeviceNameProvider))
		{
		}

			

		public override int GetNewItemIndex(DialogViewModel Item)
		{
			for(int t=0;t<Count;t++)
			{
				if (Item.TimeStamp < this[t].TimeStamp) return t;
			}
			return Count ;
		}

		
	}
}
