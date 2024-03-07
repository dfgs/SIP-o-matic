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
	public class DialogViewModel : ViewModel<Dialog>,ISIPMessageMatch
	{



		public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(DialogViewModel), new PropertyMetadata(false));
		public bool IsChecked
		{
			get { return (bool)GetValue(IsCheckedProperty); }
			set { SetValue(IsCheckedProperty, value); }
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

		public string SourceDevice
		{
			get => deviceNameProvider.GetDeviceName(Model.SourceAddress);
		}
		public string DestinationDevice
		{
			get => deviceNameProvider.GetDeviceName(Model.DestinationAddress);
		}

		public IEnumerable<string> Devices
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


		public DialogViewModel(ILogger Logger, IDeviceNameProvider DeviceNameProvider) : base(Logger)
		{
			if (DeviceNameProvider == null) throw new ArgumentNullException(nameof(DeviceNameProvider));
			this.deviceNameProvider = DeviceNameProvider;
			this.deviceNameProvider.DeviceNameUpdated += DeviceNameProvider_DeviceNameUpdated;

		}

		private void DeviceNameProvider_DeviceNameUpdated(object? sender, EventArgs e)
		{
			OnPropertyChanged(nameof(SourceDevice));
			OnPropertyChanged(nameof(DestinationDevice));
		}

		public bool Match(ISIPMessage MessageInfo)
		{
			return Model.Match(MessageInfo);
		}



	}
}
