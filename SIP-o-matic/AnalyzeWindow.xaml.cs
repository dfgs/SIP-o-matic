using SIP_o_matic.Models;
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

namespace SIP_o_matic
{
	/// <summary>
	/// Logique d'interaction pour AnalyzeWindow.xaml
	/// </summary>
	public partial class AnalyzeWindow : Window
	{
		private bool terminated = false;
		private CancellationTokenSource? cancelToken;


		public static readonly DependencyProperty StepsProperty = DependencyProperty.Register("Steps", typeof(List<AnalysisStep>), typeof(AnalyzeWindow), new PropertyMetadata(null));
		public List<AnalysisStep> Steps
		{
			get { return (List<AnalysisStep>)GetValue(StepsProperty); }
			set { SetValue(StepsProperty, value); }
		}




		public AnalyzeWindow()
		{
			Steps = new List<AnalysisStep>();
			Steps.Add(new AnalysisStep() { Label = "Loading file" });
			Steps.Add(new AnalysisStep() { Label = "Extracting sip messages" });
			Steps.Add(new AnalysisStep() { Label = "Creating events" });
			InitializeComponent();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			e.Cancel = !terminated;
		}
		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			cancelToken=new CancellationTokenSource();
			await RunAnalyzisAsync(cancelToken.Token);
		}

		private async Task RunAnalyzisAsync(CancellationToken CancelToken)
		{
			int fileCount;

			if (cancelToken==null) throw new ArgumentNullException(nameof(cancelToken));

			fileCount = 10;


			for(int step=0; step < Steps.Count; step++) 
			{ 
				Steps[step].Begin(fileCount);
				for(int t=0;t< fileCount; t++)
				{
					if (cancelToken.IsCancellationRequested)
					{
						Steps[step].End("Analysis canceled");
						break;
					}
					Steps[step].Update(t);
					await Task.Delay(1000);
				}
				if (Steps[step].Status!=StepStatuses.Error) Steps[step].End();
			}
			

			terminated = true;
			System.Windows.Input.CommandManager.InvalidateRequerySuggested();
		}

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


		


	}
}
