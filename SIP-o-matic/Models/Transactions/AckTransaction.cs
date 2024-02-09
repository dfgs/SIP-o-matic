using SIPParserLib;
using Stateless;
using Stateless.Graph;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SIP_o_matic.Models.Transactions
{
	public class AckTransaction : Transaction
    {
		private StateMachine<States, Triggers> fsm;

		private StateMachine<States, Triggers>.TriggerWithParameters<Request> ACKTrigger;


		public override States State => fsm.State;

		public override bool IsTerminated => fsm.IsInState(States.Terminated);



		[SetsRequiredMembers]
        public AckTransaction(string CallID, string ViaBranch, string CSeq, States InitialState) : base(CallID, ViaBranch, CSeq)
        {
			fsm = new StateMachine<States, Triggers>(InitialState);

			ACKTrigger = fsm.SetTriggerParameters<Request>(Triggers.ACK);
			fsm.Configure(States.Undefined)
				.PermitIf(ACKTrigger, States.Terminated, (Request) => AssertMessageBelongsToTransaction(Request), "Message doesn't belong to current transaction")
				;
		}



		public override Transaction Clone()
		{
			throw new System.NotImplementedException();
		}

		public override string GetGraph()
		{
			return UmlDotGraph.Format(fsm.GetInfo());
		}

		public override void Update(Request Request)
		{
			switch (Request.RequestLine.Method)
			{
				case "ACK":
					fsm.Fire(ACKTrigger, Request);
					break;
				default: throw new InvalidOperationException($"Unsupported transaction transition ({Request.RequestLine.Method})");
			}
		}

		public override void Update(Response Response)
		{
			throw new InvalidOperationException($"Unsupported transaction transition ({Response.StatusLine.StatusCode})");
		}
	}
}