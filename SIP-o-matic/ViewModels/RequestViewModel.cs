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
	public class RequestViewModel:SIPMessageViewModel
	{
		private Request request;

		public override string? From
		{
			get => request.GetHeader<FromHeader>()?.Value.ToShortString();
		}
		public override string? To
		{
			get => request.GetHeader<ToHeader>()?.Value.ToShortString();
		}
		public override string? CallID
		{
			get => request.GetHeader<CallIDHeader>()?.Value;
		}
		public override string? FromTag
		{
			get => request.GetHeader<FromHeader>()?.Value.Tag;
		}
		public override string? ToTag
		{
			get => request.GetHeader<ToHeader>()?.Value.Tag;
		}
		public override string? CSeq
		{
			get => request.GetHeader<CSeqHeader>()?.Value;
		}
		public override string? ViaBranch 
		{
			get => request.GetHeader<ViaHeader>()?.GetParameter<ViaBranch>()?.Value;
		}

		public override string Display => request.RequestLine.ToString();
		public override string ShortDisplay => request.RequestLine.Method;

		public RequestViewModel(ILogger Logger, Event Event, Request Request) : base(Logger,Event)
		{
			this.request = Request;
		}
	}
}
