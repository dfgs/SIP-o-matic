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
	public class DeviceViewModel : ViewModel<Device>
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


		public DeviceViewModel(ILogger Logger) : base(Logger)
		{
			Addresses = new AddressViewModelCollection(Logger);
		}

		protected override void OnLoaded()
		{
			base.OnLoaded();
			Addresses.Load(Model.Addresses);
		}

		public Device GetModel()
		{
			return Model;
		}
		

	}
}
