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
	public class MessageViewModel : ViewModel<Message>
	{
		public DateTime Timestamp
		{
			get => Model.Timestamp;
			set => Model.Timestamp = value;
		}
		public string Content
		{
			get =>Model.Content;
			set => Model.Content = value;
		}

		public string SourceAddress
		{
			get=>Model.SourceAddress; 
			set => Model.SourceAddress = value;
		}

		public string DestinationAddress
		{
			get=> Model.DestinationAddress; 
			set => Model.DestinationAddress = value;
		}

		public MessageViewModel(ILogger Logger) : base(Logger)
		{
		}

	}
}
