using LogLib;
using ParserLib;
using SIP_o_matic.corelib;
using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class SIPMessageViewModel : ViewModel<SIPMessage>, IRequest,IResponse
	{
		public enum Types { Response, Request };

		public Types MessageType
		{
			get
			{
				if (Model is Request) return Types.Request;
				else return Types.Response;
			}
		}

		public uint Index
		{
			get;
			private set;
		}

			
		public string Description
		{
			get
			{
				if (Model is Request request) return request.RequestLine.ToString();
				else if (Model is Response response) return response.StatusLine.ToString();
				return "Undefined";
			}
		}

		public bool IsRequest
		{
			get => Model is Request;
		}

		public string Method
		{
			get
			{
				if (Model is Request request) return request.RequestLine.Method;
				throw new InvalidOperationException($"SIP message is not Request");
			}
		}

		public ushort StatusCode
		{
			get
			{
				if (Model is Response response) return response.StatusLine.StatusCode;
				throw new InvalidOperationException($"SIP message is not Response");
			}
		}

		public IEnumerable<MessageHeader> Headers
		{
			get => Model.Headers;
		}

		public SDP? SDP
		{
			get;
			private set;
		}

		public SIPMessageViewModel(ILogger Logger) : base(Logger)
		{
		}

		protected override void OnLoaded()
		{
			StringReader reader;
			IParseResult result;

			base.OnLoaded();

			SDP = null;
			if (string.IsNullOrEmpty(Model.Body)) return;

			reader = new StringReader(Model.Body);
			result=SDPGrammar.SDP.TryParse(reader);
			if (result is ISucceededParseResult<SDP> sdpResult) SDP = sdpResult.Value;
			
		}
		public  string GetCallID()
		{
			string? value;

			value = Model.GetHeader<CallIDHeader>()?.Value;
			if (value == null)
			{
				string error = $"CallID header missing in SIP message";
				throw new InvalidOperationException(error);
			}

			return value;
		}
		public  string GetViaBranch()
		{
			string? value;

			value = Model.GetHeader<ViaHeader>()?.Value;
			if (value == null)
			{
				string error = $"Via branch header missing in SIP message";
				throw new InvalidOperationException(error);
			}

			return value;
		}

		public  string GetCSeq()
		{
			string? value;

			value = Model.GetHeader<CSeqHeader>()?.Value;
			if (value == null)
			{
				string error = $"CSeq header missing in SIP message";
				throw new InvalidOperationException(error);
			}

			return value;
		}
		public  string GetFromTag()
		{
			SIPURL? fromURL;
			string? value;
			URLParameter? parameter;

			fromURL = Model.GetHeader<FromHeader>()?.Value.URI as SIPURL;
			if (fromURL == null)
			{
				string error = $"Invalid or missing from URI in SIP message";
				throw new InvalidOperationException(error);
			}

			parameter = fromURL.Parameters.FirstOrDefault(item => item.Name == "tag");
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
		public  string? GetToTag()
		{
			SIPURL? toURL;
			string? value;
			URLParameter? parameter;

			toURL = Model.GetHeader<ToHeader>()?.Value.URI as SIPURL;
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

		public  Address GetFrom()
		{
			Address? value;

			value = Model.GetHeader<FromHeader>()?.Value;
			if (value == null)
			{
				string error = $"From header missing in SIP message";
				throw new InvalidOperationException(error);
			}

			return value.Value;
		}

		public  Address GetTo()
		{
			Address? value;

			value = Model.GetHeader<ToHeader>()?.Value;
			if (value == null)
			{
				string error = $"To header missing in SIP message";
				throw new InvalidOperationException(error);
			}

			return value.Value;
		}

	}

}
