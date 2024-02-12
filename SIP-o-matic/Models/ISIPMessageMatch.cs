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
		bool Match(Request Request,string SourceAddress,string DestinationAddress);
		bool Match(Response Response, string SourceAddress, string DestinationAddress);
	}
}
