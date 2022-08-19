using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SIP_o_matic.ViewModels
{
	public class SIPMessageEventViewModel : LadderEventViewModel
	{


		public static readonly DependencyProperty DisplayProperty = DependencyProperty.Register("Display", typeof(string), typeof(SIPMessageEventViewModel), new PropertyMetadata(null));
		public override string Display
		{
			get { return (string)GetValue(DisplayProperty); }
			set { SetValue(DisplayProperty, value); }
		}


		public override string BorderColor
		{
			get => "GoldenRod";
		}
		public static readonly DependencyProperty EventColorProperty = DependencyProperty.Register("EventColor", typeof(string), typeof(SIPMessageEventViewModel), new PropertyMetadata("Blue"));
		public override string EventColor
		{
			get { return (string)GetValue(EventColorProperty); }
			set { SetValue(EventColorProperty, value); }
		}

		public static readonly DependencyProperty HasBodyProperty = DependencyProperty.Register("HasBody", typeof(bool), typeof(SIPMessageEventViewModel), new PropertyMetadata(false));
		public bool HasBody
		{
			get { return (bool)GetValue(HasBodyProperty); }
			set { SetValue(HasBodyProperty, value); }
		}

		public TransactionEventViewModel? TransactionEvent
		{
			get;
			set;
		}
		public SIPMessageEventViewModel() : base()
		{
		}

	}
}
