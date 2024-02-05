using LogLib;
using SIP_o_matic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class DeviceViewModel : ViewModel<Device>
	{
		public string Name
		{
			get => Model.Name;
		}
		public IEnumerable<string> Addresses
		{
			get => Model.Addresses;
		}


		public DeviceViewModel(ILogger Logger) : base(Logger)
		{
		}
		

		

	}
}
