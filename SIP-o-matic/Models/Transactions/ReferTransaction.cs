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

		private StateMachine<States, Triggers> fsm;
	
		private StateMachine<States, Triggers>.TriggerWithParameters<Request> REFERTrigger;

		private StateMachine<States, Triggers>.TriggerWithParameters<Response> Prov1xxTrigger;
		private StateMachine<States, Triggers>.TriggerWithParameters<Response> Final2xxTrigger;
		private StateMachine<States, Triggers>.TriggerWithParameters<Response> ErrorTrigger;


		public override States State => fsm.State;

		public override bool IsTerminated => fsm.IsInState(States.Terminated);


		[SetsRequiredMembers]
		public ReferTransaction(string CallID,string ViaBranch,string CSeq, States InitialState) :base(CallID,ViaBranch,CSeq)
		{
			fsm = new StateMachine<States, Triggers>(InitialState);

			// Undefined => Transfering => Proceeding => Terminated

			REFERTrigger = fsm.SetTriggerParameters<Request>(Triggers.INVITE);
			Prov1xxTrigger = fsm.SetTriggerParameters<Response>(Triggers.Prov1xx);
			Final2xxTrigger = fsm.SetTriggerParameters<Response>(Triggers.Final2xx);
			ErrorTrigger = fsm.SetTriggerParameters<Response>(Triggers.Error);


			fsm.Configure(States.Undefined)
				.PermitIf(REFERTrigger, States.Transfering, (Request) => AssertMessageBelongsToTransaction(Request),"Message doesn't belong to current transaction")
				;

			fsm.Configure(States.Transfering)
				.PermitIf(Prov1xxTrigger, States.Proceeding, (Response) => AssertMessageBelongsToTransaction(Response), "Message doesn't belong to current transaction")
				.PermitIf(Final2xxTrigger, States.Terminated, (Response) => AssertMessageBelongsToTransaction(Response), "Message doesn't belong to current transaction")
				;

			fsm.Configure(States.Proceeding)
				.PermitReentryIf(Prov1xxTrigger, (Response) => AssertMessageBelongsToTransaction(Response), "Message doesn't belong to current transaction")
				.PermitIf(Final2xxTrigger, States.Terminated, (Response) => AssertMessageBelongsToTransaction(Response), "Message doesn't belong to current transaction")
				;


		}

		public override Transaction Clone()
		{
			return new ReferTransaction(CallID, ViaBranch, CSeq, fsm.State);
		}
		public override string GetGraph()
		{
			return  UmlDotGraph.Format(fsm.GetInfo());
		}

		public override void Update(Request Request)
		{
			switch (Request.RequestLine.Method)
			{
				case "REFER":
					fsm.Fire(REFERTrigger, Request);
					break;
				default: throw new InvalidOperationException($"Unsupported transaction transition ({Request.RequestLine.Method})");
			}
		}

		public override void Update(Response Response)
		{
			switch (Response.StatusLine.StatusCode)
			{
				case 100:
					fsm.Fire(Prov1xxTrigger, Response);
					break;
				case 202:
					fsm.Fire(Final2xxTrigger, Response);
					break;
				default: throw new InvalidOperationException($"Unsupported transaction transition ({Response.StatusLine.StatusCode})");
			}
		}


	}
}
