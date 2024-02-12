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
	public class ByeTransaction:Transaction
	{

	

		private StateMachine<States, Triggers>.TriggerWithParameters<Response, string, string>? Prov1xxTrigger;
		private StateMachine<States, Triggers>.TriggerWithParameters<Response, string, string>? Final2xxTrigger;
		private StateMachine<States, Triggers>.TriggerWithParameters<Response, string, string>? ErrorTrigger;



		protected override States TerminatedState => States.ByeTerminated;


		[SetsRequiredMembers]
		public ByeTransaction(string CallID, string SourceAddress, string DestinationAddress, string ViaBranch,string CSeq) :base(CallID,SourceAddress,DestinationAddress, ViaBranch,CSeq)
		{

		}

		protected override void OnConfigureFSM(StateMachine<States, Triggers> fsm)
		{
			// Undefined => Transfering => Proceeding => Terminated

			Prov1xxTrigger = fsm.SetTriggerParameters<Response, string, string>(Triggers.Prov1xx);
			Final2xxTrigger = fsm.SetTriggerParameters<Response, string, string>(Triggers.Final2xx);
			ErrorTrigger = fsm.SetTriggerParameters<Response, string, string>(Triggers.Error);


			fsm.Configure(States.Undefined)
				.PermitIf(ByeTrigger, States.ByeStarted,AssertMessageBelongsToTransaction, TransactionErrorMessage)
				;

			fsm.Configure(States.ByeStarted)
				.PermitIf(ErrorTrigger, States.ByeError, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				.PermitIf(Prov1xxTrigger, States.ByeProceeding, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				.PermitIf(Final2xxTrigger, States.ByeTerminated, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				;

			fsm.Configure(States.ByeProceeding)
				.PermitIf(ErrorTrigger, States.ByeError, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				.PermitReentryIf(Prov1xxTrigger, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				.PermitIf(Final2xxTrigger, States.ByeTerminated, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				;
		}

		
		
		protected override StateMachine<States, Triggers>.TriggerWithParameters<Response, string, string> OnGetUpdateTrigger(Response Response)
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
