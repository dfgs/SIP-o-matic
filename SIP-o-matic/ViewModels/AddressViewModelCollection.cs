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
	public class AddressViewModelCollection : GenericViewModelList<Address, AddressViewModel>
	{
		public AddressViewModelCollection(IList<Address> Source) : base(Source ,(SourceItem) => new AddressViewModel(SourceItem))
		{
		}

		

		
		public override void Add(AddressViewModel Item)
		{
			if (!Contains(Item.GetModel()))	base.Add(Item);
		}

		public bool Contains(Address Address)
		{
			return Source.Contains(Address);
		}

		



	}
}
