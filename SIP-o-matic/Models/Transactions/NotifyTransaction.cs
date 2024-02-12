﻿using SIPParserLib;
using Stateless.Graph;
using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace SIP_o_matic.Models.Transactions
{
	public class NotifyTransaction:Transaction
	{

	

		private StateMachine<States, Triggers>.TriggerWithParameters<Response>? Prov1xxTrigger;
		private StateMachine<States, Triggers>.TriggerWithParameters<Response>? Final2xxTrigger;
		private StateMachine<States, Triggers>.TriggerWithParameters<Response>? ErrorTrigger;



		protected override States TerminatedState => States.NotifyTerminated;


		[SetsRequiredMembers]
		public NotifyTransaction(string CallID,string ViaBranch,string CSeq, States InitialState) :base(CallID,ViaBranch,CSeq, InitialState)
		{

		}

		protected override void OnConfigureFSM(StateMachine<States, Triggers> fsm)
		{
			// Undefined => Transfering => Proceeding => Terminated

			Prov1xxTrigger = fsm.SetTriggerParameters<Response>(Triggers.Prov1xx);
			Final2xxTrigger = fsm.SetTriggerParameters<Response>(Triggers.Final2xx);
			ErrorTrigger = fsm.SetTriggerParameters<Response>(Triggers.Error);


			fsm.Configure(States.Undefined)
				.PermitIf(NotifyTrigger, States.NotifyStarted, (Request) => AssertMessageBelongsToTransaction(Request), TransactionErrorMessage)
				;

			fsm.Configure(States.NotifyStarted)
				.PermitIf(ErrorTrigger, States.NotifyError, (Response) => AssertMessageBelongsToTransaction(Response), TransactionErrorMessage)
				.PermitReentryIf(NotifyTrigger, (Response) => AssertMessageBelongsToTransaction(Response), TransactionErrorMessage).OnEntry(() => Retransmissions++)
				.PermitIf(Prov1xxTrigger, States.NotifyProceeding, (Response) => AssertMessageBelongsToTransaction(Response), TransactionErrorMessage)
				.PermitIf(Final2xxTrigger, States.NotifyTerminated, (Response) => AssertMessageBelongsToTransaction(Response), TransactionErrorMessage)
				;

			fsm.Configure(States.NotifyProceeding)
				.PermitIf(ErrorTrigger, States.NotifyError, (Response) => AssertMessageBelongsToTransaction(Response), TransactionErrorMessage)
				.PermitReentryIf(Prov1xxTrigger, (Response) => AssertMessageBelongsToTransaction(Response), TransactionErrorMessage)
				.PermitIf(Final2xxTrigger, States.NotifyTerminated, (Response) => AssertMessageBelongsToTransaction(Response), TransactionErrorMessage)
				;
		}

		public override Transaction Clone()
		{
			return new NotifyTransaction(CallID, ViaBranch, CSeq, State);
		}
		
		
		protected override StateMachine<States, Triggers>.TriggerWithParameters<Response> OnGetUpdateTrigger(Response Response)
		{
			switch (Response.StatusLine.StatusCode)
			{
				case 100:return Prov1xxTrigger!;
				case 200: case 202:return Final2xxTrigger!;
				case >= 400 and < 699: return ErrorTrigger!;
				default: throw new InvalidOperationException($"Unsupported transaction transition ({Response.StatusLine.StatusCode})");
			}
		}


	}
}
