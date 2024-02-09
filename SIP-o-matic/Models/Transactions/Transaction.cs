using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.Models.Transactions
{
	public abstract class Transaction:ICloneable<Transaction>, ISIPMessageMatch
	{
		public enum States
		{
			Undefined,
			// INVITE transaction state machine RFC 3261
			Calling, Proceeding, Ringing, Completed, 
			
			// Global terminated state
			Terminated
		};
		public enum Triggers { INVITE, ACK, Prov1xx, Prov180, Final2xx, Error };


		public required string CallID
		{
			get;
			set;
		}
		public required string ViaBranch
		{
			get;
			set;
		}

		public required string CSeq
		{
			get;
			set;
		}

		public abstract States State
		{
			get;
		}
		public abstract bool IsTerminated
		{
			get;
		}

		public Transaction()
		{

		}

		[SetsRequiredMembers]
		public Transaction(string CallID, string ViaBranch, string CSeq)
		{
			this.CallID = CallID;
			this.ViaBranch = ViaBranch;
			this.CSeq = CSeq;
		}

		public bool Match(Request Request)
		{
			return (CallID == Request.GetHeader<CallIDHeader>()?.Value) && (ViaBranch == Request.GetHeader<ViaHeader>()?.GetParameter<ViaBranch>()?.Value) && (CSeq == Request.GetHeader<CSeqHeader>()?.Value);
		}
		public bool Match(Response Response)
		{
			return (CallID == Response.GetHeader<CallIDHeader>()?.Value) && (ViaBranch == Response.GetHeader<ViaHeader>()?.GetParameter<ViaBranch>()?.Value) && (CSeq == Response.GetHeader<CSeqHeader>()?.Value);
		}

		protected bool AssertMessageBelongsToTransaction(Request Request)
		{
			return Match(Request);
		}
		protected bool AssertMessageBelongsToTransaction(Response Response)
		{
			return Match(Response);
		}

		public abstract string GetGraph();

		public abstract Transaction Clone();
		public abstract void Update(Request Request);
		public abstract void Update(Response Response);
		
	}
}
