using LogLib;
using PcapngFile;
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
	public class RTPStopViewModel : EventViewModel<RTPStop>, IEventViewModel
	{

		public string Description
		{
			get => $"RTP Stop (port: {DestinationPort})";
		}

		public string DialogColor
		{
			get => Model.DialogColor;
		}

		public Address SourceAddress
		{
			get=>Model.SourceAddress;
			set
			{
				Model.SourceAddress = value;
				OnPropertyChanged(nameof(SourceAddress));
				OnPropertyChanged(nameof(SourceDevice));
			}
		}

		public Address DestinationAddress
		{
			get=> Model.DestinationAddress;
			set
			{
				Model.DestinationAddress = value;
				OnPropertyChanged(nameof(DestinationAddress));
				OnPropertyChanged(nameof(DestinationDevice));
			}
		}

		public ushort DestinationPort
		{
			get => Model.DestinationPort;
		}
		

		public static readonly DependencyProperty IsFlippedProperty = DependencyProperty.Register("IsFlipped", typeof(bool), typeof(RTPStopViewModel), new PropertyMetadata(false));
		public bool IsFlipped
		{
			get { return (bool)GetValue(IsFlippedProperty); }
			set { SetValue(IsFlippedProperty, value); }
		}

	

		public RTPStopViewModel(RTPStop Model, IDeviceNameProvider DeviceNameProvider) : base(Model,DeviceNameProvider)
		{
           
		}

		

		

	}
}
