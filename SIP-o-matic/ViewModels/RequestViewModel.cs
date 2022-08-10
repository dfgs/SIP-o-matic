using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.ViewModels
{
	public class RequestViewModel:SIPMessageViewModel
	{
		private Request request;



		public override string Display => request.RequestLine.ToString();

		public RequestViewModel(int UID, DateTime Timestamp, Request Request) : base(UID, Timestamp, Request.GetHeader<FromHeader>()?.Value.ToString() ?? "Undefined", Request.GetHeader<ToHeader>()?.Value.ToString() ?? "Undefined")
		{
			this.request = Request;
		}
	}
}
