using SIP_o_matic.Models.Transactions;
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
using static SIP_o_matic.Models.Transactions.Transaction;

namespace SIP_o_matic.Models
{
	public class Call:ICloneable<Call>,ISIPMessageMatch
	{
		public  enum States { OnHook,Calling, Ringing,Established, Transfering, Transfered, Terminated };

		private StateMachine<States, Transaction.States> fsm;

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

		[SetsRequiredMembers]
		public Call(string callID, string SourceDevice,string DestinationDevice, string Caller, string Callee, States InitialState,bool IsAck)
		{
	
			CallID = callID;
			this.SourceDevice = SourceDevice;
			this.DestinationDevice = DestinationDevice;
			this.Caller = Caller;
			this.Callee = Callee;
			this.IsAck = IsAck;
			MessageIndices = new uint[] { };

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
				.Permit(Transaction.States.NotifyTerminated,States.Transfered)

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

			newCall = new Call(this.CallID, this.SourceDevice, this.DestinationDevice,this.Caller, this.Callee, this.State,this.IsAck);
			newCall.MessageIndices = (uint[])this.MessageIndices.Clone();
			newCall.ReplacedCallID=this.ReplacedCallID;

			return newCall;
						
		}
		private bool SourceAndDestinationMatch(string SourceDevice, string DestinationDevice)
		{
			return ((this.SourceDevice == SourceDevice) && (this.DestinationDevice == DestinationDevice))
				|| ((this.SourceDevice == DestinationDevice) && (this.DestinationDevice == SourceDevice));
		}
		public bool Match(Request Request, string SourceDevice, string DestinationDevice)
		{
			return (CallID == Request.GetHeader<CallIDHeader>()?.Value) && SourceAndDestinationMatch(SourceDevice,DestinationDevice) ;
		}
		public bool Match(Response Response, string SourceDevice, string DestinationDevice)
		{
			return (CallID == Response.GetHeader<CallIDHeader>()?.Value) && SourceAndDestinationMatch(SourceDevice, DestinationDevice);
		}



		public void Update(Transaction Transaction)
		{
			StateMachine<States, Transaction.States>.TriggerWithParameters<Transaction.States, Transaction> trigger;

			trigger = new StateMachine<States, Transaction.States>.TriggerWithParameters<Transaction.States, Transaction>(Transaction.State);
			fsm.Fire(trigger,Transaction);
			this.MessageIndices = Transaction.MessagesIndices.ToArray();
		}
		

	}
}
