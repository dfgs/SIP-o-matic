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
	public class SIPMessageViewModel : GenericViewModel<SIPMessage>
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

		public SIPMessageViewModel(SIPMessage Model) : base(Model)
		{
			StringReader reader;
			IParseResult result;

			SDP = null;
			if (string.IsNullOrEmpty(Model.Body)) return;

			reader = new StringReader(Model.Body);
			result = SDPGrammar.SDP.TryParse(reader);
			if (result is ISucceededParseResult<SDP> sdpResult) SDP = sdpResult.Value;
		}

	
		public  string GetCallID()
		{
			return Model.GetCallID();
		}
		public  string GetViaBranch()
		{
			return Model.GetViaBranch();

		}

		public  string GetCSeq()
		{
			return Model.GetCSeq();

		}
		public  string GetFromTag()
		{
			return Model.GetFromTag();


		}
		public  string? GetToTag()
		{
			return Model.GetToTag();


		}

		public  Address GetFrom()
		{
			return Model.GetFrom();

		}

		public  Address GetTo()
		{
			return Model.GetTo();

		}

	}

}
