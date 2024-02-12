﻿using SIPParserLib;
using Stateless.Graph;
using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Windows.Data;

namespace SIP_o_matic.Models.Transactions
{
	public class InviteTransaction:Transaction
	{

	

		private StateMachine<States, Triggers>.TriggerWithParameters<Response>? Prov1xxTrigger;
		private StateMachine<States, Triggers>.TriggerWithParameters<Response>? Prov180Trigger;
		private StateMachine<States, Triggers>.TriggerWithParameters<Response>? Final2xxTrigger;
		private StateMachine<States, Triggers>.TriggerWithParameters<Response>? ErrorTrigger;


		protected override States TerminatedState => States.InviteTerminated;


		[SetsRequiredMembers]
		public InviteTransaction(string CallID,string ViaBranch,string CSeq, States InitialState) :base(CallID,ViaBranch,CSeq, InitialState)
		{

		}

		protected override void OnConfigureFSM(StateMachine<States, Triggers> fsm)
		{
			// Undefined => Calling => Proceeding => Ringing => Terminated

			Prov1xxTrigger = fsm.SetTriggerParameters<Response>(Triggers.Prov1xx);
			Prov180Trigger = fsm.SetTriggerParameters<Response>(Triggers.Prov180);
			Final2xxTrigger = fsm.SetTriggerParameters<Response>(Triggers.Final2xx);
			ErrorTrigger = fsm.SetTriggerParameters<Response>(Triggers.Error);

			fsm.Configure(States.Undefined)
				.PermitIf(InviteTrigger, States.InviteStarted, (Request) => AssertMessageBelongsToTransaction(Request), TransactionErrorMessage)
				;

			fsm.Configure(States.InviteStarted)
				.PermitIf(ErrorTrigger, States.InviteError, (Response) => AssertMessageBelongsToTransaction(Response), TransactionErrorMessage)
				.PermitReentryIf(InviteTrigger, (Response) => AssertMessageBelongsToTransaction(Response), TransactionErrorMessage).OnEntry(()=>Retransmissions++) 
				.PermitIf(Prov1xxTrigger, States.InviteProceeding, (Response) => AssertMessageBelongsToTransaction(Response), TransactionErrorMessage)
				.PermitIf(Prov180Trigger, States.InviteRinging, (Response) => AssertMessageBelongsToTransaction(Response), TransactionErrorMessage)
				.PermitIf(Final2xxTrigger, States.InviteTerminated, (Response) => AssertMessageBelongsToTransaction(Response), TransactionErrorMessage)
				;

			fsm.Configure(States.InviteProceeding)
				.PermitIf(ErrorTrigger, States.InviteError, (Response) => AssertMessageBelongsToTransaction(Response), TransactionErrorMessage)
				.PermitReentryIf(Prov1xxTrigger, (Response) => AssertMessageBelongsToTransaction(Response), TransactionErrorMessage)
				.PermitIf(Prov180Trigger, States.InviteRinging, (Response) => AssertMessageBelongsToTransaction(Response), TransactionErrorMessage)
				.PermitIf(Final2xxTrigger, States.InviteTerminated, (Response) => AssertMessageBelongsToTransaction(Response), TransactionErrorMessage)
				;

			fsm.Configure(States.InviteRinging)
				.PermitIf(ErrorTrigger, States.InviteError, (Response) => AssertMessageBelongsToTransaction(Response), TransactionErrorMessage)
				.SubstateOf(States.InviteProceeding)
				.PermitIf(Final2xxTrigger, States.InviteTerminated, (Response) => AssertMessageBelongsToTransaction(Response), TransactionErrorMessage)
				;

		}

		public override Transaction Clone()
		{
			return new InviteTransaction(CallID, ViaBranch, CSeq, State);
		}
		
		protected override StateMachine<States, Triggers>.TriggerWithParameters<Response> OnGetUpdateTrigger(Response Response)
		{
			switch (Response.StatusLine.StatusCode)
			{
				case 180: return Prov180Trigger!;
				case >= 100 and <= 199:return Prov1xxTrigger!;
				case >= 200 and <= 299:return Final2xxTrigger!;
				case >= 400 and < 699: return ErrorTrigger!;
				default: throw new InvalidOperationException($"Unsupported transaction transition ({Response.StatusLine.StatusCode})");
			}
		}


	}
}
