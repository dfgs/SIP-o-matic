﻿using SIPParserLib;
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

		private StateMachine<States, Triggers> fsm;
	
		private StateMachine<States, Triggers>.TriggerWithParameters<Request> INVITETrigger;

		private StateMachine<States, Triggers>.TriggerWithParameters<Response> Prov1xxTrigger;
		private StateMachine<States, Triggers>.TriggerWithParameters<Response> Prov180Trigger;
		private StateMachine<States, Triggers>.TriggerWithParameters<Response> Final2xxTrigger;
		private StateMachine<States, Triggers>.TriggerWithParameters<Response> ErrorTrigger;


		public override States State => fsm.State;

		public override bool IsTerminated => fsm.IsInState(States.Terminated);


		[SetsRequiredMembers]
		public InviteTransaction(string CallID,string ViaBranch,string CSeq, States InitialState) :base(CallID,ViaBranch,CSeq)
		{
			fsm = new StateMachine<States, Triggers>(InitialState);

			// Undefined => Calling => Proceeding => Ringing => Terminated

			INVITETrigger = fsm.SetTriggerParameters<Request>(Triggers.INVITE);
			Prov1xxTrigger = fsm.SetTriggerParameters<Response>(Triggers.Prov1xx);
			Prov180Trigger = fsm.SetTriggerParameters<Response>(Triggers.Prov180);
			Final2xxTrigger = fsm.SetTriggerParameters<Response>(Triggers.Final2xx);
			ErrorTrigger = fsm.SetTriggerParameters<Response>(Triggers.Error);


			fsm.Configure(States.Undefined)
				.PermitIf(INVITETrigger, States.Calling, (Request) => AssertMessageBelongsToTransaction(Request),"Message doesn't belong to current transaction")
				;

			#region INVITE
			fsm.Configure(States.Calling)
				.PermitIf(Prov1xxTrigger, States.Proceeding, (Response) => AssertMessageBelongsToTransaction(Response), "Message doesn't belong to current transaction")
				.PermitIf(Prov180Trigger, States.Ringing, (Response) => AssertMessageBelongsToTransaction(Response), "Message doesn't belong to current transaction")
				.PermitIf(Final2xxTrigger, States.Terminated, (Response) => AssertMessageBelongsToTransaction(Response), "Message doesn't belong to current transaction")
				;

			fsm.Configure(States.Proceeding)
				.PermitReentryIf(Prov1xxTrigger, (Response) => AssertMessageBelongsToTransaction(Response), "Message doesn't belong to current transaction")
				.PermitIf(Prov180Trigger, States.Ringing, (Response) => AssertMessageBelongsToTransaction(Response), "Message doesn't belong to current transaction")
				.PermitIf(Final2xxTrigger, States.Terminated, (Response) => AssertMessageBelongsToTransaction(Response), "Message doesn't belong to current transaction")
				;

			fsm.Configure(States.Ringing)
				.SubstateOf(States.Proceeding)
				.PermitIf(Final2xxTrigger, States.Terminated, (Response) => AssertMessageBelongsToTransaction(Response), "Message doesn't belong to current transaction")
				;

			//fsm.Configure(States.Completed)
				//.PermitReentryIf(ACKTrigger, (Request) => CallIdIsValid(Request)).OnEntryFrom(Triggers.ACK, () => this.IsAck = true)
				//.PermitReentryIf(INVITETrigger, (Request) => CallIdIsValid(Request))
				;

			#endregion


			//fsm.OnUnhandledTrigger((state, trigger) => { });
		}

		public override Transaction Clone()
		{
			return new InviteTransaction(CallID, ViaBranch, CSeq, fsm.State);
		}
		public override string GetGraph()
		{
			return  UmlDotGraph.Format(fsm.GetInfo());
		}

		public override void Update(Request Request)
		{
			switch (Request.RequestLine.Method)
			{
				case "INVITE":
					fsm.Fire(INVITETrigger, Request);
					break;
				default: throw new InvalidOperationException($"Unsupported transaction transition ({Request.RequestLine.Method})");
			}
		}

		public override void Update(Response Response)
		{
			switch (Response.StatusLine.StatusCode)
			{
				case 180:
					fsm.Fire(Prov180Trigger, Response);
					break;
				case >= 100 and <= 199:
					fsm.Fire(Prov1xxTrigger, Response);
					break;
				case >= 200 and <= 299:
					fsm.Fire(Final2xxTrigger, Response);
					break;
				default: throw new InvalidOperationException($"Unsupported transaction transition ({Response.StatusLine.StatusCode})");
			}
		}


	}
}
