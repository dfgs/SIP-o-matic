using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.DataSources
{
	public struct Device
	{
		public string Name
		{
			get;
			private set;
		}
		public string Address
		{
			get;
			private set;
		}

		public Device(string Name, string Address)
		{
			this.Name = Name;this.Address = Address;
		}


	}
}
