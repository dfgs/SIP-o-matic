using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SIP_o_matic.ViewModels
{
	public class SessionViewModel:ViewModel
	{
		public static readonly DependencyProperty StartTimeProperty = DependencyProperty.Register("StartTime", typeof(DateTime), typeof(SessionViewModel));
		public DateTime StartTime
		{
			get { return (DateTime)GetValue(StartTimeProperty); }
			set { SetValue(StartTimeProperty, value); }
		}
		public static readonly DependencyProperty StopTimeProperty = DependencyProperty.Register("StopTime", typeof(DateTime), typeof(SessionViewModel));
		public DateTime StopTime
		{
			get { return (DateTime)GetValue(StopTimeProperty); }
			set { SetValue(StopTimeProperty, value); }
		}

		public static readonly DependencyProperty SourceAddressProperty = DependencyProperty.Register("SourceAddress", typeof(string), typeof(SessionViewModel),new PropertyMetadata("Undefined"));
		public string SourceAddress
		{
			get { return (string)GetValue(SourceAddressProperty); }
			set { SetValue(SourceAddressProperty, value); }
		}
		public static readonly DependencyProperty SourcePortProperty = DependencyProperty.Register("SourcePort", typeof(int), typeof(SessionViewModel));
		public int SourcePort
		{
			get { return (int)GetValue(SourcePortProperty); }
			set { SetValue(SourcePortProperty, value); }
		}

		public static readonly DependencyProperty DestinationAddressProperty = DependencyProperty.Register("DestinationAddress", typeof(string), typeof(SessionViewModel), new PropertyMetadata("Undefined"));
		public string DestinationAddress
		{
			get { return (string)GetValue(DestinationAddressProperty); }
			set { SetValue(DestinationAddressProperty, value); }
		}
		public static readonly DependencyProperty DestinationPortProperty = DependencyProperty.Register("DestinationPort", typeof(int), typeof(SessionViewModel));
		public int DestinationPort
		{
			get { return (int)GetValue(DestinationPortProperty); }
			set { SetValue(DestinationPortProperty, value); }
		}
		public static readonly DependencyProperty CodecProperty = DependencyProperty.Register("Codec", typeof(string), typeof(SessionViewModel), new PropertyMetadata("Undefined"));
		public string Codec
		{
			get { return (string)GetValue(CodecProperty); }
			set { SetValue(CodecProperty, value); }
		}

		public SessionViewModel() : base(NullLogger.Instance)
		{
		}
	}
}
