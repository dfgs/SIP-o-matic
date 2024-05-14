using SIP_o_matic.ViewModels;
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
	/// Logique d'interaction pour DialogsView.xaml
	/// </summary>
	public partial class DialogsView : UserControl
	{
		public DialogsView()
		{
			InitializeComponent();
		}

		private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
		{
			DialogViewModelCollection? dialogs;

			dialogs = DataContext as DialogViewModelCollection;
			if (dialogs == null) return;

			foreach(DialogViewModel dialog in dialogs)
			{
				dialog.IsChecked = !dialog.IsChecked;
			}
		}

		private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count>0) ListView.ScrollIntoView(e.AddedItems[0]);
        }


    }
}
