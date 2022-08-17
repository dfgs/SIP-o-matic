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





		public TransactionEventViewModel() : base()
		{
		}

	}
}