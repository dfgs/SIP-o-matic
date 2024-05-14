using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.corelib
{
    public static class SIPExtensions
    {

    

		public static string GetCallID(this SIPMessage Message)
		{
			string? value;

			value = Message.GetHeader<CallIDHeader>()?.Value;
			if (value == null)
			{
				string error = $"CallID header missing in SIP message";
				throw new InvalidOperationException(error);
			}

			return value;
		}
		public static string GetViaBranch(this SIPMessage Message)
		{
			string? value;

			value = Message.GetHeader<ViaHeader>()?.Value;
			if (value == null)
			{
				string error = $"Via branch header missing in SIP message";
				throw new InvalidOperationException(error);
			}

			return value;
		}

		public static string GetCSeq(this SIPMessage Message)
		{
			string? value;

			value = Message.GetHeader<CSeqHeader>()?.Value;
			if (value == null)
			{
				string error = $"CSeq header missing in SIP message";
				throw new InvalidOperationException(error);
			}

			return value;
		}
		public static string GetFromTag(this SIPMessage Message)
		{
			Address? fromAddress;
			string? value;
			AddressParameter? parameter;

			fromAddress= Message.GetHeader<FromHeader>()?.Value;
			if (fromAddress == null)
			{
				string error = $"Invalid or missing from address in SIP message";
				throw new InvalidOperationException(error);
			}


			parameter = fromAddress.Value.Parameters?.FirstOrDefault(item => item.Name == "tag");
			if (parameter == null)
			{
				string error = $"Tag parameter missing in from header";
				throw new InvalidOperationException(error);
			}

			value = parameter.Value.Value;
			if (value == null)
			{
				string error = $"From tag missing in SIP message";
				throw new InvalidOperationException(error);
			}
			return value;

		}
		public static string? GetToTag(this SIPMessage Message)
		{
			Address? toAddress;
			string? value;
			AddressParameter? parameter;

			toAddress = Message.GetHeader<ToHeader>()?.Value;
			if (toAddress == null)
			{
				string error = $"Invalid or missing to address in SIP message";
				throw new InvalidOperationException(error);
			}


			parameter = toAddress.Value.Parameters?.FirstOrDefault(item => item.Name == "tag");
			if (parameter == null) return null;
			

			value = parameter.Value.Value;
			if (value == null)
			{
				string error = $"From tag missing in SIP message";
				throw new InvalidOperationException(error);
			}
			return value;


		}

		public static Address GetFrom(this SIPMessage Message)
		{
			Address? value;

			value = Message.GetHeader<FromHeader>()?.Value;
			if (value == null)
			{
				string error = $"From header missing in SIP message";
				throw new InvalidOperationException(error);
			}

			return value.Value;
		}

		public static Address GetTo(this SIPMessage Message)
		{
			Address? value;

			value = Message.GetHeader<ToHeader>()?.Value;
			if (value == null)
			{
				string error = $"To header missing in SIP message";
				throw new InvalidOperationException(error);
			}

			return value.Value;
		}


	}
}
