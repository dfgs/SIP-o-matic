using LogLib;
using SIP_o_matic.DataSources;
using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace SIP_o_matic.ViewModels
{
	public class ResponseViewModel:SIPMessageViewModel
	{
		private Response response;
		public Response Response => response;
		public override string? From
		{
			get => response.GetHeader<FromHeader>()?.Value.ToShortString();
		}
		public override string? To
		{
			get => response.GetHeader<ToHeader>()?.Value.ToShortString();
		}
		public override string? CallID
		{
			get => response.GetHeader<CallIDHeader>()?.Value;
		}

		public override string? FromTag
		{
			get => response.GetHeader<FromHeader>()?.Value.Tag;
		}

		public override string? ToTag
		{
			get => response.GetHeader<ToHeader>()?.Value.Tag;
		}

		public override string? CSeq
		{
			get => response.GetHeader<CSeqHeader>()?.Value;
		}
		public override string? ViaBranch
		{
			get => response.GetHeader<ViaHeader>()?.GetParameter<ViaBranch>()?.Value;
		}

		public override string Display => response.StatusLine.ToString();

		public override string ShortDisplay => response.StatusLine.ToString();


		public Statuses Status
		{
			get;
			private set;
		}


		public ResponseViewModel(ILogger Logger, Event Event, Response Response, SDP? SDP) : base(Logger,Event,SDP)
		{
			this.response = Response;
		}
		public void Analyze()
		{
			char code;

			code = response.StatusLine.StatusCode[0];
			switch(code)
			{
				case '1': Status = Statuses.Success; break;
				case '2': Status = Statuses.Success; break;
				case '3': Status = Statuses.Redirected; break;
				default: Status = Statuses.Failed; break;
			}
			OnPropertyChanged(nameof(Status));
		}
		public override MessageHeader? GetHeader(string Name)
		{
			return response.Headers.FirstOrDefault(item => item.Name == Name);
		}


	}
}
