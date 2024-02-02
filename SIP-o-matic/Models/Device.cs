using SIP_o_matic.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SIP_o_matic.Models
{
	public class Device
	{
		public required string Name
		{
			get;
			set;
		}
		
		public ObservableCollection<string> Addresses
		{
			get;
			set;
		}

		public Device()
		{
			Addresses = new ObservableCollection<string>();
		}

	}

}
