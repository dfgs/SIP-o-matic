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
using System.Windows.Shapes;

namespace SIP_o_matic
{
	/// <summary>
	/// Logique d'interaction pour RenameWindow.xaml
	/// </summary>
	public partial class RenameWindow : Window
	{


		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(RenameWindow), new PropertyMetadata(null));
		public string Value
		{
			get { return (string)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}



		public RenameWindow()
		{
			InitializeComponent();
		}
		private void root_Loaded(object sender, RoutedEventArgs e)
		{
			textBox.Focus();
			textBox.SelectAll();
		}

		#region events
		private void OKCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void OKCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void CancelCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void CancelCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			DialogResult = false;
		}

		#endregion

		
	}
}
