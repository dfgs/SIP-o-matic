using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SIP_o_matic.corelib.Models
{
	public class Device
	{
		[XmlAttribute]
		public required string Name
		{
			get;
			set;
		}
		
		public List<Address> Addresses
		{
			get;
			set;
		}
		
		
		public Device()
		{
			Addresses = new List<Address>();
		}
		[SetsRequiredMembers]
		public Device(string Name,IEnumerable<Address> Addresses)
		{
			this.Addresses = new List<Address>();
			this.Name = Name;
			this.Addresses.AddRange(Addresses);
		}
		public override string ToString()
		{
			return Name;
		}

	}

}
