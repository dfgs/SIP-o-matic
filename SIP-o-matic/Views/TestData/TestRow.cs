using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SIP_o_matic.Views.TestData
{
	internal class TestRow:DependencyObject
	{


		public static readonly DependencyProperty LeftColumnProperty = DependencyProperty.Register("LeftColumn", typeof(string), typeof(TestRow), new PropertyMetadata(null));
		public string LeftColumn
		{
			get { return (string)GetValue(LeftColumnProperty); }
			set { SetValue(LeftColumnProperty, value); }
		}
		public static readonly DependencyProperty RightColumnProperty = DependencyProperty.Register("RightColumn", typeof(string), typeof(TestRow), new PropertyMetadata(null));
		public string RightColumn
		{
			get { return (string)GetValue(RightColumnProperty); }
			set { SetValue(RightColumnProperty, value); }
		}

		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(TestRow), new PropertyMetadata("Line 1"));
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}



	}
}
