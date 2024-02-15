using SIP_o_matic.DataSources;
using SIP_o_matic.Models;
using SIP_o_matic.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SIPParserLib;
using ParserLib;
using LogLib;
using SIP_o_matic.Models.Transactions;
using SIP_o_matic.Modules;


namespace SIP_o_matic
{
	/// <summary>
	/// Logique d'interaction pour AnalyzeWindow.xaml
	/// </summary>
	public partial class AnalyzeWindow : Window
	{
		private bool terminated = false;
		private CancellationTokenSource? cancelToken;

		private AnalyzeModule analyzeModule;
		private CallFormatModule callFormatModule;

		public static readonly DependencyProperty StepsProperty = DependencyProperty.Register("Steps", typeof(List<AnalysisStep>), typeof(AnalyzeWindow), new PropertyMetadata(null));
		public List<AnalysisStep> Steps
		{
			get { return (List<AnalysisStep>)GetValue(StepsProperty); }
			set { SetValue(StepsProperty, value); }
		}


		public static readonly DependencyProperty ProjectProperty = DependencyProperty.Register("Project", typeof(ProjectViewModel), typeof(AnalyzeWindow), new PropertyMetadata(null));
		public ProjectViewModel Project
		{
			get { return (ProjectViewModel)GetValue(ProjectProperty); }
			set { SetValue(ProjectProperty, value); }
		}

		private ILogger logger;



		public AnalyzeWindow(ILogger Logger)
		{
			this.logger = Logger; 

			cancelToken = new CancellationTokenSource();

			analyzeModule = new AnalyzeModule(Logger);
			callFormatModule = new CallFormatModule(Logger);

			Steps = new List<AnalysisStep>();
			Steps.Add(new AnalysisStep() { Label = "Extracting devices",TaskFactory= ExtractDevicesAsync });
			Steps.Add(new AnalysisStep() { Label = "Extracting sip messages",TaskFactory= ExtractMessagesAsync });
			Steps.Add(new AnalysisStep() { Label = "Creating key frames", TaskFactory = analyzeModule.CreateKeyFramesAsync });
			Steps.Add(new AnalysisStep() { Label = "Formatting calls", TaskFactory = callFormatModule.FormatKeyFramesAsync});
			InitializeComponent();
		}


		private async Task DelayAsync(CancellationToken CancellationToken, ProjectViewModel Project, IDataSource DataSource, string Path)
		{
			if (CancellationToken.IsCancellationRequested) throw new Exception("Analysis canceled");
			await Task.Delay(1000);
		}

		private async Task ExtractDevicesAsync(CancellationToken CancellationToken, ProjectViewModel Project, IDataSource DataSource, string Path)
		{

			await foreach(Device device in DataSource.EnumerateDevicesAsync(Path))
			{
				Project.Devices.Add(device);
			}
		}
		private async Task ExtractMessagesAsync(CancellationToken CancellationToken, ProjectViewModel Project, IDataSource DataSource, string Path)
		{

			await foreach (Message message in DataSource.EnumerateMessagesAsync(Path))
			{
				Project.Messages.Add(message);
			}
		}


		private async Task RunAnalyzisAsync(CancellationToken CancellationToken, ProjectViewModel Project)
		{
			int fileCount;
			AnalysisStep step;
			IDataSource dataSource;

			// actually, only supported datasource
			dataSource = new OracleOEMDataSource();

			fileCount = Project.SourceFiles.Count; ;
			Project.Clear();

			for (int stepIndex = 0; stepIndex < Steps.Count; stepIndex++)
			{
				Steps[stepIndex].Init(fileCount);
			}

			for (int stepIndex = 0; stepIndex < Steps.Count; stepIndex++)
			{
				step = Steps[stepIndex];
				step.Begin();
				for(int t=0;t< fileCount; t++)
				{
					step.Update(t);
					try
					{
						if (step.TaskFactory == null) await Task.Delay(1000);
						else await step.TaskFactory(CancellationToken,Project,dataSource, Project.SourceFiles[t].Path);
					}
					catch(Exception ex)
					{
						logger.Log(0, "AnalyzeWindow", "RunAnalyzisAsync", ex);
						step.End(ex.Message);
						break;
					}
				}
				if (step.Status!=StepStatuses.Error) step.End();
			}
			

			terminated = true;
			System.Windows.Input.CommandManager.InvalidateRequerySuggested();
		}

		#region events
		private void OKCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = terminated;
		}

		private void OKCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void CancelCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = !terminated;
		}

		private void CancelCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (cancelToken == null) return;
			cancelToken.Cancel();
		}


		protected override void OnClosing(CancelEventArgs e)
		{
			e.Cancel = !terminated;
		}
		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if ((Project == null) || (cancelToken==null))
			{
				terminated = true;
				return;
			}
			await RunAnalyzisAsync(cancelToken.Token,Project) ;
		}
		#endregion


	}
}
