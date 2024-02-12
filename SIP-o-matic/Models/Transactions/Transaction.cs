using SIPParserLib;
using Stateless;
using Stateless.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.Models.Transactions
{
	public abstract class Transaction: ISIPMessageMatch
	{
		protected static string TransactionErrorMessage = "Message doesn't belong to current transaction";

		public enum States
		{
			Undefined,
			// INVITE transaction states
			InviteStarted, InviteProceeding, InviteRinging, InviteError, InviteTerminated,
			// ACK transaction states 
			AckTerminated,
			// REFER transaction states
			ReferStarted, ReferProceeding, ReferError, ReferTerminated,
			// NOTIFY transaction states
			NotifyStarted, NotifyProceeding, NotifyError, NotifyTerminated,
			// BYE transaction states
			ByeStarted, ByeProceeding, ByeError, ByeTerminated,

		};
		public enum Triggers { INVITE, ACK, REFER,NOTIFY,BYE, Prov1xx, Prov180, Final2xx, Error };


		private StateMachine<States, Triggers> fsm;
		
		protected StateMachine<States, Triggers>.TriggerWithParameters<Request, string, string> InviteTrigger;
		protected StateMachine<States, Triggers>.TriggerWithParameters<Request, string,string> ReferTrigger;
		protected StateMachine<States, Triggers>.TriggerWithParameters<Request, string, string> AckTrigger;
		protected StateMachine<States, Triggers>.TriggerWithParameters<Request, string, string> NotifyTrigger;
		protected StateMachine<States, Triggers>.TriggerWithParameters<Request, string, string> ByeTrigger;


		public required string CallID
		{
			get;
			set;
		}
		public required string ViaBranch
		{
			get;
			set;
		}

		public required string SourceAddress
		{
			get;
			set;
		}

		public required string DestinationAddress
		{
			get;
			set;
		}

		public required string CSeq
		{
			get;
			set;
		}

		protected abstract States TerminatedState
		{
			get;
		}

		public uint Retransmissions
		{
			get;
			set;
		}

		public States State => fsm.State;

		public bool IsTerminated => fsm.IsInState(TerminatedState);


		private Request? previousRequest;
		private Response? previousResponse;



		[SetsRequiredMembers]
		public Transaction(string CallID, string SourceAddress, string DestinationAddress, string ViaBranch, string CSeq)
		{
			this.CallID = CallID;
			this.SourceAddress = SourceAddress;
			this.DestinationAddress=DestinationAddress;
			this.ViaBranch = ViaBranch;
			this.CSeq = CSeq;

			previousRequest= null; ;
			previousResponse = null;
			Retransmissions = 0;

			fsm = new StateMachine<States, Triggers>(States.Undefined);

			InviteTrigger = fsm.SetTriggerParameters<Request,string,string>(Triggers.INVITE);
			AckTrigger = fsm.SetTriggerParameters<Request, string, string>(Triggers.ACK);
			ReferTrigger = fsm.SetTriggerParameters<Request, string, string>(Triggers.REFER);
			NotifyTrigger = fsm.SetTriggerParameters<Request, string, string>(Triggers.NOTIFY);
			ByeTrigger = fsm.SetTriggerParameters<Request, string, string>(Triggers.BYE);

			OnConfigureFSM(fsm);
		}

		protected abstract void OnConfigureFSM(StateMachine<States, Triggers> fsm);

		private bool SourceAndDestinationMatch(string SourceAddress, string DestinationAddress)
		{
			return ((this.SourceAddress == SourceAddress) && (this.DestinationAddress == DestinationAddress))
				|| ((this.SourceAddress == DestinationAddress) && (this.DestinationAddress == SourceAddress));
		}

		public bool Match(Request Request, string SourceAddress, string DestinationAddress)
		{
			return (CallID == Request.GetHeader<CallIDHeader>()?.Value) && (ViaBranch == Request.GetHeader<ViaHeader>()?.GetParameter<ViaBranch>()?.Value) && (CSeq == Request.GetHeader<CSeqHeader>()?.Value)
				&& SourceAndDestinationMatch(SourceAddress, DestinationAddress);
		}
		public bool Match(Response Response, string SourceAddress, string DestinationAddress)
		{
			return (CallID == Response.GetHeader<CallIDHeader>()?.Value) && (ViaBranch == Response.GetHeader<ViaHeader>()?.GetParameter<ViaBranch>()?.Value) && (CSeq == Response.GetHeader<CSeqHeader>()?.Value)
				&& SourceAndDestinationMatch(SourceAddress, DestinationAddress);
		}

		protected bool AssertMessageBelongsToTransaction(Request Request,string SourceAddress, string DestinationAddress)
		{
			return Match(Request,SourceAddress,DestinationAddress);
		}
		protected bool AssertMessageBelongsToTransaction(Response Response, string SourceAddress, string DestinationAddress)
		{
			return Match(Response, SourceAddress, DestinationAddress);
		}

		public string GetGraph()
		{
			return UmlDotGraph.Format(fsm.GetInfo());
		}

		public bool Update(Request Request, string SourceAddress, string DestinationAddress)
		{
			if ((previousRequest!=null) && (previousRequest.RequestLine.Method== Request.RequestLine.Method))
			{
				Retransmissions++;
				return false;
			}
			previousRequest = Request;

			switch (Request.RequestLine.Method)
			{
				case "INVITE":
					fsm.Fire(InviteTrigger, Request,SourceAddress,DestinationAddress);
					break;
				case "ACK":
					fsm.Fire(AckTrigger, Request, SourceAddress, DestinationAddress);
					break;
				case "REFER":
					fsm.Fire(ReferTrigger, Request, SourceAddress, DestinationAddress);
					break;
				case "NOTIFY":
					fsm.Fire(NotifyTrigger, Request, SourceAddress, DestinationAddress);
					break;
				case "BYE":
					fsm.Fire(ByeTrigger, Request, SourceAddress, DestinationAddress);
					break;
				default: throw new InvalidOperationException($"Unsupported transaction transition ({Request.RequestLine.Method})");
			}
			return true;
		}
		public bool Update(Response Response,string SourceAddress,string DestinationAddress)
		{
			if ((previousResponse!=null) && (previousResponse.StatusLine.StatusCode==Response.StatusLine.StatusCode))
			{
				Retransmissions++;
				return false;
			}
			previousResponse = Response;

			fsm.Fire(OnGetUpdateTrigger(Response),Response, SourceAddress, DestinationAddress);
			return true;
		}

		protected abstract StateMachine<States, Triggers>.TriggerWithParameters<Response, string, string> OnGetUpdateTrigger(Response Response);

	}
}
