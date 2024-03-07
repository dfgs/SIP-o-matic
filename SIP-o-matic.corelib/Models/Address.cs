using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SIP_o_matic.corelib.Models
{
	public class Address:IEquatable<Address>
	{
		[XmlAttribute]
		public required string Value
		{
			get;
			set;
		}

		public Address()
		{
			this.Value = "none";
		}

		[SetsRequiredMembers]
		public Address(string Value)
		{
			this.Value = Value;
		}

		public override string ToString()
		{
			return Value;
		}

		public bool Equals(Address? other)
		{
			if (other == null) return false;
			return other.Value == Value;
		}

	}
}
