using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.Models
{
	public enum CallStatuses { Initiated,Ringing,OnHold,InProgress };
	public class Call:ICloneable<Call>
	{
		public required string CallID
		{
			get;
			set;
		}

		public required string SourceAddress
		{
			get;
			set;
		}

		public required string DestinationAddress
		{
			get;
			set;
		}

		public required Address FromURI
		{
			get;
			set;
		}
		public required Address ToURI
		{
			get;
			set;
		}

		public required CallStatuses Status
		{
			get;
			set;
		}


		[SetsRequiredMembers]
		public Call(string callID, string SourceAddress,string DestinationAddress, Address FromURI, Address ToURI, CallStatuses Status)
		{
			CallID = callID;
			this.SourceAddress = SourceAddress;
			this.DestinationAddress = DestinationAddress;
			this.FromURI = FromURI;
			this.ToURI = ToURI;
			this.Status = Status;
		}

		public Call Clone()
		{
			return new Call(this.CallID, this.SourceAddress, this.DestinationAddress,this.FromURI, this.ToURI, this.Status);
		}

	}
}
