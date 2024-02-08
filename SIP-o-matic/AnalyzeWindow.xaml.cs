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


		public static readonly DependencyProperty ProjectProperty = DependencyProperty.Register("Project", typeof(ProjectViewModel), typeof(AnalyzeWindow), new PropertyMetadata(null));
		public ProjectViewModel Project
		{
			get { return (ProjectViewModel)GetValue(ProjectProperty); }
			set { SetValue(ProjectProperty, value); }
		}

		public required ILogger Logger
		{
			get;
			set;
		}



		public AnalyzeWindow()
		{
			cancelToken = new CancellationTokenSource();

			Steps = new List<AnalysisStep>();
			Steps.Add(new AnalysisStep() { Label = "Extracting devices",TaskFactory= ExtractDevicesAsync });
			Steps.Add(new AnalysisStep() { Label = "Extracting sip messages",TaskFactory= ExtractMessagesAsync });
			Steps.Add(new AnalysisStep() { Label = "Creating key frames", TaskFactory = CreateKeyFramesAsync });
			InitializeComponent();
		}

		private void UpdateKeyFrame(KeyFrame KeyFrame, Request Request, MessageViewModel Message)
		{
			string? callID;
			Address? from, to;
			Call? call;

			callID = Request.GetHeader<CallIDHeader>()?.Value;
			if (callID == null)
			{
				string error = $"CallID header missing in SIP message [{Message.Index}]";
				throw new InvalidOperationException(error);
			}

			from = Request.GetHeader<FromHeader>()?.Value;
			if (from == null)
			{
				string error = $"From header missing in SIP message [{Message.Index}]";
				throw new InvalidOperationException(error);
			}

			to = Request.GetHeader<ToHeader>()?.Value;
			if (to == null)
			{
				string error = $"To header missing in SIP message [{Message.Index}]";
				throw new InvalidOperationException(error);
			}

			call = KeyFrame.Calls.FirstOrDefault(item => item.CallID == callID);
			if (call != null) call.Update(Request);
			else
			{
				if (Request.RequestLine.Method != "INVITE") throw new InvalidOperationException($"No call to update was found, unexpected SIP message [{Message.Index}]");

				call = KeyFrame.Calls.FirstOrDefault(item => item.CallID == callID);
				if (call == null)
				{
					call = new Call(callID, Message.SourceAddress, Message.DestinationAddress, from.Value, to.Value, Call.States.OnHook);
					KeyFrame.Calls.Add(call);
				}

				// update call
				call.Update(Request);
			}
			
		}
		private void UpdateKeyFrame(KeyFrame KeyFrame, Response Response, MessageViewModel Message)
		{
			//return TelephonyEventTypes.SessionRefreshed;
		}

		private void UpdateKeyFrame(KeyFrame KeyFrame, SIPMessage SIPMessage, MessageViewModel Message)
		{

			switch (SIPMessage)
			{
				case Request request: UpdateKeyFrame(KeyFrame, request,Message) ;
					break;
				case Response response:UpdateKeyFrame(KeyFrame,response, Message);
					break;
				default: throw new InvalidOperationException("Invalid SIP message type");
			}

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
		private async Task CreateKeyFramesAsync(CancellationToken CancellationToken, ProjectViewModel Project, IDataSource DataSource, string Path)
		{
			StringReader reader;
			SIPMessage sipMessage;
			
			KeyFrame newKeyFrame;
			KeyFrame? previousKeyFrame=null;

			await foreach (MessageViewModel message in Project.Messages.ToAsyncEnumerable())
			{
				reader = new StringReader(message.Content, ' ');
				try
				{
					sipMessage = SIPGrammar.SIPMessage.Parse(reader);
				}
				catch (Exception ex)
				{
					string error = "Failed to decode SIP message:\r\n" + ex.Message + "\r\n" + message.Content;
					throw new InvalidOperationException(error);
				}

				newKeyFrame = new KeyFrame(message.Timestamp,previousKeyFrame);
				UpdateKeyFrame(newKeyFrame, sipMessage,message);

				Project.KeyFrames.Add(newKeyFrame);
				previousKeyFrame = newKeyFrame;
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
						Logger.Log(0, "AnalyzeWindow", "RunAnalyzisAsync", ex);
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
