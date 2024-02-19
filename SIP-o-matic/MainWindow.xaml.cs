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
using SIP_o_matic.Modules;


namespace SIP_o_matic
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ApplicationViewModel applicationViewModel;
		private DataSourceManager dataSourceManager;
		private ILogger Logger;

		public MainWindow()
		{
			dataSourceManager= new DataSourceManager();
			dataSourceManager.Register(new OracleOEMDataSource());
			//dataSourceManager.Register(new OracleSBCDataSource());
			//dataSourceManager.Register(new WiresharkDataSource());
			//dataSourceManager.Register(new AudiocodesSyslogDataSource());
			//dataSourceManager.Register(new AlcatelSIPTraceDataSource());

			Logger = NullLogger.Instance;
			applicationViewModel = new ApplicationViewModel(Logger);


			InitializeComponent();
			DataContext = applicationViewModel;

			
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			string[] args = Environment.GetCommandLineArgs();

			if (args.Length > 1)
			{
				applicationViewModel.Projects.AddNew();
				AddFiles(args[1]);
			}
		}


		private void ShowError(Exception ex)
		{
			MessageBox.Show(this,ex.Message, "Error");
		}


		private void AddFiles(params string[] FileNames)
		{
			ProgressWindow analyzeWindow;
			FileImporterModule module;

			if (applicationViewModel.Projects.SelectedItem == null) return;

			try
			{
				module = new FileImporterModule(Logger, applicationViewModel.Projects.SelectedItem, FileNames);

				analyzeWindow = new ProgressWindow(Logger);
				analyzeWindow.Owner = this;
				analyzeWindow.Steps.AddRange(module.ProgressSteps);
				analyzeWindow.ShowDialog();
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}
		}


		private void Analyze()
		{
			ProgressWindow analyzeWindow;
			AnalyzeModule module;


			if ((applicationViewModel.Projects.SelectedItem == null) || (applicationViewModel.Projects.SelectedItem.Messages.Count == 0)) return;
			try
			{
				module = new AnalyzeModule(Logger, applicationViewModel.Projects.SelectedItem);

				analyzeWindow = new ProgressWindow(Logger);
				analyzeWindow.Owner = this;
				analyzeWindow.Steps.AddRange(module.ProgressSteps);
				analyzeWindow.ShowDialog();
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}
		}

		#region command bindings

		private void AnalyzeCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = (applicationViewModel.Projects.SelectedItem!=null) && (applicationViewModel.Projects.SelectedItem.Messages.Count>0);
		}

		private void AnalyzeCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Analyze();
		}

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
		private void CloseCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = (applicationViewModel.Projects.SelectedItem != null) ;
		}

		private void CloseCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			try
			{
				applicationViewModel.CloseCurrentProject();
			}
			catch(Exception ex)
			{
				ShowError(ex);
			}
		}

		private void OpenFileCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = true;
		}

		private async void OpenFileCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			OpenFileDialog dialog;

			dialog = new OpenFileDialog();
			dialog.Title = "Open xml file";
			dialog.DefaultExt = "xml";
			dialog.Filter = "xml files|*.xml|All files|*.*";
			dialog.Multiselect = false;

			if (dialog.ShowDialog(this) ?? false)
			{
				try
				{
					await applicationViewModel.OpenProjectAsync(dialog.FileName);
				}
				catch (Exception ex)
				{
					ShowError(ex);
				}
			}


		}

		

		private void AddFileCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = applicationViewModel.Projects.SelectedItem!=null;
		}

		private void AddFileCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			OpenFileDialog dialog;


			if (applicationViewModel.Projects.SelectedItem == null) return;

			dialog = new OpenFileDialog();
			dialog.Filter = "All files|*.*";
			dialog.Title = "Add file to project";
			dialog.Multiselect = true;

			if (!dialog.ShowDialog(this) ?? false) return;

			AddFiles(dialog.FileNames);
			/*dataSources = dataSourceManager.GetDataSourceForFile(dialog.FileName).ToArray();

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
			}*/

			
			

		}

		private void SaveCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = (applicationViewModel.Projects.SelectedItem != null) ;
		}

		private async void SaveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (applicationViewModel.Projects.SelectedItem == null) return;
			if (applicationViewModel.Projects.SelectedItem.Path == null) await SaveProjectAsAsync();
			else await SaveProjectAsync();
		}
		private void SaveAsCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = (applicationViewModel.Projects.SelectedItem != null);
		}

		private async void SaveAsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (applicationViewModel.Projects.SelectedItem == null) return;
			await SaveProjectAsAsync();
		}



		private async Task SaveProjectAsync()
		{
			if (applicationViewModel.Projects.SelectedItem == null) return;
			try
			{
				await applicationViewModel.Projects.SelectedItem.SaveAsync(applicationViewModel.Projects.SelectedItem.Path);
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}
		}

		private async Task SaveProjectAsAsync()
		{
			SaveFileDialog dialog;

			if (applicationViewModel.Projects.SelectedItem == null) return;

			dialog = new SaveFileDialog();
			dialog.Title = "Save project as";
			dialog.DefaultExt = "xml";
			dialog.Filter = "xml files|*.xml|All files|*.*";

			if (dialog.ShowDialog(this) ?? false)
			{
				try
				{
					await applicationViewModel.Projects.SelectedItem.SaveAsync(dialog.FileName);
				}
				catch (Exception ex)
				{
					ShowError(ex);
				}
			}

		}


		private void ExportSIPCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = (applicationViewModel.Projects.SelectedItem != null);
		}

		private async void ExportSIPCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			SaveFileDialog dialog;

			if (applicationViewModel.Projects.SelectedItem == null) return;
			
			dialog = new SaveFileDialog();
			dialog.Title = "Save project as";
			dialog.DefaultExt = "sip";
			dialog.Filter = "sip files|*.sip|All files|*.*";

			if (dialog.ShowDialog(this) ?? false)
			{
				try
				{
					await applicationViewModel.Projects.SelectedItem.ExportSIPAsync(dialog.FileName);
				}
				catch (Exception ex)
				{
					ShowError(ex);
				}
			}
		}



		#endregion


	}
}
