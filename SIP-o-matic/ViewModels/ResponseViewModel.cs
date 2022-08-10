using LogLib;
using SIP_o_matic.DataSources;
using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.ViewModels
{
	public class ResponseViewModel:SIPMessageViewModel
	{
		private Response response;


		public override string Display => response.StatusLine.ToString();

		public ResponseViewModel(ILogger Logger, Event Event, Response Response): base(Logger,Event, Response.GetHeader<FromHeader>()?.Value.ToString()?? "Undefined", Response.GetHeader<ToHeader>()?.Value.ToString() ?? "Undefined")
		{
			this.response = Response;
		}


	}
}
