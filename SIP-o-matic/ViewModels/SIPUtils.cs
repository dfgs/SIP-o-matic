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
		public static int GetDialogUID(SIPMessage SIPMessage,string SourceAddress,string DestinationAddress)
		{
			string s, d;
			string addressID;
			string callID;
			string fromTag;
			int dialogID;

			s = SourceAddress ?? "";
			d = DestinationAddress ?? "";

			if (s.CompareTo(d)<0) addressID = s + d;
			else addressID = d + s;

			callID = SIPMessage.GetHeader<CallIDHeader>()?.Value ?? "";
			fromTag = SIPMessage.GetHeader<FromHeader>()?.Value.Tag ?? "";
			dialogID = (addressID+callID+fromTag).GetHashCode();

			return dialogID;
		}
		public static int GetTransactionUID(SIPMessage SIPMessage)
		{
			string transactionID;

			transactionID = SIPMessage.GetHeader<CSeqHeader>()?.Value ?? ""
							+ SIPMessage.GetHeader<ViaHeader>()?.GetParameter<ViaBranch>()?.Value ?? "";

			return transactionID.GetHashCode();
		}

	}
}
