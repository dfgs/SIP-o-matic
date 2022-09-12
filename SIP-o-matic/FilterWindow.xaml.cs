using SIP_o_matic.ViewModels.Filters;
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
	/// Logique d'interaction pour FilterWindow.xaml
	/// </summary>
	public partial class FilterWindow : Window
	{
		public static IEnumerable<FilterOperands> Operands = Enum.GetValues<FilterOperands>();
		public static IEnumerable<string> Headers = new string[] {"From","To","Call-ID","P-Asserted-Identity" };
		public FilterWindow()
		{
			InitializeComponent();
		}

		private void CancelCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = true;
		}

		private void CancelCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			this.DialogResult = false;
		}
		private void OKCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = true;
		}

		private void OKCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			this.DialogResult = true;
		}



	}
}
