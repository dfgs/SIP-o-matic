using SIPParserLib;
using Stateless;
using Stateless.Graph;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SIP_o_matic.Models.Transactions
{
	public class AckTransaction : Transaction
    {



		protected override States TerminatedState => States.AckTerminated;


		[SetsRequiredMembers]
        public AckTransaction(string CallID, string ViaBranch, string CSeq, States InitialState) : base(CallID, ViaBranch, CSeq, InitialState)
        {

		}

		protected override void OnConfigureFSM(StateMachine<States, Triggers> fsm)
		{
			fsm.Configure(States.Undefined)
				.PermitIf(AckTrigger, States.AckTerminated, (Request) => AssertMessageBelongsToTransaction(Request), "Message doesn't belong to current transaction")
				;
		}

		public override Transaction Clone()
		{
			return new AckTransaction(CallID, ViaBranch, CSeq, State);
		}

		

	

		protected override StateMachine<States, Triggers>.TriggerWithParameters<Response> OnGetUpdateTrigger(Response Response)
		{
			throw new InvalidOperationException($"Unsupported transaction transition ({Response.StatusLine.StatusCode})");
		}
	}
}