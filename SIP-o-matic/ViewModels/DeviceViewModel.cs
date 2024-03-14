using LogLib;
using SIP_o_matic.corelib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class DeviceViewModel : GenericViewModel<Device>
	{
		public string Name
		{
			get => Model.Name;
			set 
			{ 
				Model.Name = value; 
				OnPropertyChanged(nameof(Name)); 
			}
		}
		public AddressViewModelCollection Addresses
		{
			get;
			private set;
		}


		public DeviceViewModel(Device Model) : base(Model)
		{
			Addresses = new AddressViewModelCollection(Model.Addresses);
		}

		

		public Device GetModel()
		{
			return Model;
		}
		

	}
}
