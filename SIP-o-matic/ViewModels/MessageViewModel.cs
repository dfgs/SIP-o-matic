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
		public uint Index
		{
			get => Model.Index;
		}
		public DateTime Timestamp
		{
			get => Model.Timestamp;
		}
		public string Content
		{
			get =>Model.Content;
		}

		public string SourceAddress
		{
			get=>Model.SourceAddress; 
		}

		public string DestinationAddress
		{
			get=> Model.DestinationAddress; 
		}

		public MessageViewModel(ILogger Logger) : base(Logger)
		{
		}

	}
}
