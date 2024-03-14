using SIPParserLib;
using Stateless;
using Stateless.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.corelib.Models.Transactions
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
			NotifyStarted, NotifyProceeding, NotifyError,  NotifyTerminated,
			// BYE transaction states
			ByeStarted, ByeProceeding, ByeError, ByeTerminated,
			// OPTIONS transaction states
			OptionsStarted, OptionsTerminated,
			// REGISTER transaction states
			RegisterStarted, RegisterTerminated,
			// MESSAGE transaction states
			MessageStarted, MessageTerminated,
			// CANCEL transaction states
			CancelStarted, CancelProceeding, CancelError, CancelTerminated,
			// SUBSCRIBE transaction states
			SubscribeStarted, SubscribeProceeding, SubscribeError, SubscribeTerminated,
			// UPDATE transaction states
			UpdateStarted, UpdateProceeding, UpdateError, UpdateTerminated,

		};
		public enum Triggers { INVITE, ACK, REFER,NOTIFY,BYE,OPTIONS,REGISTER,MESSAGE,CANCEL,SUBSCRIBE,UPDATE, Prov1xx, Prov180, Final2xx, Error };


		private StateMachine<States, Triggers> fsm;
		
		protected StateMachine<States, Triggers>.TriggerWithParameters<Request> InviteTrigger;
		protected StateMachine<States, Triggers>.TriggerWithParameters<Request> ReferTrigger;
		protected StateMachine<States, Triggers>.TriggerWithParameters<Request> AckTrigger;
		protected StateMachine<States, Triggers>.TriggerWithParameters<Request> NotifyTrigger;
		protected StateMachine<States, Triggers>.TriggerWithParameters<Request> ByeTrigger;
		protected StateMachine<States, Triggers>.TriggerWithParameters<Request> OptionsTrigger;
		protected StateMachine<States, Triggers>.TriggerWithParameters<Request> RegisterTrigger;
		protected StateMachine<States, Triggers>.TriggerWithParameters<Request> MessageTrigger;
		protected StateMachine<States, Triggers>.TriggerWithParameters<Request> CancelTrigger;
		protected StateMachine<States, Triggers>.TriggerWithParameters<Request> SubscribeTrigger;
		protected StateMachine<States, Triggers>.TriggerWithParameters<Request> UpdateTrigger;


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

		/*public required string SourceDevice
		{
			get;
			set;
		}

		public required string DestinationDevice
		{
			get;
			set;
		}*/
		
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


		public List<uint> MessagesIndices
		{
			get;
			private set;
		}

		public bool IsTerminated => fsm.IsInState(TerminatedState);


		private Request? previousRequest;
		private Response? previousResponse;



		[SetsRequiredMembers]
		public Transaction(string CallID,  string ViaBranch, string CSeq)
		{
			this.CallID = CallID;
			/*this.SourceDevice = SourceDevice;
			this.DestinationDevice=DestinationDevice;*/
			this.ViaBranch = ViaBranch;
			this.CSeq = CSeq;

			MessagesIndices = new List<uint>();
				
			previousRequest = null;
			previousResponse = null;
			Retransmissions = 0;

			fsm = new StateMachine<States, Triggers>(States.Undefined);

			InviteTrigger = fsm.SetTriggerParameters<Request>(Triggers.INVITE);
			AckTrigger = fsm.SetTriggerParameters<Request>(Triggers.ACK);
			ReferTrigger = fsm.SetTriggerParameters<Request>(Triggers.REFER);
			NotifyTrigger = fsm.SetTriggerParameters<Request>(Triggers.NOTIFY);
			ByeTrigger = fsm.SetTriggerParameters<Request>(Triggers.BYE);
			OptionsTrigger = fsm.SetTriggerParameters<Request>(Triggers.OPTIONS);
			RegisterTrigger = fsm.SetTriggerParameters<Request>(Triggers.REGISTER);
			MessageTrigger = fsm.SetTriggerParameters<Request>(Triggers.MESSAGE);
			CancelTrigger = fsm.SetTriggerParameters<Request>(Triggers.CANCEL);
			SubscribeTrigger = fsm.SetTriggerParameters<Request>(Triggers.SUBSCRIBE);
			UpdateTrigger = fsm.SetTriggerParameters<Request>(Triggers.UPDATE);

			OnConfigureFSM(fsm);
		}

		protected abstract void OnConfigureFSM(StateMachine<States, Triggers> fsm);



		/*public bool Match(Request Request)
		{
			return (CallID == Request.GetCallID()) && (this.ViaBranch == Request.GetViaBranch()) && (this.CSeq==Request.GetCSeq());
		}
		public bool Match(Response Response)
		{
			return (CallID == Response.GetCallID()) && (this.ViaBranch == Response.GetViaBranch()) && (this.CSeq == Response.GetCSeq());
		}*/
		public bool Match(SIPMessage SIPMessage)
		{
			return (CallID == SIPMessage.GetCallID()) && (this.ViaBranch == SIPMessage.GetViaBranch()) && (this.CSeq == SIPMessage.GetCSeq());
		}
	

		protected bool AssertMessageBelongsToTransaction(SIPMessage SIPMessage)
		{
			return Match(SIPMessage);
		}
		

		public string GetGraph()
		{
			return UmlDotGraph.Format(fsm.GetInfo());
		}

		public bool Update(Request Request,uint MessageIndex)
		{
			if ((previousRequest!=null) && (previousRequest.RequestLine.Method== Request.RequestLine.Method))
			{
				Retransmissions++;
				return false;
			}
			previousRequest = Request;

			MessagesIndices.Add(MessageIndex);

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
				case "BYE":
					fsm.Fire(ByeTrigger, Request);
					break;
				case "OPTIONS":
					fsm.Fire(OptionsTrigger, Request);
					break;
				case "REGISTER":
					fsm.Fire(RegisterTrigger, Request);
					break;
				case "MESSAGE":
					fsm.Fire(MessageTrigger, Request);
					break;
				case "CANCEL":
					fsm.Fire(CancelTrigger, Request);
					break;
				case "SUBSCRIBE":
					fsm.Fire(SubscribeTrigger, Request);
					break;
				case "UPDATE":
					fsm.Fire(UpdateTrigger, Request);
					break;
				default: throw new InvalidOperationException($"Unsupported transaction transition ({Request.RequestLine.Method})");
			}
			return true;
		}
		public bool Update(Response Response,uint MessageIndex)
		{
			if ((previousResponse!=null) && (previousResponse.StatusLine.StatusCode==Response.StatusLine.StatusCode))
			{
				Retransmissions++;
				return false;
			}
			previousResponse = Response;

			MessagesIndices.Add(MessageIndex);

			fsm.Fire(OnGetUpdateTrigger(Response),Response);
			return true;
		}

		protected abstract StateMachine<States, Triggers>.TriggerWithParameters<Response> OnGetUpdateTrigger(Response Response);

	}
}
