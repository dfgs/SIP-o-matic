using SIP_o_matic.corelib.Models.Transactions;
using SIPParserLib;
using Stateless;
using Stateless.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using static SIP_o_matic.corelib.Models.Transactions.Transaction;

namespace SIP_o_matic.corelib.Models
{
	public class Call:ICloneable<Call>,ISIPMessageMatch
	{
		public  enum States { OnHook,Calling, Ringing,Established, Transfering, Transfered, Terminated,Error };

		private StateMachine<States, Transaction.States> fsm;

		private StateMachine<States, Transaction.States>.TriggerWithParameters<Transaction> notifyTerminatedTrigger;

		public required string CallID
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
		public required string FromTag
		{
			get;
			set;
		}

		public string? ToTag
		{
			get;
			set;
		}

		public required string Caller
		{
			get;
			set;
		}
		public required string Callee
		{
			get;
			set;
		}

		public required bool IsAck
		{
			get;
			set;
		}

		public  States State
		{
			get => fsm.State;
		}

		public uint[] MessageIndices
		{
			get;
			set;
		}

		public string? ReplacedCallID
		{
			get;
			set;
		}

		public bool IsUpdated
		{
			get;
			private set;
		}

		[SetsRequiredMembers]
		public Call(string callID, string SourceDevice,string DestinationDevice,string FromTag, string Caller, string Callee, States InitialState,bool IsAck)
		{
	
			CallID = callID;
			this.SourceDevice = SourceDevice;
			this.DestinationDevice = DestinationDevice;
			this.FromTag = FromTag;
			this.Caller = Caller;
			this.Callee = Callee;
			this.IsAck = IsAck;
			MessageIndices = new uint[] { };
			IsUpdated = false;

			notifyTerminatedTrigger= new StateMachine<States, Transaction.States>.TriggerWithParameters<Transaction>(Transaction.States.NotifyTerminated);

			fsm = new StateMachine<States, Transaction.States>(InitialState);
			fsm.Configure(States.OnHook)
				.Permit(Transaction.States.InviteStarted, States.Calling)
				;
			fsm.Configure(States.Calling)
				.Ignore(Transaction.States.InviteStarted)
				.PermitReentry(Transaction.States.InviteProceeding)
				.Permit(Transaction.States.InviteRinging, States.Ringing)
				.Permit(Transaction.States.InviteTerminated, States.Established)
				;
			fsm.Configure(States.Ringing)
				.PermitReentry(Transaction.States.InviteRinging)
				.Ignore(Transaction.States.InviteProceeding)
				.Permit(Transaction.States.InviteError,States.Error)
				.Permit(Transaction.States.InviteTerminated, States.Established)
				;

			fsm.Configure(States.Established)
				.Ignore(Transaction.States.InviteStarted)   // réinvite
				.Ignore(Transaction.States.InviteProceeding)
				.Ignore(Transaction.States.InviteRinging)
				.Ignore(Transaction.States.InviteError)
				.Ignore(Transaction.States.InviteTerminated)

				.PermitReentry(Transaction.States.AckTerminated)

				.Ignore(Transaction.States.ReferStarted)
				.Ignore(Transaction.States.ReferProceeding)
				.Permit(Transaction.States.ReferTerminated, States.Transfering)

				.Ignore(Transaction.States.ByeStarted)
				.Ignore(Transaction.States.ByeProceeding)
				.Permit(Transaction.States.ByeTerminated, States.Terminated)

				.OnEntryFrom(Transaction.States.AckTerminated, () => {this.IsAck = true;})
				;

			fsm.Configure(States.Transfering)
				.Ignore(Transaction.States.InviteStarted)   // réinvite
				.Ignore(Transaction.States.InviteProceeding)
				.Ignore(Transaction.States.InviteRinging)
				.Ignore(Transaction.States.InviteError)
				.Ignore(Transaction.States.InviteTerminated)

				.Ignore(Transaction.States.AckTerminated)

				.Ignore(Transaction.States.NotifyStarted)
				.Ignore(Transaction.States.NotifyProceeding)
				.PermitIf(notifyTerminatedTrigger, States.Transfered, AssertTransfertIsCompleted)
				.IgnoreIf(notifyTerminatedTrigger,(t)=> {return !AssertTransfertIsCompleted(t); })

				.Ignore(Transaction.States.ByeStarted)
				.Ignore(Transaction.States.ByeProceeding)
				.Permit(Transaction.States.ByeTerminated, States.Terminated)
				.OnEntry(CheckReplacedCallID)
				;

			fsm.Configure(States.Transfered)
				.Ignore(Transaction.States.InviteStarted)   // réinvite
				.Ignore(Transaction.States.InviteProceeding)
				.Ignore(Transaction.States.InviteRinging)
				.Ignore(Transaction.States.InviteError)
				.Ignore(Transaction.States.InviteTerminated)

				.Ignore(Transaction.States.AckTerminated)

				.Ignore(Transaction.States.NotifyStarted)
				.Ignore(Transaction.States.NotifyProceeding)
				.Ignore(Transaction.States.NotifyTerminated)

				.Ignore(Transaction.States.ByeStarted)
				.Ignore(Transaction.States.ByeProceeding)
				.Permit(Transaction.States.ByeTerminated, States.Terminated)
				;

			fsm.Configure(States.Error)
				.Ignore(Transaction.States.AckTerminated)
				;
			/*fsm.Configure(States.Terminated)
				.OnEntry(() =>
					{
						if (CallID == "SDo2pg901-c155e675bb01dc8ff7638df0e6c6c83f-v300g00")
						{
							int t = 0;
						}
					}
				)
				;*/
		}

		

		private bool AssertTransfertIsCompleted(Transaction Transaction)
		{
			NotifyTransaction? transaction;

			transaction=Transaction as NotifyTransaction;
			if (transaction == null) return false;
			return transaction.IsTransfertCompleted;
		}

		private void CheckReplacedCallID(StateMachine<States, Transaction.States>.Transition Transition)
		{
			ReferTransaction? transaction;

			if (Transition.Parameters.Length == 0) return;

			transaction = Transition.Parameters[0] as ReferTransaction;
			if (transaction== null) return;

			this.ReplacedCallID = transaction.ReplacedCallID;


		}


		public Call Clone()
		{
			Call newCall;

			newCall = new Call(this.CallID, this.SourceDevice, this.DestinationDevice,this.FromTag, this.Caller, this.Callee, this.State,this.IsAck);
			newCall.MessageIndices = (uint[])this.MessageIndices.Clone();
			newCall.ReplacedCallID=this.ReplacedCallID;
			newCall.ToTag=this.ToTag; 


			return newCall;
						
		}
		
		public bool Match(Request Request)
		{
			return (CallID == Request.GetCallID()) && 
				( (this.FromTag==Request.GetFromTag() || (this.FromTag == Request.GetToTag() )) );
		}
		public bool Match(Response Response)
		{
			return (CallID == Response.GetCallID()) &&
				((this.FromTag == Response.GetFromTag() || (this.FromTag == Response.GetToTag())));
		}



		public bool Update(Transaction Transaction)
		{
			bool oldAck;
			States oldState;
			StateMachine<States, Transaction.States>.TriggerWithParameters<Transaction.States, Transaction> trigger;


			oldAck = IsAck;
			oldState = State;

			trigger = new StateMachine<States, Transaction.States>.TriggerWithParameters<Transaction.States, Transaction>(Transaction.State);


			fsm.Fire(trigger,Transaction);
			this.MessageIndices = Transaction.MessagesIndices.ToArray();

			IsUpdated= (oldAck != IsAck) || (oldState != State);
			
			return IsUpdated;

		}
		

	}
}
