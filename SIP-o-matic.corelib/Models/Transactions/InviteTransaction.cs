using SIPParserLib;
using Stateless.Graph;
using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;

namespace SIP_o_matic.corelib.Models.Transactions
{
	public class InviteTransaction:Transaction
	{

	

		private StateMachine<States, Triggers>.TriggerWithParameters<IResponse>? Prov1xxTrigger;
		private StateMachine<States, Triggers>.TriggerWithParameters<IResponse>? Prov180Trigger;
		private StateMachine<States, Triggers>.TriggerWithParameters<IResponse>? Final2xxTrigger;
		private StateMachine<States, Triggers>.TriggerWithParameters<IResponse>? ErrorTrigger;


		protected override States TerminatedState => States.InviteTerminated;


		[SetsRequiredMembers]
		public InviteTransaction(string CallID,   string ViaBranch, string CSeq) : base(CallID, ViaBranch, CSeq)
		{

		}

		protected override void OnConfigureFSM(StateMachine<States, Triggers> fsm)
		{
			// Undefined => Calling => Proceeding => Ringing => Terminated

			Prov1xxTrigger = fsm.SetTriggerParameters<IResponse>(Triggers.Prov1xx);
			Prov180Trigger = fsm.SetTriggerParameters<IResponse>(Triggers.Prov180);
			Final2xxTrigger = fsm.SetTriggerParameters<IResponse>(Triggers.Final2xx);
			ErrorTrigger = fsm.SetTriggerParameters<IResponse>(Triggers.Error);

			fsm.Configure(States.Undefined)
				.PermitIf(InviteTrigger, States.InviteStarted,AssertMessageBelongsToTransaction, TransactionErrorMessage)
				;

			fsm.Configure(States.InviteStarted)
				.PermitIf(ErrorTrigger, States.InviteError, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				.PermitIf(Prov1xxTrigger, States.InviteProceeding, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				.PermitIf(Prov180Trigger, States.InviteRinging, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				.PermitIf(Final2xxTrigger, States.InviteTerminated, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				;

			fsm.Configure(States.InviteProceeding)
				.PermitIf(ErrorTrigger, States.InviteError, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				.PermitReentryIf(Prov1xxTrigger, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				.PermitIf(Prov180Trigger, States.InviteRinging, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				.PermitIf(Final2xxTrigger, States.InviteTerminated, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				;

			fsm.Configure(States.InviteRinging)
				.PermitIf(ErrorTrigger, States.InviteError, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				.SubstateOf(States.InviteProceeding)
				.PermitIf(Final2xxTrigger, States.InviteTerminated, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				;

			fsm.Configure(States.InviteError)
				.PermitIf(Prov1xxTrigger, States.InviteProceeding, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				.PermitIf(Prov180Trigger, States.InviteRinging, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				.PermitIf(Final2xxTrigger, States.InviteTerminated, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				;


		}



		protected override StateMachine<States, Triggers>.TriggerWithParameters<IResponse> OnGetUpdateTrigger(IResponse Response)
		{
			switch (Response.StatusCode)
			{
				case 180: return Prov180Trigger!;
				case >= 100 and <= 199:return Prov1xxTrigger!;
				case >= 200 and <= 299:return Final2xxTrigger!;
				case >= 400 and < 699: return ErrorTrigger!;
				default: throw new InvalidOperationException($"Unsupported transaction transition ({Response.StatusCode})");
			}
		}


	}
}
