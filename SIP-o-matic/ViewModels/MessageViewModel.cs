using LogLib;
using SIP_o_matic.corelib.Models;
using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
		public string Color
		{
			get;
			set;
		}
		public SIPMessage? SIPMessage
		{
			get;
			set;
		}

		public string SourceAddress
		{
			get=>Model.SourceAddress; 
		}

		public string DestinationAddress
		{
			get=> Model.DestinationAddress; 
		}


		private string? sourceDevice;
		public string SourceDevice
		{
			get => sourceDevice ?? SourceAddress;
			set => this.sourceDevice = value;
		}
		private string? destinationDevice;
		public string DestinationDevice
		{
			get => destinationDevice ?? DestinationAddress;
			set => this.destinationDevice = value;
		}

		public IEnumerable<string> Devices
		{
			get
			{
				if (SourceDevice!=null) yield return SourceDevice;
				if (DestinationDevice != null) yield return DestinationDevice;
			}
		}
		public static readonly DependencyProperty IsFlippedProperty = DependencyProperty.Register("IsFlipped", typeof(bool), typeof(MessageViewModel), new PropertyMetadata(false));
		public bool IsFlipped
		{
			get { return (bool)GetValue(IsFlippedProperty); }
			set { SetValue(IsFlippedProperty, value); }
		}

		public string Description
		{
			get
			{
				if (SIPMessage == null) return "Undefined";
				else if (SIPMessage is Request request) return $"[{Index}] {request.RequestLine}";
				else if (SIPMessage is Response response) return $"[{Index}] {response.StatusLine}";
				return "Undefined";
			}
		}

		public MessageViewModel(ILogger Logger) : base(Logger)
		{
			
		}

	}
}
