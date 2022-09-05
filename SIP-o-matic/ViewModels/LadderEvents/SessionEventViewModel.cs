using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SIP_o_matic.ViewModels
{
	public class SessionEventViewModel:ViewModel
	{
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


		public string Source
		{
			get => $"{SourceAddress} {SourcePort}";
		}
		public string Destination
		{
			get => $"{DestinationAddress} {DestinationPort}";
		}
		public SessionEventViewModel() : base(NullLogger.Instance)
		{
		}
	}
}
