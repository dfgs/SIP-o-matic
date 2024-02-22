using SIP_o_matic.corelib.DataSources;
using SIP_o_matic.corelib.Models;
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
using SIP_o_matic.corelib.Models.Transactions;
using SIP_o_matic.Modules;


namespace SIP_o_matic
{
	/// <summary>
	/// Logique d'interaction pour AnalyzeWindow.xaml
	/// </summary>
	public partial class ProgressWindow : Window
	{
		private bool terminated = false;
		private CancellationTokenSource? cancelToken;

		public static readonly DependencyProperty StepsProperty = DependencyProperty.Register("Steps", typeof(List<ProgressStep>), typeof(ProgressWindow), new PropertyMetadata(null));
		public List<ProgressStep> Steps
		{
			get { return (List<ProgressStep>)GetValue(StepsProperty); }
			set { SetValue(StepsProperty, value); }
		}

		private ILogger logger;



		public ProgressWindow(ILogger Logger)
		{
			this.logger = Logger; 

			cancelToken = new CancellationTokenSource();

			Steps = new List<ProgressStep>();
			InitializeComponent();
		}


		

		private async Task RunStepsAsync(CancellationToken CancellationToken)
		{
			ProgressStep step;

			for (int stepIndex = 0; stepIndex < Steps.Count; stepIndex++)
			{
				step = Steps[stepIndex];
				//step.Init();
				step.Begin();
				for(int t=0;t< step.Maximum; t++)
				{
					step.Update(t);
					try
					{
						if (step.TaskFactory == null) await Task.Delay(1000);
						else await step.TaskFactory(CancellationToken,t);
					}
					catch(Exception ex)
					{
						logger.Log(0, "ProgressWindow", "RunStepsAsync", ex);
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
			if (cancelToken==null)
			{
				terminated = true;
				return;
			}
			await RunStepsAsync(cancelToken.Token) ;
		}
		#endregion


	}
}
