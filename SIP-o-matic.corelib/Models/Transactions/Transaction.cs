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
		
		protected StateMachine<States, Triggers>.TriggerWithParameters<IRequest> InviteTrigger;
		protected StateMachine<States, Triggers>.TriggerWithParameters<IRequest> ReferTrigger;
		protected StateMachine<States, Triggers>.TriggerWithParameters<IRequest> AckTrigger;
		protected StateMachine<States, Triggers>.TriggerWithParameters<IRequest> NotifyTrigger;
		protected StateMachine<States, Triggers>.TriggerWithParameters<IRequest> ByeTrigger;
		protected StateMachine<States, Triggers>.TriggerWithParameters<IRequest> OptionsTrigger;
		protected StateMachine<States, Triggers>.TriggerWithParameters<IRequest> RegisterTrigger;
		protected StateMachine<States, Triggers>.TriggerWithParameters<IRequest> MessageTrigger;
		protected StateMachine<States, Triggers>.TriggerWithParameters<IRequest> CancelTrigger;
		protected StateMachine<States, Triggers>.TriggerWithParameters<IRequest> SubscribeTrigger;
		protected StateMachine<States, Triggers>.TriggerWithParameters<IRequest> UpdateTrigger;


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


		private IRequest? previousRequest;
		private IResponse? previousResponse;



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

			InviteTrigger = fsm.SetTriggerParameters<IRequest>(Triggers.INVITE);
			AckTrigger = fsm.SetTriggerParameters<IRequest>(Triggers.ACK);
			ReferTrigger = fsm.SetTriggerParameters<IRequest>(Triggers.REFER);
			NotifyTrigger = fsm.SetTriggerParameters<IRequest>(Triggers.NOTIFY);
			ByeTrigger = fsm.SetTriggerParameters<IRequest>(Triggers.BYE);
			OptionsTrigger = fsm.SetTriggerParameters<IRequest>(Triggers.OPTIONS);
			RegisterTrigger = fsm.SetTriggerParameters<IRequest>(Triggers.REGISTER);
			MessageTrigger = fsm.SetTriggerParameters<IRequest>(Triggers.MESSAGE);
			CancelTrigger = fsm.SetTriggerParameters<IRequest>(Triggers.CANCEL);
			SubscribeTrigger = fsm.SetTriggerParameters<IRequest>(Triggers.SUBSCRIBE);
			UpdateTrigger = fsm.SetTriggerParameters<IRequest>(Triggers.UPDATE);

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

		public bool Match(ISIPMessage MessageInfo)
		{
			return (CallID == MessageInfo.GetCallID()) && (this.ViaBranch == MessageInfo.GetViaBranch()) && (this.CSeq == MessageInfo.GetCSeq());
		}

		protected bool AssertMessageBelongsToTransaction(ISIPMessage MessageInfo)
		{
			return Match(MessageInfo);
		}
		

		public string GetGraph()
		{
			return UmlDotGraph.Format(fsm.GetInfo());
		}

		public bool Update(IRequest Request,uint MessageIndex)
		{
			if ((previousRequest!=null) && (previousRequest.Method== Request.Method))
			{
				Retransmissions++;
				return false;
			}
			previousRequest = Request;

			MessagesIndices.Add(MessageIndex);

			switch (Request.Method)
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
				default: throw new InvalidOperationException($"Unsupported transaction transition ({Request.Method})");
			}
			return true;
		}
		public bool Update(IResponse Response,uint MessageIndex)
		{
			if ((previousResponse!=null) && (previousResponse.StatusCode==Response.StatusCode))
			{
				Retransmissions++;
				return false;
			}
			previousResponse = Response;

			MessagesIndices.Add(MessageIndex);

			fsm.Fire(OnGetUpdateTrigger(Response),Response);
			return true;
		}

		protected abstract StateMachine<States, Triggers>.TriggerWithParameters<IResponse> OnGetUpdateTrigger(IResponse Response);

	}
}
