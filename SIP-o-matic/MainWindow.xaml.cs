﻿using LogLib;
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
using System.Text.Json;
using SIP_o_matic.DataSources;

namespace SIP_o_matic
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ApplicationViewModel applicationViewModel;
		private DataSourceManager dataSourceManager;
		public MainWindow()
		{
			dataSourceManager= new DataSourceManager();
			dataSourceManager.Register(new OracleDataSource());
			dataSourceManager.Register(new WiresharkDataSource());

			applicationViewModel = new ApplicationViewModel();


			InitializeComponent();
			DataContext = applicationViewModel;

			
		}

		/*private async void Window_Loaded(object sender, RoutedEventArgs e)
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
		}//**/


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
			IDataSource[] dataSources;

			dialog = new OpenFileDialog();
			dialog.Filter = "All files|*.*";
			dialog.Title = "Add file to project";

			if (applicationViewModel.SelectedProject == null) return;

			if (!dialog.ShowDialog(this) ?? false) return;

			dataSources = dataSourceManager.GetDataSourceForFile(dialog.FileName).ToArray();

			if (dataSources.Length == 0)
			{
				ShowError(new Exception("File format is not supported"));
				return;
			}

			try
			{
				await applicationViewModel.SelectedProject.AddFileAsync(dialog.FileName, dataSources[0]);
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


		private void CopyLogsCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = applicationViewModel?.SelectedProject != null;
		}

		private void CopyLogsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			string logs;

			if (applicationViewModel?.SelectedProject == null) return;
			try
			{
				logs=string.Join("\r\n",((ProjectLogger)applicationViewModel.SelectedProject.Logger).Logs.Select(item=>item.Message));
				Clipboard.SetText(logs);
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}
		}
		#endregion


	}
}
