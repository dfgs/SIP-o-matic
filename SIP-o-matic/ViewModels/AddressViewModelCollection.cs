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
	public class AddressViewModelCollection : ListViewModel<Address, AddressViewModel>
	{
		public AddressViewModelCollection(ILogger Logger) : base(Logger)
		{
		}

		protected override AddressViewModel OnCreateItem()
		{
			return new AddressViewModel(Logger);
		}

		public void Add(Address Address)
		{
			AddressViewModel addressViewModel;

			if (Model.Contains(Address)) return;
			if ((Address==null) || (string.IsNullOrEmpty(Address.Value))) return;
			
			Model.Add(Address);
			addressViewModel = new AddressViewModel(Logger);
			addressViewModel.Load(Address);
			AddInternal(addressViewModel);
		}

		public bool Contains(Address Address)
		{
			return Model.Contains(Address);
		}

		public void Remove(AddressViewModel Address)
		{
			Model.Remove(Address.GetModel());
			RemoveInternal(Address);
		}



	}
}
