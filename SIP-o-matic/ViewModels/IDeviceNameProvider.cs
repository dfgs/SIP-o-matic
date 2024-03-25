using SIP_o_matic.corelib.Models;
using SIPParserLib;
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

		public DeviceViewModel GetDevice(corelib.Models.Address Address);
		public DeviceViewModel GetDevice(Device Model);

		public SIPMessageViewModel GetSIPMessage(Message Message);
		public SDPViewModel GetSDPBody(Message Message);
		public SDPViewModel GetSDPBody(ISIPMessage Message);

	}
}
