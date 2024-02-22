using SIP_o_matic.corelib.DataSources;
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
	/// Logique d'interaction pour DataSourceWindow.xaml
	/// </summary>
	public partial class DataSourceWindow : Window
	{


		public static readonly DependencyProperty SelectedDataSourceProperty = DependencyProperty.Register("SelectedDataSource", typeof(IDataSource), typeof(DataSourceWindow));

		public IDataSource SelectedDataSource
		{
			get { return (IDataSource)GetValue(SelectedDataSourceProperty); }
			set { SetValue(SelectedDataSourceProperty, value); }
		}




		public DataSourceWindow()
		{
			InitializeComponent();
		}

		private void CancelCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;e.CanExecute = true;
		}

		private void CancelCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			this.DialogResult = false;
		}
		private void OKCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = SelectedDataSource!=null;
		}

		private void OKCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			this.DialogResult = true;
		}

	}
}
