using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SIP_o_matic.ViewModels
{
	public class TimestampViewModel : ViewModel
	{


		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(DateTime), typeof(TimestampViewModel));
		public DateTime Value
		{
			get { return (DateTime)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}




		public TimestampViewModel() : base(NullLogger.Instance)
		{
		}
	}
}
