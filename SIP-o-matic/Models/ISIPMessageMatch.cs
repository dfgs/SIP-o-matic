using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.Models
{
	public interface ISIPMessageMatch
	{
		bool Match(Request Request,string SourceDevice,string DestinationDevice);
		bool Match(Response Response, string SourceDevice, string DestinationDevice);
	}
}
