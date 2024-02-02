using LogLib;
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
			dataSourceManager.Register(new OracleOEMDataSource());
			//dataSourceManager.Register(new OracleSBCDataSource());
			//dataSourceManager.Register(new WiresharkDataSource());
			//dataSourceManager.Register(new AudiocodesSyslogDataSource());
			//dataSourceManager.Register(new AlcatelSIPTraceDataSource());

			applicationViewModel = new ApplicationViewModel(NullLogger.Instance);


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

		private void NewCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			try
			{
				applicationViewModel.Projects.AddNew();
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}
		}

		private void RemoveFileCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = applicationViewModel.Projects.SelectedItem?.SourceFiles.SelectedItem != null;
		}

		private void RemoveFileCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{

			if (applicationViewModel.Projects.SelectedItem == null) return;

			applicationViewModel.Projects.SelectedItem.SourceFiles.RemoveSelected();
		}

		private void AddFileCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = applicationViewModel.Projects.SelectedItem!=null;
		}

		private void AddFileCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			OpenFileDialog dialog;
			IDataSource[] dataSources;
			IDataSource selectedDataSource;
			DataSourceWindow dataSourceWindow;

			dialog = new OpenFileDialog();
			dialog.Filter = "All files|*.*";
			dialog.Title = "Add file to project";

			if (applicationViewModel.Projects.SelectedItem == null) return;

			if (!dialog.ShowDialog(this) ?? false) return;

			dataSources = dataSourceManager.GetDataSourceForFile(dialog.FileName).ToArray();

			if (dataSources.Length == 0)
			{
				ShowError(new Exception("File format is not supported"));
				return;
			}

			if (dataSources.Length == 1) selectedDataSource = dataSources[0];
			else
			{
				dataSourceWindow = new DataSourceWindow();
				dataSourceWindow.Owner = this.Owner;
				dataSourceWindow.DataContext = dataSources;

				if (!dataSourceWindow.ShowDialog() ?? false) return;
				selectedDataSource = dataSourceWindow.SelectedDataSource;
			}


			try
			{
				applicationViewModel.Projects.SelectedItem.SourceFiles.Add(dialog.FileName);
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}
		}
		

		
		#endregion


	}
}
