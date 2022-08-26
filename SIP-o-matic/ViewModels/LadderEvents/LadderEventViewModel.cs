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

		public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(object), typeof(LadderEventViewModel), new PropertyMetadata(null));
		public object Data
		{
			get { return GetValue(DataProperty); }
			set { SetValue(DataProperty, value); }
		}

		public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(Statuses), typeof(LadderEventViewModel), new PropertyMetadata(Statuses.Undefined));
		public Statuses Status
		{
			get { return (Statuses)GetValue(StatusProperty); }
			set { SetValue(StatusProperty, value); }
		}

		public static readonly DependencyProperty HasRetransmissionsProperty = DependencyProperty.Register("HasRetransmissions", typeof(bool), typeof(LadderEventViewModel), new PropertyMetadata(false));
		public bool HasRetransmissions
		{
			get { return (bool)GetValue(HasRetransmissionsProperty); }
			set { SetValue(HasRetransmissionsProperty, value); }
		}

		public LadderEventViewModel() : base(NullLogger.Instance)
		{
		}

	}
}
