﻿using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SIP_o_matic.ViewModels
{
	public class DialogEventViewModel : LadderEventViewModel
	{


		public static readonly DependencyProperty DisplayProperty = DependencyProperty.Register("Display", typeof(string), typeof(DialogEventViewModel), new PropertyMetadata(null));
		public override string Display
		{
			get { return (string)GetValue(DisplayProperty); }
			set { SetValue(DisplayProperty, value); }
		}

		public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(DialogEventViewModel), new PropertyMetadata(false));
		public bool IsExpanded
		{
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}





		public DialogEventViewModel() : base()
		{
		}

	}
}