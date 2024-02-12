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
				.PermitIf(InviteTrigger, States.InviteStarted, (Request) => AssertMessageBelongsToTransaction(Request), "Message doesn't belong to current transaction")
				;

			fsm.Configure(States.InviteStarted)
				.PermitIf(Prov1xxTrigger, States.InviteProceeding, (Response) => AssertMessageBelongsToTransaction(Response), "Message doesn't belong to current transaction")
				.PermitIf(Prov180Trigger, States.InviteRinging, (Response) => AssertMessageBelongsToTransaction(Response), "Message doesn't belong to current transaction")
				.PermitIf(Final2xxTrigger, States.InviteTerminated, (Response) => AssertMessageBelongsToTransaction(Response), "Message doesn't belong to current transaction")
				;

			fsm.Configure(States.InviteProceeding)
				.PermitReentryIf(Prov1xxTrigger, (Response) => AssertMessageBelongsToTransaction(Response), "Message doesn't belong to current transaction")
				.PermitIf(Prov180Trigger, States.InviteRinging, (Response) => AssertMessageBelongsToTransaction(Response), "Message doesn't belong to current transaction")
				.PermitIf(Final2xxTrigger, States.InviteTerminated, (Response) => AssertMessageBelongsToTransaction(Response), "Message doesn't belong to current transaction")
				;

			fsm.Configure(States.InviteRinging)
				.SubstateOf(States.InviteProceeding)
				.PermitIf(Final2xxTrigger, States.InviteTerminated, (Response) => AssertMessageBelongsToTransaction(Response), "Message doesn't belong to current transaction")
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
				default: throw new InvalidOperationException($"Unsupported transaction transition ({Response.StatusLine.StatusCode})");
			}
		}


	}
}
