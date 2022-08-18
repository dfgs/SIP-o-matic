using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SIP_o_matic.ViewModels
{
	public abstract class LadderEventViewModel : ViewModel
	{
		public static readonly DependencyProperty TimestampProperty = DependencyProperty.Register("Timestamp", typeof(DateTime), typeof(LadderEventViewModel), new PropertyMetadata(null));
		public DateTime Timestamp
		{
			get { return (DateTime)GetValue(TimestampProperty); }
			set { SetValue(TimestampProperty, value); }
		}


		public static readonly DependencyProperty SourceDeviceProperty = DependencyProperty.Register("SourceDevice", typeof(DeviceViewModel), typeof(LadderEventViewModel), new PropertyMetadata(null));
		public DeviceViewModel SourceDevice
		{
			get { return (DeviceViewModel)GetValue(SourceDeviceProperty); }
			set { SetValue(SourceDeviceProperty, value); }
		}

		public static readonly DependencyProperty DestinationDeviceProperty = DependencyProperty.Register("DestinationDevice", typeof(DeviceViewModel), typeof(LadderEventViewModel), new PropertyMetadata(null));
		public DeviceViewModel DestinationDevice
		{
			get { return (DeviceViewModel)GetValue(DestinationDeviceProperty); }
			set { SetValue(DestinationDeviceProperty, value); }
		}

		public abstract string Display
		{
			get;
			set;
		}
		public abstract string BorderColor
		{
			get;
		}
		public abstract string EventColor
		{
			get;
			set;
		}

		public LadderEventViewModel() : base(NullLogger.Instance)
		{
		}

	}
}
