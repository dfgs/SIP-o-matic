﻿using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SIP_o_matic.ViewModels
{
	public class TransactionEventViewModel : LadderEventViewModel
	{


		public static readonly DependencyProperty DisplayProperty = DependencyProperty.Register("Display", typeof(string), typeof(TransactionEventViewModel), new PropertyMetadata(null));
		public override string Display
		{
			get { return (string)GetValue(DisplayProperty); }
			set { SetValue(DisplayProperty, value); }
		}


		public override string BorderColor
		{
			get => "Green";
		}
		public static readonly DependencyProperty EventColorProperty = DependencyProperty.Register("EventColor", typeof(string), typeof(TransactionEventViewModel), new PropertyMetadata("Blue"));
		public override string EventColor
		{
			get { return (string)GetValue(EventColorProperty); }
			set { SetValue(EventColorProperty, value); }
		}
		public TransactionViewModel? Transaction
		{
			get;
			set;
		}
		public TransactionEventViewModel() : base()
		{
		}

	}
}
