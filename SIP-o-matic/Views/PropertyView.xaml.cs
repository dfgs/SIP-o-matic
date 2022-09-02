using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SIP_o_matic.Views
{
	/// <summary>
	/// Logique d'interaction pour PropertyView.xaml
	/// </summary>
	public partial class PropertyView : UserControl
	{


		public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(PropertyView));
		public string Header
		{
			get { return (string)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(PropertyView));
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}



		public PropertyView()
		{
			InitializeComponent();
		}
	}
}
