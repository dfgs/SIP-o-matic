using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.corelib.Models
{
	public interface ISIPMessageMatch
	{
		//bool Match(Request Request);
		//bool Match(Response Response);
		bool Match(SIPMessage SIPMessage);
	}
}
