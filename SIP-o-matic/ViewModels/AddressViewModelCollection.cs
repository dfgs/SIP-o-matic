using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class AddressViewModelCollection : ListViewModel<string, AddressViewModel>
	{
		public AddressViewModelCollection(ILogger Logger) : base(Logger)
		{
		}

		protected override AddressViewModel OnCreateItem()
		{
			return new AddressViewModel(Logger);
		}

		public void Add(string Address)
		{
			AddressViewModel addressViewModel;

			if (Model.Contains(Address)) return;
			
			
			Model.Add(Address);
			addressViewModel = new AddressViewModel(Logger);
			addressViewModel.Load(Address);
			AddInternal(addressViewModel);
		}


	}
}
