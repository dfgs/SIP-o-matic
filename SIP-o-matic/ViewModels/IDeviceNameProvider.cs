using SIP_o_matic.corelib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.ViewModels
{
	public interface IDeviceNameProvider
	{
		//public event EventHandler DeviceNameUpdated;

		public DeviceViewModel GetDevice(Address Address);
		public DeviceViewModel GetDevice(Device Model);
		
	}
}
