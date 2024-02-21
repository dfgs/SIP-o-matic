using SIP_o_matic.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SIP_o_matic.Models
{
	public class Device
	{
		[XmlAttribute]
		public required string Name
		{
			get;
			set;
		}
		
		public List<string> Addresses
		{
			get;
			set;
		}
		
		
		public Device()
		{
			Addresses = new List<string>();
		}
		[SetsRequiredMembers]
		public Device(string Name,IEnumerable<string> Addresses)
		{
			this.Addresses = new List<string>();
			this.Name = Name;
			this.Addresses.AddRange(Addresses);
		}
		public override string ToString()
		{
			return Name;
		}

	}

}
