﻿using SIP_o_matic.Models.Transactions;
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
		public  enum States { OnHook,Calling, Ringing,Established,Transfering,Terminated};

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

				.PermitReentry(Transaction.States.AckTerminated).OnEntry(() => { IsAck = true; })

				.Ignore(Transaction.States.ReferStarted)
				.Ignore(Transaction.States.ReferProceeding)
				.Permit(Transaction.States.ReferTerminated, States.Transfering)

				.Ignore(Transaction.States.ByeStarted)
				.Ignore(Transaction.States.ByeProceeding)
				.Permit(Transaction.States.ByeTerminated, States.Terminated)
				;

			fsm.Configure(States.Transfering)
				.Ignore(Transaction.States.NotifyStarted)
				.Ignore(Transaction.States.NotifyProceeding)
				.Ignore(Transaction.States.NotifyTerminated)

				.Ignore(Transaction.States.ByeStarted)
				.Ignore(Transaction.States.ByeProceeding)

				.Permit(Transaction.States.ByeTerminated, States.Terminated)
				;

			fsm.Configure(States.Terminated)
				.OnEntry(() =>
					{
						if (CallID == "SDo2pg901-c155e675bb01dc8ff7638df0e6c6c83f-v300g00")
						{
							int t = 0;
						}
					}
				)
				;
		}

		public Call Clone()
		{
			return new Call(this.CallID, this.SourceAddress, this.DestinationAddress,this.FromURI, this.ToURI, this.State,this.IsAck);
						
		}
		private bool SourceAndDestinationMatch(string SourceAddress, string DestinationAddress)
		{
			return ((this.SourceAddress == SourceAddress) && (this.DestinationAddress == DestinationAddress))
				|| ((this.SourceAddress == DestinationAddress) && (this.DestinationAddress == SourceAddress));
		}
		public bool Match(Request Request, string SourceAddress, string DestinationAddress)
		{
			return (CallID == Request.GetHeader<CallIDHeader>()?.Value) && SourceAndDestinationMatch(SourceAddress,DestinationAddress) ;
		}
		public bool Match(Response Response, string SourceAddress, string DestinationAddress)
		{
			return (CallID == Response.GetHeader<CallIDHeader>()?.Value) && SourceAndDestinationMatch(SourceAddress, DestinationAddress);
		}



		public void Update(Transaction Transaction)
		{
			fsm.Fire(Transaction.State);
		}
		

	}
}
