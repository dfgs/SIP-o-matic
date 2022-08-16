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
		public static int GetDialogUIDFirstStage(SIPMessage SIPMessage,string SourceAddress,string DestinationAddress)
		{
			string sourceAddress, destinationAddress;
			string addressID,tagID;
			string callID;
			string fromTag;
			int dialogID;

			sourceAddress = SourceAddress ?? "";
			destinationAddress = DestinationAddress ?? "";
			fromTag = SIPMessage.GetHeader<FromHeader>()?.Value.Tag ?? "";
			callID = SIPMessage.GetHeader<CallIDHeader>()?.Value ?? "";

			if (sourceAddress.CompareTo(destinationAddress) < 0)
			{
				addressID = sourceAddress + destinationAddress;
				tagID = fromTag ;
			}
			else
			{
				addressID = destinationAddress + sourceAddress;
				tagID = fromTag;
			}

			dialogID = (addressID+callID+tagID).GetHashCode();

			return dialogID;
		}
		public static int GetDialogUIDSecondStage(SIPMessage SIPMessage, string SourceAddress, string DestinationAddress)
		{
			string sourceAddress, destinationAddress;
			string addressID, tagID;
			string callID;
			string fromTag;
			string toTag;
			int dialogID;

			sourceAddress = SourceAddress ?? "";
			destinationAddress = DestinationAddress ?? "";
			fromTag = SIPMessage.GetHeader<FromHeader>()?.Value.Tag ?? "";
			toTag = SIPMessage.GetHeader<ToHeader>()?.Value.Tag ?? "";
			callID = SIPMessage.GetHeader<CallIDHeader>()?.Value ?? "";

			if (sourceAddress.CompareTo(destinationAddress) < 0)
			{
				addressID = sourceAddress + destinationAddress;
			}
			else
			{
				addressID = destinationAddress + sourceAddress;
			}
			if (fromTag.CompareTo(toTag) < 0)
			{
				tagID = fromTag + toTag;
			}
			else
			{
				tagID = toTag + fromTag;
			}

			dialogID = (addressID + callID + tagID).GetHashCode();

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
