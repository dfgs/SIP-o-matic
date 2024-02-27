using LogLib;
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

		public string SourceAddress
		{
			get=>Model.SourceAddress; 
		}

		public string DestinationAddress
		{
			get=> Model.DestinationAddress; 
		}


		public string? SourceDevice
		{
			get;
			set;
		}
		public string? DestinationDevice
		{
			get;
			set;
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
		}

	}
}
