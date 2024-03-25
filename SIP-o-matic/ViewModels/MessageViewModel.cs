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
using Address = SIP_o_matic.corelib.Models.Address;

namespace SIP_o_matic.ViewModels
{
	public class MessageViewModel : EventViewModel<Message>, IEventViewModel
	{

		public uint Index
		{
			get => Model.Index;
		}
		
		public string Content
		{
			get =>Model.Content;
		}
		public string TransactionColor
		{
			get => Model.TransactionColor;
		}
		public string DialogColor
		{
			get => Model.DialogColor;
		}
		public string Description
		{
			get
			{
				return $"[{Index}] {SIPMessage?.Description??""}";
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


 

		

		public static readonly DependencyProperty IsFlippedProperty = DependencyProperty.Register("IsFlipped", typeof(bool), typeof(MessageViewModel), new PropertyMetadata(false));
		public bool IsFlipped
		{
			get { return (bool)GetValue(IsFlippedProperty); }
			set { SetValue(IsFlippedProperty, value); }
		}

	
		//private IDeviceNameProvider deviceNameProvider;

		public MessageViewModel(Message Model, IDeviceNameProvider DeviceNameProvider) : base(Model,DeviceNameProvider)
		{
 			this.SIPMessage = DeviceNameProvider.GetSIPMessage(Model);
		}

		/*private void DeviceNameProvider_DeviceNameUpdated(object? sender, EventArgs e)
		{
			OnPropertyChanged(nameof(SourceDevice));
			OnPropertyChanged(nameof(DestinationDevice));
		}*/

		

	}
}
