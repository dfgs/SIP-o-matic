using LogLib;
using SIP_o_matic.corelib.Models;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class AddressViewModel : ViewModel<Address>
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

		public AddressViewModel(ILogger Logger) : base(Logger)
		{
		}

		public Address GetModel()
		{
			return Model;
		}

	}
}