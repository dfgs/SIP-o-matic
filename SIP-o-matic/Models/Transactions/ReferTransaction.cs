using SIPParserLib;
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
	public class ReferTransaction:Transaction
	{

	

		private StateMachine<States, Triggers>.TriggerWithParameters<Response>? Prov1xxTrigger;
		private StateMachine<States, Triggers>.TriggerWithParameters<Response>? Final2xxTrigger;
		private StateMachine<States, Triggers>.TriggerWithParameters<Response>? ErrorTrigger;



		protected override States TerminatedState => States.ReferTerminated;


		[SetsRequiredMembers]
		public ReferTransaction(string CallID,string ViaBranch,string CSeq, States InitialState) :base(CallID,ViaBranch,CSeq,InitialState)
		{

		}

		protected override void OnConfigureFSM(StateMachine<States, Triggers> fsm)
		{
			// Undefined => Transfering => Proceeding => Terminated

			Prov1xxTrigger = fsm.SetTriggerParameters<Response>(Triggers.Prov1xx);
			Final2xxTrigger = fsm.SetTriggerParameters<Response>(Triggers.Final2xx);
			ErrorTrigger = fsm.SetTriggerParameters<Response>(Triggers.Error);


			fsm.Configure(States.Undefined)
				.PermitIf(ReferTrigger, States.ReferStarted, (Request) => AssertMessageBelongsToTransaction(Request), TransactionErrorMessage)
				;

			fsm.Configure(States.ReferStarted)
				.PermitIf(ErrorTrigger, States.ReferError, (Response) => AssertMessageBelongsToTransaction(Response), TransactionErrorMessage)
				.PermitIf(Prov1xxTrigger, States.ReferProceeding, (Response) => AssertMessageBelongsToTransaction(Response), TransactionErrorMessage)
				.PermitIf(Final2xxTrigger, States.ReferTerminated, (Response) => AssertMessageBelongsToTransaction(Response), TransactionErrorMessage)
				;

			fsm.Configure(States.ReferProceeding)
				.PermitIf(ErrorTrigger, States.ReferError, (Response) => AssertMessageBelongsToTransaction(Response), TransactionErrorMessage)
				.PermitReentryIf(Prov1xxTrigger, (Response) => AssertMessageBelongsToTransaction(Response), TransactionErrorMessage)
				.PermitIf(Final2xxTrigger, States.ReferTerminated, (Response) => AssertMessageBelongsToTransaction(Response), TransactionErrorMessage)
				;
		}

		public override Transaction Clone()
		{
			return new ReferTransaction(CallID, ViaBranch, CSeq, State);
		}
		

	

		protected override StateMachine<States, Triggers>.TriggerWithParameters<Response> OnGetUpdateTrigger(Response Response)
		{
			switch (Response.StatusLine.StatusCode)
			{
				case 100:return Prov1xxTrigger!;
				case 202:return Final2xxTrigger!;
				case >= 400 and < 699: return ErrorTrigger!;
				default: throw new InvalidOperationException($"Unsupported transaction transition ({Response.StatusLine.StatusCode})");
			}
		}


	}
}
