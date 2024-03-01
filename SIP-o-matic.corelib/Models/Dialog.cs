using SIP_o_matic.corelib.Models.Transactions;
using SIPParserLib;
using Stateless;
using Stateless.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using static SIP_o_matic.corelib.Models.Transactions.Transaction;

namespace SIP_o_matic.corelib.Models
{
	public class Dialog:ISIPMessageMatch
	{

		public required DateTime TimeStamp
		{
			get;
			set;
		}

		public required string CallID
		{
			get;
			set;
		}

		public required string SourceDevice
		{
			get;
			set;
		}

		public required string DestinationDevice
		{
			get;
			set;
		}
		public required string FromTag
		{
			get;
			set;
		}

		public string? ToTag
		{
			get;
			set;
		}

		public required string Caller
		{
			get;
			set;
		}
		public required string Callee
		{
			get;
			set;
		}

		[SetsRequiredMembers]
		public Dialog(DateTime TimeStamp, string callID, string SourceDevice,string DestinationDevice,string FromTag,string? ToTag, string Caller, string Callee)
		{
			this.TimeStamp = TimeStamp;
			this.CallID = callID;
			this.SourceDevice = SourceDevice;
			this.DestinationDevice = DestinationDevice;
			this.FromTag = FromTag;
			this.ToTag = ToTag;
			this.Caller = Caller;
			this.Callee = Callee;

		}

		
		public bool Match(ISIPMessage MessageInfo)
		{
			return (CallID == MessageInfo.GetCallID()) &&
				((this.FromTag == MessageInfo.GetFromTag() || (this.FromTag == MessageInfo.GetToTag())));
		}

		
		

	}
}
