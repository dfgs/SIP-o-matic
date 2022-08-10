using Microsoft.Win32;
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

namespace SIP_o_matic
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ApplicationViewModel applicationViewModel;

		public MainWindow()
		{
			applicationViewModel = new ApplicationViewModel();

			InitializeComponent();
			DataContext = applicationViewModel;

			
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			string[] args = Environment.GetCommandLineArgs();

			if (args.Length > 1)
			{
				try
				{
					await applicationViewModel.AddProjectAsync();
					if (applicationViewModel.SelectedProject!=null) await applicationViewModel.SelectedProject.AddFileAsync(args[1]);
				}
				catch (Exception ex)
				{
					ShowError(ex);
				}
			}
		}


		private void ShowError(Exception ex)
		{
			MessageBox.Show(this,ex.Message, "Error");
		}

		

		#region command bindings
		private void NewCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;e.CanExecute = true;
		}

		private async void NewCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			try
			{
				await applicationViewModel.AddProjectAsync();
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}
		}
		private void AddFileCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = applicationViewModel.SelectedProject!=null;
		}

		private async void AddFileCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			OpenFileDialog dialog;

			dialog = new OpenFileDialog();
			dialog.Filter = "All files|*.*";
			dialog.Title = "Add file to project";

			if (applicationViewModel.SelectedProject == null) return;

			if (!dialog.ShowDialog(this) ?? false) return;

			try
			{
				await applicationViewModel.SelectedProject.AddFileAsync(dialog.FileName);
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}
		}
		private void RemoveFileCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = applicationViewModel?.SelectedProject?.SelectedFile != null;
		}

		private async void RemoveFileCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (applicationViewModel?.SelectedProject?.SelectedFile == null) return;
			try
			{
				await applicationViewModel.SelectedProject.RemoveFileAsync(applicationViewModel.SelectedProject.SelectedFile);
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}
		}
		#endregion


	}
}
