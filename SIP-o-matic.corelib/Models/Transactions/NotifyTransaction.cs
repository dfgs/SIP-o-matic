using SIPParserLib;
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
	public class NotifyTransaction:Transaction
	{

	

		private StateMachine<States, Triggers>.TriggerWithParameters<Response>? Prov1xxTrigger;
		private StateMachine<States, Triggers>.TriggerWithParameters<Response>? Final2xxTrigger;
		private StateMachine<States, Triggers>.TriggerWithParameters<Response>? ErrorTrigger;

		public bool IsTransfertCompleted
		{
			get;
			set;
		}

		protected override States TerminatedState => States.NotifyTerminated;


		[SetsRequiredMembers]
		public NotifyTransaction(string CallID, string ViaBranch, string CSeq) : base(CallID, ViaBranch, CSeq)
		{

		}

		protected override void OnConfigureFSM(StateMachine<States, Triggers> fsm)
		{
			// Undefined => Transfering => Proceeding => Terminated

			Prov1xxTrigger = fsm.SetTriggerParameters<Response>(Triggers.Prov1xx);
			Final2xxTrigger = fsm.SetTriggerParameters<Response>(Triggers.Final2xx);
			ErrorTrigger = fsm.SetTriggerParameters<Response>(Triggers.Error);


			fsm.Configure(States.Undefined)
				.PermitIf(NotifyTrigger, States.NotifyStarted, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				;

			fsm.Configure(States.NotifyStarted)
				.PermitIf(ErrorTrigger, States.NotifyError, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				.PermitIf(Prov1xxTrigger, States.NotifyProceeding,  AssertMessageBelongsToTransaction, TransactionErrorMessage)
				.PermitIf(Final2xxTrigger, States.NotifyTerminated, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				.OnEntry(CheckIfTransfertIsCompleted);
			;

			fsm.Configure(States.NotifyProceeding)
				.PermitIf(ErrorTrigger, States.NotifyError, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				.PermitReentryIf(Prov1xxTrigger,AssertMessageBelongsToTransaction, TransactionErrorMessage)
				.PermitIf(Final2xxTrigger, States.NotifyTerminated, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				;
				
		}










		private void CheckIfTransfertIsCompleted(StateMachine<States, Triggers>.Transition Transition)
		{
			Request? request;
			ContentLengthHeader? contentLengthHeader;

			if (Transition.Parameters.Length == 0) return;

			request = Transition.Parameters[0] as Request;
			if (request==null) return;

			contentLengthHeader = request.GetHeader<ContentLengthHeader>();
			if (contentLengthHeader == null) return;

			if (contentLengthHeader.Value == "0") return;

			IsTransfertCompleted = request.Body == "SIP/2.0 200 OK\r\n";
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
