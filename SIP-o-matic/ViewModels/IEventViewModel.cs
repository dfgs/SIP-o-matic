using SIP_o_matic.corelib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
    public interface IEventViewModel:IGenericViewModel<IEvent>
    {
		DateTime Timestamp
		{
			get;
		}
		IEnumerable<DeviceViewModel> Devices
		{
			get;
		}
		DeviceViewModel SourceDevice
		{
			get;
		}
		DeviceViewModel DestinationDevice
		{
			get;
		}

	}
}
