using SIPParserLib;
using Stateless.Graph;
using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace SIP_o_matic.corelib.Models.Transactions
{
	public class ReferTransaction:Transaction
	{

		private static Regex callIDRegex = new Regex(@"(?<CallID>[^;]*);.*");

		private StateMachine<States, Triggers>.TriggerWithParameters<IResponse>? Prov1xxTrigger;
		private StateMachine<States, Triggers>.TriggerWithParameters<IResponse>? Final2xxTrigger;
		private StateMachine<States, Triggers>.TriggerWithParameters<IResponse>? ErrorTrigger;

		public string? ReplacedCallID
		{
			get;
			set;
		}

		protected override States TerminatedState => States.ReferTerminated;


		[SetsRequiredMembers]
		public ReferTransaction(string CallID, string ViaBranch, string CSeq) : base(CallID, ViaBranch, CSeq)
		{

		}

		protected override void OnConfigureFSM(StateMachine<States, Triggers> fsm)
		{
			// Undefined => Transfering => Proceeding => Terminated

			Prov1xxTrigger = fsm.SetTriggerParameters<IResponse>(Triggers.Prov1xx);
			Final2xxTrigger = fsm.SetTriggerParameters<IResponse>(Triggers.Final2xx);
			ErrorTrigger = fsm.SetTriggerParameters<IResponse>(Triggers.Error);


			fsm.Configure(States.Undefined)
				.PermitIf(ReferTrigger, States.ReferStarted,  AssertMessageBelongsToTransaction, TransactionErrorMessage)
				;

			fsm.Configure(States.ReferStarted)
				.PermitIf(ErrorTrigger, States.ReferError, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				.PermitIf(Prov1xxTrigger, States.ReferProceeding, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				.PermitIf(Final2xxTrigger, States.ReferTerminated, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				.OnEntry(CheckReplacedCallID)
				;

			fsm.Configure(States.ReferProceeding)
				.PermitIf(ErrorTrigger, States.ReferError, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				.PermitReentryIf(Prov1xxTrigger, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				.PermitIf(Final2xxTrigger, States.ReferTerminated, AssertMessageBelongsToTransaction, TransactionErrorMessage)
				;
		}

		

		private void CheckReplacedCallID(StateMachine<States, Triggers>.Transition Transition)
		{
			Request? request;
			ReferToHeader? header;
			SIPURL? uri;
			Header? uriHeader;
			Match match;

			request=Transition.Parameters[0] as Request;
			if (request == null) return;

			header = request.GetHeader<ReferToHeader>();
			if (header == null) return;

			uri = header.Value.URI as SIPURL;
			if (uri== null) return;

			uriHeader = uri.Headers.FirstOrDefault(item => item.Name == "Replaces");
			if (uriHeader == null) return;

			if (string.IsNullOrEmpty(uriHeader?.Value)) return;
			
			match = callIDRegex.Match(uriHeader.Value.Value);
			if (!match.Success) return;

			ReplacedCallID = match.Groups["CallID"].Value;


		}
		
		

	

		protected override StateMachine<States, Triggers>.TriggerWithParameters<IResponse> OnGetUpdateTrigger(IResponse Response)
		{
			switch (Response.StatusCode)
			{
				case 100:return Prov1xxTrigger!;
				case 202:return Final2xxTrigger!;
				case >= 400 and < 699: return ErrorTrigger!;
				default: throw new InvalidOperationException($"Unsupported transaction transition ({Response.StatusCode})");
			}
		}


	}
}
