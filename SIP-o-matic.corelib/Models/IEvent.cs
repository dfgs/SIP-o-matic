using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.corelib.Models
{
	public interface IEvent
	{
		Address SourceAddress
		{
			get;
		}
		Address DestinationAddress
		{
			get;
		}

	}
}
