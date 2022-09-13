using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SIP_o_matic.ViewModels
{
	public class HeaderFilterViewModel : FilterViewModel
	{


		public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(HeaderFilterViewModel),new PropertyMetadata("From"));
		public string Header
		{
			get { return (string)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(HeaderFilterViewModel), new PropertyMetadata("+33"));
		public string Value
		{
			get { return (string)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		public static readonly DependencyProperty OperandProperty = DependencyProperty.Register("Operand", typeof(FilterOperands), typeof(HeaderFilterViewModel), new PropertyMetadata(FilterOperands.Contains));
		public FilterOperands Operand
		{
			get { return (FilterOperands)GetValue(OperandProperty); }
			set { SetValue(OperandProperty, value); }
		}

		public override string Description => this.ToString();
		public HeaderFilterViewModel()
		{
		}

		public override void CopyFrom(FilterViewModel Other)
		{
			HeaderFilterViewModel? other;

			other = Other as HeaderFilterViewModel;
			if (other == null) return;

			this.Header = other.Header;
			this.Operand = other.Operand;
			this.Value = other.Value;
			OnPropertyChanged("Description");
		}

		public override string ToString()
		{
			string op;
			
			switch(Operand)
			{
				case FilterOperands.Contains:
					op = " ct ";
					break;
				case FilterOperands.In:
					op = " in ";
					break;
				case FilterOperands.Regex:
					op = " mt ";
					break;

				default:
					op = "??";
					break;
			}
			return $"{Header}{op}{Value}";
		}



	}
}
