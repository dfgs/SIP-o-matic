using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.corelib
{
	public static class Utils
	{
		public static int Hash(string SourceDevice,string DestinationDevice)
		{
			int hash; ;

			int hash1, hash2;
			hash1 = 23 * 31 + SourceDevice.GetHashCode();
			hash2 = 23 * 31 + DestinationDevice.GetHashCode();
			hash = hash1 ^ hash2;

			return hash;
		}
	}
}
