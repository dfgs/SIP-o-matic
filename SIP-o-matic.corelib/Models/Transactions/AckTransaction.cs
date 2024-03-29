﻿using SIPParserLib;
using Stateless;
using Stateless.Graph;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SIP_o_matic.corelib.Models.Transactions
{
	public class AckTransaction : Transaction
    {

		protected override States TerminatedState => States.AckTerminated;


		[SetsRequiredMembers]
        public AckTransaction(string CallID, string ViaBranch, string CSeq) : base(CallID,ViaBranch, CSeq)
        {

		}

		protected override void OnConfigureFSM(StateMachine<States, Triggers> fsm)
		{
			fsm.Configure(States.Undefined)
				.PermitIf(AckTrigger, States.AckTerminated,AssertMessageBelongsToTransaction, TransactionErrorMessage)
				;
		}

		

		protected override StateMachine<States, Triggers>.TriggerWithParameters<Response> OnGetUpdateTrigger(Response Response)
		{
			throw new InvalidOperationException($"Unsupported transaction transition ({Response.StatusLine.StatusCode})");
		}
	}
}