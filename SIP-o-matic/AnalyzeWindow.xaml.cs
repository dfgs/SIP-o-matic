using SIP_o_matic.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
	/// Logique d'interaction pour AnalyzeWindow.xaml
	/// </summary>
	public partial class AnalyzeWindow : Window
	{
		private bool terminated = false;



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
			InitializeComponent();
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			await RunAnalyzisAsync();
		}

		private async Task RunAnalyzisAsync()
		{
			int fileCount;

			fileCount = 10;


			Steps[0].Begin(fileCount);
			for(int t=0;t< fileCount; t++)
			{
				Steps[0].Update(t);
				await Task.Delay(1000);
			}
			Steps[0].End("Error message");

			Steps[1].Begin(fileCount);
			for (int t = 0; t < fileCount; t++)
			{
				Steps[1].Update(t);
				await Task.Delay(1000);
			}
			Steps[1].End();





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


		/*private override void OnClosing(object sender, CancelEventArgs e)
		{
			e.Cancel = true;
			//Do whatever you want here..
		}*/


	}
}
