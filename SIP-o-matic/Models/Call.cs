using SIP_o_matic.Models.Transactions;
using SIPParserLib;
using Stateless;
using Stateless.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static SIP_o_matic.Models.Transactions.Transaction;

namespace SIP_o_matic.Models
{
	public class Call:ICloneable<Call>,ISIPMessageMatch
	{
		public  enum States { OnHook,Calling, Ringing,Established,Transfering};

		private StateMachine<States, Transaction.States> fsm;

		public required string CallID
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

		public required Address FromURI
		{
			get;
			set;
		}
		public required Address ToURI
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

		
		[SetsRequiredMembers]
		public Call(string callID, string SourceAddress,string DestinationAddress, Address FromURI, Address ToURI, States InitialState,bool IsAck)
		{
	
			CallID = callID;
			this.SourceAddress = SourceAddress;
			this.DestinationAddress = DestinationAddress;
			this.FromURI = FromURI;
			this.ToURI = ToURI;
			this.IsAck = IsAck;

			fsm = new StateMachine<States, Transaction.States>(InitialState);
			fsm.Configure(States.OnHook)
				.Permit(Transaction.States.InviteStarted, States.Calling)
				;
			fsm.Configure(States.Calling)
				.PermitReentry(Transaction.States.InviteProceeding)
				.Permit(Transaction.States.InviteRinging, States.Ringing)
				.Permit(Transaction.States.AckTerminated, States.Established)
				;
			fsm.Configure(States.Ringing)
				.PermitReentry(Transaction.States.InviteRinging)
				.Ignore(Transaction.States.InviteTerminated)
				.Permit(Transaction.States.AckTerminated, States.Established)
				;
			fsm.Configure(States.Established)
				.Ignore(Transaction.States.InviteStarted)
				.Ignore(Transaction.States.InviteProceeding)
				.Ignore(Transaction.States.InviteRinging)
				.Ignore(Transaction.States.InviteError)
				.Ignore(Transaction.States.InviteTerminated)
				
				.Ignore(Transaction.States.AckTerminated)

				.Ignore(Transaction.States.ReferStarted)
				.Ignore(Transaction.States.ReferProceeding)
				.Permit(Transaction.States.ReferTerminated, States.Transfering)
				;
			fsm.Configure(States.Transfering)
				.Ignore(Transaction.States.NotifyStarted)
				.Ignore(Transaction.States.NotifyProceeding)
				.Ignore(Transaction.States.NotifyTerminated)
				;
		}

		public Call Clone()
		{
			return new Call(this.CallID, this.SourceAddress, this.DestinationAddress,this.FromURI, this.ToURI, this.State,this.IsAck);
						
		}

		public bool Match(Request Request)
		{
			return (CallID == Request.GetHeader<CallIDHeader>()?.Value) ;
		}
		public bool Match(Response Response)
		{
			return (CallID == Response.GetHeader<CallIDHeader>()?.Value) ;
		}



		public void Update(Transaction Transaction)
		{
			fsm.Fire(Transaction.State);
		}
		

	}
}
