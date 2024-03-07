using LogLib;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class AddressViewModel : ViewModel<string>
	{
		public string Value
		{
			get => Model;
			
		}

		public AddressViewModel(ILogger Logger) : base(Logger)
		{
		}
	}
}