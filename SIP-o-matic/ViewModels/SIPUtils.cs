using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.ViewModels
{
	public static class SIPUtils
	{
		public static int GetMessageUID(string Message)
		{
			return Message.GetHashCode();
		}
		public static int GetCallUID(SIPMessage SIPMessage)
		{
			string callID;

			callID = SIPMessage.GetHeader<CallIDHeader>()?.Value??"";

			return callID.GetHashCode();
		}


	}
}
