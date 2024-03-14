using LogLib;
using SIP_o_matic.corelib.Models;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class AddressViewModel : GenericViewModel<Address>
	{
		public string Value
		{
			get => Model.Value;
			set
			{
				Model.Value = value;
				OnPropertyChanged(nameof(Value));
			}
			
		}

		public AddressViewModel(Address Address) : base(Address)
		{
		}

		public Address GetModel()
		{
			return Model;
		}

	}
}