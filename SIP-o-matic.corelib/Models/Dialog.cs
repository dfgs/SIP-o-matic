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
using System.Xml.Serialization;
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

		public required Address SourceAddress
		{
			get;
			set;
		}

		public required Address DestinationAddress
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
		[XmlIgnore]
		public bool IsChecked
		{
			get;
			set;
		}

		public Dialog()
		{

		}

		[SetsRequiredMembers]
		public Dialog(DateTime TimeStamp, string callID, Address SourceAddress, Address DestinationAddress,string FromTag,string? ToTag, string Caller, string Callee)
		{
			this.TimeStamp = TimeStamp;
			this.CallID = callID;
			this.SourceAddress = SourceAddress;
			this.DestinationAddress = DestinationAddress;
			this.FromTag = FromTag;
			this.ToTag = ToTag;
			this.Caller = Caller;
			this.Callee = Callee;

		}

		public bool Match(SIPMessage SIPMessage)
		{
			return (CallID == SIPMessage.GetCallID()) &&
				((this.FromTag == SIPMessage.GetFromTag() || (this.FromTag == SIPMessage.GetToTag())));
		}
		

		
		

	}
}
