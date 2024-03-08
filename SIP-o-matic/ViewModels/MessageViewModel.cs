using LogLib;
using ParserLib;
using PcapngFile;
using SIP_o_matic.corelib;
using SIP_o_matic.corelib.Models;
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
		public string TransactionColor
		{
			get;
			set;
		}
		public string DialogColor
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

		public Address SourceAddress
		{
			get=>Model.SourceAddress;
			set
			{
				Model.SourceAddress = value;
				OnPropertyChanged(nameof(SourceAddress));
				OnPropertyChanged(nameof(SourceDevice));
			}
		}

		public Address DestinationAddress
		{
			get=> Model.DestinationAddress;
			set
			{
				Model.DestinationAddress = value;
				OnPropertyChanged(nameof(DestinationAddress));
				OnPropertyChanged(nameof(DestinationDevice));
			}
		}


		public string SourceDevice
		{
			get => deviceNameProvider.GetDeviceName(Model.SourceAddress);
		}
		public string DestinationDevice
		{
			get => deviceNameProvider.GetDeviceName(Model.DestinationAddress);
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

		/*public static readonly DependencyProperty IsPinnedProperty = DependencyProperty.Register("IsPinned", typeof(bool), typeof(MessageViewModel), new PropertyMetadata(false));
		public bool IsPinned
		{
			get { return (bool)GetValue(IsPinnedProperty); }
			set { SetValue(IsPinnedProperty, value); }
		}*/


		private IDeviceNameProvider deviceNameProvider;

		public MessageViewModel(ILogger Logger,IDeviceNameProvider DeviceNameProvider) : base(Logger)
		{
			if (DeviceNameProvider == null) throw new ArgumentNullException(nameof(DeviceNameProvider));
			this.deviceNameProvider = DeviceNameProvider;
			this.deviceNameProvider.DeviceNameUpdated += DeviceNameProvider_DeviceNameUpdated;

			SIPMessage = new SIPMessageViewModel(Logger);
			TransactionColor = "Black";DialogColor = "Blue";

		}

		private void DeviceNameProvider_DeviceNameUpdated(object? sender, EventArgs e)
		{
			OnPropertyChanged(nameof(SourceDevice));
			OnPropertyChanged(nameof(DestinationDevice));
		}

		protected override void OnLoaded()
		{
			StringReader reader;
            SIPParserLib.SIPMessage sipMessage;

			base.OnLoaded();

			reader = new StringReader(Model.Content, ' ');
			try
			{
				sipMessage = SIPParserLib.SIPGrammar.SIPMessage.Parse(reader);
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
