using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.Models
{
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

		[SetsRequiredMembers]
		public Call(string callID, string SourceAddress,string DestinationAddress)
		{
			CallID = callID;
			this.SourceAddress= SourceAddress;
			this.DestinationAddress = DestinationAddress;
		}

		public Call Clone()
		{
			return new Call(this.CallID, this.SourceAddress, this.DestinationAddress);
		}

	}
}
