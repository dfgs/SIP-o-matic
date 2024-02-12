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
	public abstract class Transaction:ICloneable<Transaction>, ISIPMessageMatch
	{
		public enum States
		{
			Undefined,
			// INVITE transaction states
			InviteStarted, InviteProceeding, InviteRinging, InviteCompleted, InviteTerminated,
			// ACK transaction states 
			AckTerminated,
			// REFER transaction states
			ReferStarted, ReferProceeding, ReferTerminated,
			// Notify transaction states
			NotifyStarted, NotifyProceeding, NotifyTerminated,

		};
		public enum Triggers { INVITE, ACK, REFER,NOTIFY, Prov1xx, Prov180, Final2xx, Error };


		private StateMachine<States, Triggers> fsm;
		
		protected StateMachine<States, Triggers>.TriggerWithParameters<Request> InviteTrigger;
		protected StateMachine<States, Triggers>.TriggerWithParameters<Request> ReferTrigger;
		protected StateMachine<States, Triggers>.TriggerWithParameters<Request> AckTrigger;
		protected StateMachine<States, Triggers>.TriggerWithParameters<Request> NotifyTrigger;


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

		public required string CSeq
		{
			get;
			set;
		}

		protected abstract States TerminatedState
		{
			get;
		}

		public States State => fsm.State;

		public bool IsTerminated => fsm.IsInState(TerminatedState);

		public Transaction(States InitialState)
		{
			fsm = new StateMachine<States, Triggers>(InitialState);

			InviteTrigger = fsm.SetTriggerParameters<Request>(Triggers.INVITE);
			AckTrigger = fsm.SetTriggerParameters<Request>(Triggers.ACK);
			ReferTrigger = fsm.SetTriggerParameters<Request>(Triggers.REFER);
			NotifyTrigger = fsm.SetTriggerParameters<Request>(Triggers.NOTIFY);

			OnConfigureFSM(fsm);
		}

		[SetsRequiredMembers]
		public Transaction(string CallID, string ViaBranch, string CSeq, States InitialState):this(InitialState)
		{
			this.CallID = CallID;
			this.ViaBranch = ViaBranch;
			this.CSeq = CSeq;
		}

		protected abstract void OnConfigureFSM(StateMachine<States, Triggers> fsm);

		public bool Match(Request Request)
		{
			return (CallID == Request.GetHeader<CallIDHeader>()?.Value) && (ViaBranch == Request.GetHeader<ViaHeader>()?.GetParameter<ViaBranch>()?.Value) && (CSeq == Request.GetHeader<CSeqHeader>()?.Value);
		}
		public bool Match(Response Response)
		{
			return (CallID == Response.GetHeader<CallIDHeader>()?.Value) && (ViaBranch == Response.GetHeader<ViaHeader>()?.GetParameter<ViaBranch>()?.Value) && (CSeq == Response.GetHeader<CSeqHeader>()?.Value);
		}

		protected bool AssertMessageBelongsToTransaction(Request Request)
		{
			return Match(Request);
		}
		protected bool AssertMessageBelongsToTransaction(Response Response)
		{
			return Match(Response);
		}

		public string GetGraph()
		{
			return UmlDotGraph.Format(fsm.GetInfo());
		}

		public abstract Transaction Clone();
		public void Update(Request Request)
		{
			switch (Request.RequestLine.Method)
			{
				case "INVITE":
					fsm.Fire(InviteTrigger, Request);
					break;
				case "ACK":
					fsm.Fire(AckTrigger, Request);
					break;
				case "REFER":
					fsm.Fire(ReferTrigger, Request);
					break;
				case "NOTIFY":
					fsm.Fire(NotifyTrigger, Request);
					break;
				default: throw new InvalidOperationException($"Unsupported transaction transition ({Request.RequestLine.Method})");
			}
		}
		public void Update(Response Response)
		{
			fsm.Fire(OnGetUpdateTrigger(Response),Response);
		}

		protected abstract StateMachine<States, Triggers>.TriggerWithParameters<Response> OnGetUpdateTrigger(Response Response);

	}
}
