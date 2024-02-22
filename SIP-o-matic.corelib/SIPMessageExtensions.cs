using LogLib;
using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.corelib
{
	public static class SIPMessageExtensions
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
			SIPURL? fromURL;
			string? value;
			URLParameter? parameter;

			fromURL = Message.GetHeader<FromHeader>()?.Value.URI as SIPURL;
			if (fromURL == null)
			{
				string error = $"Invalid or missing from URI in SIP message";
					throw new InvalidOperationException(error);
			}

			parameter = fromURL.Parameters.FirstOrDefault(item => item.Name == "tag");
            if ( parameter==null)
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
			SIPURL? toURL;
			string? value;
			URLParameter? parameter;

			toURL = Message.GetHeader<ToHeader>()?.Value.URI as SIPURL;
			if (toURL == null)
			{
				string error = $"Invalid or missing to URI in SIP message";
				throw new InvalidOperationException(error);
			}

			parameter = toURL.Parameters.FirstOrDefault(item => item.Name == "tag");
			if (parameter == null)
			{
				string error = $"Tag parameter missing in to header";
				throw new InvalidOperationException(error);
			}
			value = parameter.Value.Value;

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
