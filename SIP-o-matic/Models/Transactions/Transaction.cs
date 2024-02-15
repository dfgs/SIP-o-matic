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

		public required string SourceDevice
		{
			get;
			set;
		}

		public required string DestinationDevice
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


		public List<uint> MessagesIndices
		{
			get;
			private set;
		}

		public bool IsTerminated => fsm.IsInState(TerminatedState);


		private Request? previousRequest;
		private Response? previousResponse;



		[SetsRequiredMembers]
		public Transaction(string CallID, string SourceDevice, string DestinationDevice, string ViaBranch, string CSeq)
		{
			this.CallID = CallID;
			this.SourceDevice = SourceDevice;
			this.DestinationDevice=DestinationDevice;
			this.ViaBranch = ViaBranch;
			this.CSeq = CSeq;

			MessagesIndices = new List<uint>();
				
			previousRequest = null;
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

		private bool SourceAndDestinationMatch(string SourceDevice, string DestinationDevice)
		{
			return ((this.SourceDevice == SourceDevice) && (this.DestinationDevice == DestinationDevice))
				|| ((this.SourceDevice == DestinationDevice) && (this.DestinationDevice == SourceDevice));
		}

		public bool Match(Request Request, string SourceDevice, string DestinationDevice)
		{
			return (CallID == Request.GetHeader<CallIDHeader>()?.Value) && (ViaBranch == Request.GetHeader<ViaHeader>()?.GetParameter<ViaBranch>()?.Value) && (CSeq == Request.GetHeader<CSeqHeader>()?.Value)
				&& SourceAndDestinationMatch(SourceDevice, DestinationDevice);
		}
		public bool Match(Response Response, string SourceDevice, string DestinationDevice)
		{
			return (CallID == Response.GetHeader<CallIDHeader>()?.Value) && (ViaBranch == Response.GetHeader<ViaHeader>()?.GetParameter<ViaBranch>()?.Value) && (CSeq == Response.GetHeader<CSeqHeader>()?.Value)
				&& SourceAndDestinationMatch(SourceDevice, DestinationDevice);
		}

		protected bool AssertMessageBelongsToTransaction(Request Request,string SourceDevice, string DestinationDevice)
		{
			return Match(Request,SourceDevice,DestinationDevice);
		}
		protected bool AssertMessageBelongsToTransaction(Response Response, string SourceDevice, string DestinationDevice)
		{
			return Match(Response, SourceDevice, DestinationDevice);
		}

		public string GetGraph()
		{
			return UmlDotGraph.Format(fsm.GetInfo());
		}

		public bool Update(Request Request, string SourceDevice, string DestinationDevice,uint MessageIndex)
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
					fsm.Fire(InviteTrigger, Request,SourceDevice,DestinationDevice);
					break;
				case "ACK":
					fsm.Fire(AckTrigger, Request, SourceDevice, DestinationDevice);
					break;
				case "REFER":
					fsm.Fire(ReferTrigger, Request, SourceDevice, DestinationDevice);
					break;
				case "NOTIFY":
					fsm.Fire(NotifyTrigger, Request, SourceDevice, DestinationDevice);
					break;
				case "BYE":
					fsm.Fire(ByeTrigger, Request, SourceDevice, DestinationDevice);
					break;
				default: throw new InvalidOperationException($"Unsupported transaction transition ({Request.RequestLine.Method})");
			}
			return true;
		}
		public bool Update(Response Response,string SourceDevice,string DestinationDevice,uint MessageIndex)
		{
			if ((previousResponse!=null) && (previousResponse.StatusLine.StatusCode==Response.StatusLine.StatusCode))
			{
				Retransmissions++;
				return false;
			}
			previousResponse = Response;

			MessagesIndices.Add(MessageIndex);

			fsm.Fire(OnGetUpdateTrigger(Response),Response, SourceDevice, DestinationDevice);
			return true;
		}

		protected abstract StateMachine<States, Triggers>.TriggerWithParameters<Response, string, string> OnGetUpdateTrigger(Response Response);

	}
}
