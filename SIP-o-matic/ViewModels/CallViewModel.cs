using LogLib;
using Microsoft.Office.Interop.PowerPoint;
using SIP_o_matic.corelib.Models;
using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class CallViewModel : GenericViewModel<Call>
	{
		public string CallID
		{
			get => Model.CallID;
		}

		public DeviceViewModel SourceDevice
		{
			get=>deviceNameProvider.GetDevice(Model.SourceDevice);
		}

		public DeviceViewModel DestinationDevice
		{
			get=> deviceNameProvider.GetDevice(Model.DestinationDevice);
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
		public Call.States State
		{
			get => Model.State;
		}
		public bool IsAck
		{
			get =>Model.IsAck;
		}

		public string LegName
		{
			get => Model.LegName;
		}

		public string LegDescription
		{
			get => Model.LegDescription;
		}

		public string Color
		{
			get => Model.Color;
		}

		public uint[] MessageIndices
		{
			get => Model.MessageIndices;
		}

		public string MessageIndicesDescription
		{
			get => string.Join(',', MessageIndices.Select(index => $"[{index}]"));
		}

		public string? ReplacedCallID
		{
			get => Model.ReplacedCallID;
		}

		public string StateDisplay
		{
			get
			{
				if ((IsAck) && (State==Call.States.Established)) return $"{State} (ACK)";
				else return State.ToString();
			}
		}

		public bool IsUpdated
		{
			get => Model.IsUpdated;
		}
			

		public static readonly DependencyProperty IsFlippedProperty = DependencyProperty.Register("IsFlipped", typeof(bool), typeof(CallViewModel), new PropertyMetadata(false));
		public bool IsFlipped
		{
			get { return (bool)GetValue(IsFlippedProperty); }
			set { SetValue(IsFlippedProperty, value); }
		}

		private IDeviceNameProvider deviceNameProvider;
		public CallViewModel(Call Model,IDeviceNameProvider DeviceNameProvider) : base(Model)
		{
			this.deviceNameProvider = DeviceNameProvider;
		}
	}
}
