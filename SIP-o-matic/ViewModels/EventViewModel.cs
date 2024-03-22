using SIP_o_matic.corelib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public abstract class EventViewModel<T> : GenericViewModel<T>,IEventViewModel
		where T:IEvent
	{
		IEvent IGenericViewModel<IEvent>.Model => Model;

		public DateTime Timestamp => Model.Timestamp;

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



		

		private IDeviceNameProvider deviceNameProvider;
		protected EventViewModel(T Model,IDeviceNameProvider DeviceNameProvider) : base(Model)
		{
			if (DeviceNameProvider == null) throw new ArgumentNullException(nameof(DeviceNameProvider));
			this.deviceNameProvider = DeviceNameProvider;
		}

	}
}
