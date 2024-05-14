using LogLib;
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
	public class SIPMessageViewModel : GenericViewModel<ISIPMessage>
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
			get
			{
				if (Model is SIPMessage sipMessage) return sipMessage.Headers;
				else return Enumerable.Empty<MessageHeader>();
			}
		}

		public SDPViewModel SDP
		{
			get;
			private set;
		}

		public SIPMessageViewModel(ISIPMessage Model,IDeviceNameProvider DeviceNameProvider) : base(Model)
		{
			this.SDP = DeviceNameProvider.GetSDPBody(Model);
		}

	
		public string GetCallID()
		{
			if (!(Model is SIPMessage validSIPMessage)) throw new InvalidOperationException("Invalid SIP message");
			return validSIPMessage.GetCallID();
		}
		public string GetViaBranch()
		{
			if (!(Model is SIPMessage validSIPMessage)) throw new InvalidOperationException("Invalid SIP message");
			return validSIPMessage.GetViaBranch();
		}

		public  string GetCSeq()
		{
			if (!(Model is SIPMessage validSIPMessage)) throw new InvalidOperationException("Invalid SIP message");
			return validSIPMessage.GetCSeq();
		}

		public  string GetFromTag()
		{
			if (!(Model is SIPMessage validSIPMessage)) throw new InvalidOperationException("Invalid SIP message");
			return validSIPMessage.GetFromTag();
		}
		public  string? GetToTag()
		{
			if (!(Model is SIPMessage validSIPMessage)) throw new InvalidOperationException("Invalid SIP message");
			return validSIPMessage.GetToTag();
		}

		public  Address GetFrom()
		{
			if (!(Model is SIPMessage validSIPMessage)) throw new InvalidOperationException("Invalid SIP message");
			return validSIPMessage.GetFrom();
		}

		public  Address GetTo()
		{
			if (!(Model is SIPMessage validSIPMessage)) throw new InvalidOperationException("Invalid SIP message");
			return validSIPMessage.GetTo();
		}

	}

}
