using LogLib;
using ParserLib;
using PcapngFile;
using SIP_o_matic.corelib;
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

		public string Description
		{
			get
			{
				return $"[{Index}] {SIPMessage.Description}";
			}
		}

		public SIPMessageViewModel SIPMessage
		{
			get;
			private set;
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


		public MessageViewModel(ILogger Logger) : base(Logger)
		{
			SIPMessage = new SIPMessageViewModel(Logger);
			Color = "Black";
		}

		protected override void OnLoaded()
		{
			StringReader reader;
			SIPMessage sipMessage;

			base.OnLoaded();

			reader = new StringReader(Model.Content, ' ');
			try
			{
				sipMessage = SIPGrammar.SIPMessage.Parse(reader);
			}
			catch (Exception ex)
			{
				string error = $"Failed to decode SIP message ({ex.Message})\r\r{Model.Content}";
				throw new InvalidOperationException(error);
			}

			SIPMessage.Load(sipMessage);
			

		}

	}
}
