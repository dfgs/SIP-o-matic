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
using SIP_o_matic.corelib.DataSources;
using SIP_o_matic.Modules;
using SIP_o_matic.corelib.Models;



namespace SIP_o_matic
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ApplicationViewModel applicationViewModel;
		private ILogger Logger;

		public MainWindow()
		{

			//Logger = NullLogger.Instance;
			Logger = new FileLogger(new DefaultLogFormatter(), "SIP-o-matic.log");
			applicationViewModel = new ApplicationViewModel("");


			InitializeComponent();
			DataContext = applicationViewModel;

			
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			string[] args = Environment.GetCommandLineArgs();

			if (args.Length > 1)
			{
				applicationViewModel.Projects.AddNew();
				AddFiles("EOM", args[1]);
			}
		}


		private void ShowError(Exception ex)
		{
			MessageBox.Show(this,ex.Message, "Error");
		}


		private void AddFiles(string FileSource, params string[] FileNames)
		{
			ProgressWindow analyzeWindow;
			FileImporterModule fileImporterModule;
			ModelAnalyzeModule modelAnalyzeModule;

			if (applicationViewModel.Projects.SelectedItem == null) return;

			try
			{
				fileImporterModule = new FileImporterModule(Logger, applicationViewModel.Projects.SelectedItem.GetModel(), FileSource, FileNames);
				modelAnalyzeModule = new ModelAnalyzeModule(Logger, applicationViewModel.Projects.SelectedItem.GetModel());

				analyzeWindow = new ProgressWindow(Logger);
				analyzeWindow.Owner = this;
				analyzeWindow.Steps.AddRange(fileImporterModule.ProgressSteps.Union(modelAnalyzeModule.ProgressSteps));
				analyzeWindow.ShowDialog();

				applicationViewModel.Projects.SelectedItem.RefreshDeviceAndMessages();
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
				module = new AnalyzeModule(Logger, applicationViewModel.Projects.SelectedItem.GetModel());

				analyzeWindow = new ProgressWindow(Logger);
				analyzeWindow.Owner = this;
				analyzeWindow.Steps.AddRange(module.ProgressSteps);
				analyzeWindow.ShowDialog();

				applicationViewModel.Projects.SelectedItem.RefreshFrames();

			}
			catch (Exception ex)
			{
				ShowError(ex);
			}
		}

		#region command bindings
		private void AboutCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = true;
		}

		private void AboutCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			HelpWindow window;

			window = new HelpWindow();
			window.Owner = this;

			window.ShowDialog();
		}
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
			ProgressWindow analyzeWindow;
			ModelAnalyzeModule modelAnalyzeModule;
			ProjectViewModel newProject;

			dialog = new OpenFileDialog();
			dialog.Title = "Open xml file";
			dialog.DefaultExt = "xml";
			dialog.Filter = "xml files|*.xml|All files|*.*";
			dialog.Multiselect = false;

			if (dialog.ShowDialog(this) ?? false)
			{
				try
				{
					newProject= await applicationViewModel.OpenProjectAsync(dialog.FileName);

					modelAnalyzeModule = new ModelAnalyzeModule(Logger, newProject.GetModel());

					analyzeWindow = new ProgressWindow(Logger);
					analyzeWindow.Owner = this;
					analyzeWindow.Steps.AddRange(modelAnalyzeModule.ProgressSteps);
					analyzeWindow.ShowDialog();

					newProject.RefreshDeviceAndMessages();

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
			string? fileSource;

			fileSource = e.Parameter as string;
			if (fileSource == null) fileSource = "EOM";


			if (applicationViewModel.Projects.SelectedItem == null) return;

			dialog = new OpenFileDialog();
			dialog.Title = "Add file to project";
			dialog.Multiselect = true;

			switch(fileSource)
			{
				case "EOM":
					dialog.Filter = "html files|*.html|All files|*.*";
					break;
				case "Alcatel":
					dialog.Filter = "sip motor files|*.txt*|All files|*.*";
					break;
				case "pcapng":
					dialog.Filter = "pcapng files|*.pcapng|All files|*.*";
					break;
				case "SIP":
					dialog.Filter = "SIP files|*.sip|All files|*.*";
					break;
				default:
					dialog.Filter = "All files|*.*";
					break;
			}



			if (!dialog.ShowDialog(this) ?? false) return;

			AddFiles(fileSource, dialog.FileNames);
			
			

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

		private void EditDeviceOrAddressCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; 
			if (applicationViewModel.Projects.SelectedItem==null)
			{
				e.CanExecute = false;
				return;
			}
			e.CanExecute = (applicationViewModel.Projects.SelectedItem.Devices.SelectedDeviceOrAddress!=null);
		}

		private void EditDeviceOrAddressCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			RenameWindow window;
			DeviceViewModelCollection? devices;
			ProjectViewModel? project;
			string oldValue;

			project = applicationViewModel?.Projects.SelectedItem;
			if (project == null) return;
			devices = project.Devices;


			if (devices?.SelectedDeviceOrAddress == null) return;

			window = new RenameWindow();
			window.Owner = this;
			if (devices.SelectedDeviceOrAddress is DeviceViewModel deviceViewModel)
			{
				// rename device
				oldValue = deviceViewModel.Name;
				window.Value = oldValue;
				if (window.ShowDialog() ?? false)
				{
					try
					{
						project.RenameDevice(deviceViewModel, window.Value);
					}
					catch (Exception ex)
					{
						ShowError(ex);
					}
				}
			}
			else if (devices.SelectedDeviceOrAddress is AddressViewModel addressViewModel)
			{
				// rename address
				oldValue = addressViewModel.Value;
				window.Value = oldValue;

				if (window.ShowDialog() ?? false)
				{
					try
					{
						addressViewModel.Value = window.Value;
						project.UpdateAddresses(oldValue, window.Value);
					}
					catch (Exception ex)
					{
						ShowError(ex);
					}
				}
			}

		}


		private void AddDeviceCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;e.CanExecute=(applicationViewModel.Projects.SelectedItem != null);
			
		}

		private void AddDeviceCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			RenameWindow window;
			DeviceViewModelCollection? devices;
			ProjectViewModel? project;
			string oldValue;

			project = applicationViewModel?.Projects.SelectedItem;
			if (project == null) return;
			devices = project.Devices;


			window = new RenameWindow();
			window.Owner = this;
			if (devices.SelectedDeviceOrAddress is DeviceViewModel deviceViewModel)
			{
				oldValue = "New device";
				window.Value = oldValue;
				if (window.ShowDialog() ?? false)
				{
					try
					{
						project.AddDevice(new Device(window.Value, new Address[] { }));
					}
					catch (Exception ex)
					{
						ShowError(ex);
					}
				}
			}
			

		}

		private void AddAddressCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			if (applicationViewModel.Projects.SelectedItem == null)
			{
				e.CanExecute = false;
				return;
			}
			e.CanExecute = (applicationViewModel.Projects.SelectedItem.Devices.SelectedDeviceOrAddress as DeviceViewModel != null);
		}

		private void AddAddressCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			RenameWindow window;
			DeviceViewModelCollection? devices;
			ProjectViewModel? project;
			string oldValue;
			DeviceViewModel? device;

			project = applicationViewModel?.Projects.SelectedItem;
			if (project == null) return;
			devices = project.Devices;


			device = devices?.SelectedDeviceOrAddress as DeviceViewModel;

			if ( device== null) return;

			window = new RenameWindow();
			window.Owner = this;
							
			oldValue = "127.0.0.1";
			window.Value = oldValue;

			if (window.ShowDialog() ?? false)
			{
				try
				{
					project.AddAddressToDevice(device,new Address( window.Value));
				}
				catch (Exception ex)
				{
					ShowError(ex);
				}
			}
			

		}


		private void RemoveDeviceOrAddressCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			if (applicationViewModel.Projects.SelectedItem == null)
			{
				e.CanExecute = false;
				return;
			}
			e.CanExecute = (applicationViewModel.Projects.SelectedItem.Devices.SelectedDeviceOrAddress != null);
		}

		private void RemoveDeviceOrAddressCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			DeviceViewModelCollection? devices;
			ProjectViewModel? project;

			project = applicationViewModel?.Projects.SelectedItem;
			if (project == null) return;
			devices = project.Devices;


			if (devices?.SelectedDeviceOrAddress == null) return;

			
			if (devices.SelectedDeviceOrAddress is DeviceViewModel deviceViewModel)
			{
				// remove device
				project.RemoveDevice(deviceViewModel);
			}
			else if (devices.SelectedDeviceOrAddress is AddressViewModel addressViewModel)
			{
				// remove address
				project.RemoveAddress(addressViewModel);
			}

		}
		#endregion


	}
}
