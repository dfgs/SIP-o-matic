using LogLib;
using SIP_o_matic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class CallViewModel : ViewModel<Call>
	{
		public string CallID
		{
			get => Model.CallID;
		}

		public string SourceAddress
		{
			get=>Model.SourceAddress;
		}

		public string DestinationAddress
		{
			get=> Model.DestinationAddress;
		}


		public CallViewModel(ILogger Logger) : base(Logger)
		{
		}
	}
}
