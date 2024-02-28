using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.corelib
{
	public interface ISIPMessage
	{
		string GetCallID();
		string GetViaBranch();
		string GetCSeq();
		string GetFromTag();
		string? GetToTag();
		Address GetFrom();
		Address GetTo();
		
	}
}
