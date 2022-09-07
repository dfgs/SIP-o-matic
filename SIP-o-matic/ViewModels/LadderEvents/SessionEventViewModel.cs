using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SIP_o_matic.ViewModels
{
	public class SessionEventViewModel: LadderEventViewModel
	{
		public override string BorderColor
		{
			get => "Blue";
		}
		public static readonly DependencyProperty EventColorProperty = DependencyProperty.Register("EventColor", typeof(string), typeof(SessionEventViewModel), new PropertyMetadata("Blue"));
		public override string EventColor
		{
			get { return (string)GetValue(EventColorProperty); }
			set { SetValue(EventColorProperty, value); }
		}

		public static readonly DependencyProperty DisplayProperty = DependencyProperty.Register("Display", typeof(string), typeof(SessionEventViewModel), new PropertyMetadata(null));
		public override string Display
		{
			get { return (string)GetValue(DisplayProperty); }
			set { SetValue(DisplayProperty, value); }
		}


		public static readonly DependencyProperty StartTimeProperty = DependencyProperty.Register("StartTime", typeof(TimestampViewModel), typeof(SessionEventViewModel));
		public TimestampViewModel StartTime
		{
			get { return (TimestampViewModel)GetValue(StartTimeProperty); }
			set { SetValue(StartTimeProperty, value); }
		}
		public static readonly DependencyProperty StopTimeProperty = DependencyProperty.Register("StopTime", typeof(TimestampViewModel), typeof(SessionEventViewModel));
		public TimestampViewModel StopTime
		{
			get { return (TimestampViewModel)GetValue(StopTimeProperty); }
			set { SetValue(StopTimeProperty, value); }
		}

		public static readonly DependencyProperty SourceAddressProperty = DependencyProperty.Register("SourceAddress", typeof(string), typeof(SessionEventViewModel));
		public string SourceAddress
		{
			get { return (string)GetValue(SourceAddressProperty); }
			set { SetValue(SourceAddressProperty, value); }
		}
		public static readonly DependencyProperty SourcePortProperty = DependencyProperty.Register("SourcePort", typeof(int), typeof(SessionEventViewModel));
		public int SourcePort
		{
			get { return (int)GetValue(SourcePortProperty); }
			set { SetValue(SourcePortProperty, value); }
		}

		public static readonly DependencyProperty DestinationAddressProperty = DependencyProperty.Register("DestinationAddress", typeof(string), typeof(SessionEventViewModel));
		public string DestinationAddress
		{
			get { return (string)GetValue(DestinationAddressProperty); }
			set { SetValue(DestinationAddressProperty, value); }
		}
		public static readonly DependencyProperty DestinationPortProperty = DependencyProperty.Register("DestinationPort", typeof(int), typeof(SessionEventViewModel));
		public int DestinationPort
		{
			get { return (int)GetValue(DestinationPortProperty); }
			set { SetValue(DestinationPortProperty, value); }
		}

		public static readonly DependencyProperty CodecProperty = DependencyProperty.Register("Codec", typeof(string), typeof(SessionEventViewModel));
		public string Codec
		{
			get { return (string)GetValue(CodecProperty); }
			set { SetValue(CodecProperty, value); }
		}

		public static readonly DependencyProperty DialogEventProperty = DependencyProperty.Register("DialogEvent", typeof(DialogEventViewModel), typeof(SessionEventViewModel));
		public DialogEventViewModel DialogEvent
		{
			get { return (DialogEventViewModel)GetValue(DialogEventProperty); }
			set { SetValue(DialogEventProperty, value); }
		}
		public static readonly DependencyProperty TransactionEventProperty = DependencyProperty.Register("TransactionEvent", typeof(TransactionEventViewModel), typeof(SessionEventViewModel));
		public TransactionEventViewModel? TransactionEvent
		{
			get { return (TransactionEventViewModel)GetValue(TransactionEventProperty); }
			set { SetValue(TransactionEventProperty, value); }
		}


		public string Source
		{
			get => $"{SourceAddress} {SourcePort}";
		}
		public string Destination
		{
			get => $"{DestinationAddress} {DestinationPort}";
		}
		public SessionEventViewModel() : base()
		{
		}
	}
}
