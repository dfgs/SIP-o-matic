using LogLib;
using SIP_o_matic.Models;
using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class TelephonyEventViewModel : ViewModel<TelephonyEvent>
	{
		public DateTime Timestamp
		{
			get => Model.Timestamp;
		}
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

		public Address FromURI
		{
			get => Model.FromURI;
		}
		public Address ToURI
		{
			get => Model.ToURI;
		}

		public uint MessageIndex
		{
			get => Model.MessageIndex;
		}

		public TelephonyEventTypes EventType
		{
			get => Model.EventType;
		}

		public TelephonyEventViewModel(ILogger Logger) : base(Logger)
		{
		}
	}
}
