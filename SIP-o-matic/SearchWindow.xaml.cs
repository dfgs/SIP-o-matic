using SIP_o_matic.corelib.Models;
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
using System.Windows.Shapes;

namespace SIP_o_matic
{
	/// <summary>
	/// Logique d'interaction pour SearchWindow.xaml
	/// </summary>
	public partial class SearchWindow : Window
	{


		public static readonly DependencyProperty ProjectProperty = DependencyProperty.Register("Project", typeof(ProjectViewModel), typeof(SearchWindow), new PropertyMetadata(null));
		public ProjectViewModel	Project
		{
			get { return (ProjectViewModel)GetValue(ProjectProperty); }
			set { SetValue(ProjectProperty, value); }
		}



		public SearchWindow()
		{
			InitializeComponent();
		}
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			searchTextBox.Focus();
		}

		private void CancelCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void CancelCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			DialogResult = false;
		}


		private void SearchNextCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (Project!=null) && !string.IsNullOrEmpty(searchTextBox.Text);
		}

		private void SearchNextCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			int firstIndex;

			firstIndex = Project.Dialogs.SelectedIndex+1;
			if (firstIndex >= Project.Dialogs.Count) firstIndex = 0;
			for(int t=firstIndex;t<Project.Dialogs.Count;t++)
			{
				if (Project.Dialogs[t].Match(searchTextBox.Text))
				{
					Project.Dialogs.SelectedItem = Project.Dialogs[t];
					return;
				}
			}
		}

		private void SearchPreviousCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (Project != null) && !string.IsNullOrEmpty(searchTextBox.Text);
		}

		private void SearchPreviousCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			int firstIndex;

			firstIndex = Project.Dialogs.SelectedIndex - 1;
			if (firstIndex <=0 ) firstIndex = Project.Dialogs.Count-1;
			for (int t = firstIndex; t >= 0; t--)
			{
				if (Project.Dialogs[t].Match(searchTextBox.Text))
				{
					Project.Dialogs.SelectedItem = Project.Dialogs[t];
					return;
				}
			}
		}


	}
}
