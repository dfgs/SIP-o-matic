using LogLib;
using SIP_o_matic.corelib;
using SIP_o_matic.corelib.Models;
using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewModelLib;
using Address = SIP_o_matic.corelib.Models.Address;

namespace SIP_o_matic.ViewModels
{
	public class DialogViewModel : GenericViewModel<Dialog>
	{



		public bool IsChecked
		{
			get => Model.IsChecked;
			set { Model.IsChecked = value;OnPropertyChanged(nameof(IsChecked)); }
		}




		public DateTime TimeStamp
		{
			get => Model.TimeStamp;
		}

		public string CallID
		{
			get => Model.CallID;
		}

		public Address SourceAddress
		{
			get => Model.SourceAddress;
			set
			{
				Model.SourceAddress = value;
				OnPropertyChanged(nameof(SourceAddress));
				OnPropertyChanged(nameof(SourceDevice));
			}
		}

		public Address DestinationAddress
		{
			get => Model.DestinationAddress;
			set
			{
				Model.DestinationAddress = value;
				OnPropertyChanged(nameof(DestinationAddress));
				OnPropertyChanged(nameof(DestinationDevice));
			}
		}

		public DeviceViewModel SourceDevice
		{
			get => deviceNameProvider.GetDevice(Model.SourceAddress);
		}
		public DeviceViewModel DestinationDevice
		{
			get => deviceNameProvider.GetDevice(Model.DestinationAddress);
		}

		public IEnumerable<DeviceViewModel> Devices
		{
			get
			{
				yield return SourceDevice;
				yield return DestinationDevice;
			}
		}
		public string Caller
		{
			get=>Model.Caller;
		}

		public string Callee
		{
			get => Model.Callee;
		}


		private IDeviceNameProvider deviceNameProvider;


		public DialogViewModel(Dialog Model, IDeviceNameProvider DeviceNameProvider) : base(Model)
		{
			if (DeviceNameProvider == null) throw new ArgumentNullException(nameof(DeviceNameProvider));
			this.deviceNameProvider = DeviceNameProvider;
			//this.deviceNameProvider.DeviceNameUpdated += DeviceNameProvider_DeviceNameUpdated;

		}

		/*private void DeviceNameProvider_DeviceNameUpdated(object? sender, EventArgs e)
		{
			OnPropertyChanged(nameof(SourceDevice));
			OnPropertyChanged(nameof(DestinationDevice));
		}*/

		



	}
}
