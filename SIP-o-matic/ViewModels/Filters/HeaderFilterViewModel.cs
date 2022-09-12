using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SIP_o_matic.ViewModels.Filters
{
	public class HeaderFilterViewModel : FilterViewModel
	{


		public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(HeaderFilterViewModel));
		public string Header
		{
			get { return (string)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(HeaderFilterViewModel));
		public string Value
		{
			get { return (string)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		public static readonly DependencyProperty OperandProperty = DependencyProperty.Register("Operand", typeof(FilterOperands), typeof(HeaderFilterViewModel));
		public string Operand
		{
			get { return (string)GetValue(OperandProperty); }
			set { SetValue(OperandProperty, value); }
		}

		public HeaderFilterViewModel()
		{
		}



	}
}
