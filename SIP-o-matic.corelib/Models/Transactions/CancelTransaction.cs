﻿using SIPParserLib;
using Stateless.Graph;
using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace SIP_o_matic.corelib.Models.Transactions
{
	public class CancelTransaction:Transaction
	{

	

		private StateMachine<States, Triggers>.TriggerWithParameters<Response>? Prov1xxTrigger;
		private StateMachine<States, Triggers>.TriggerWithParameters<Response>? Final2xxTrigger;
		private StateMachine<States, Triggers>.TriggerWithParameters<Response>? ErrorTrigger;



		protected override States TerminatedState => States.CancelTerminated;


		[SetsRequiredMembers]
		public CancelTransaction(string CallID,  string ViaBranch,string CSeq) :base(CallID, ViaBranch,CSeq)
		{

		}

		protected override void OnConfigureFSM(StateMachine<States, Triggers> fsm)
		{
			// Undefined => Transfering => Proceeding => Terminated

			Prov1xxTrigger = fsm.SetTriggerParameters<Response>(Triggers.Prov1xx);
			Final2xxTrigger = fsm.SetTriggerParameters<Response>(Triggers.Final2xx);
			ErrorTrigger = fsm.SetTriggerParameters<Response>(Triggers.Error);


			fsm.Configure(States.Undefined)
				.PermitIf(CancelTrigger, States.CancelStarted,AssertMessageBelongsToTransaction, TransactionErrorMessage)
				;

			fsm.Configure(States.CancelStarted)
				.PermitIf(ErrorTrigger, States.CancelError, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				.PermitIf(Prov1xxTrigger, States.CancelProceeding, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				.PermitIf(Final2xxTrigger, States.CancelTerminated, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				;

			fsm.Configure(States.CancelProceeding)
				.PermitIf(ErrorTrigger, States.CancelError, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				.PermitReentryIf(Prov1xxTrigger, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				.PermitIf(Final2xxTrigger, States.CancelTerminated, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				;
		}

		
		
		protected override StateMachine<States, Triggers>.TriggerWithParameters<Response> OnGetUpdateTrigger(Response Response)
		{
			switch (Response.StatusLine.StatusCode)
			{
				case 100:return Prov1xxTrigger!;
				case 200:return Final2xxTrigger!;
				case >= 400 and < 699: return ErrorTrigger!;
				default: throw new InvalidOperationException($"Unsupported transaction transition ({Response.StatusLine.StatusCode})");
			}
		}


	}
}
