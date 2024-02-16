using LogLib;
using ModuleLib;
using SIP_o_matic.DataSources;
using SIP_o_matic.Models;
using SIP_o_matic.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SIP_o_matic.Modules
{
	public class FileImporterModule : BaseProgressModule
	{
		private List<ProgressStep> progressSteps;
		public override IEnumerable<ProgressStep> ProgressSteps
		{
			get => progressSteps;
		}

		private string[] fileNames;
		private ProjectViewModel project;

		public FileImporterModule(ILogger Logger, ProjectViewModel Project,IEnumerable<string> FileNames) : base(Logger)
		{
			ProgressStep step;


			if (Project == null) throw new ArgumentNullException(nameof(Project));
			if (FileNames == null) throw new ArgumentNullException(nameof(FileNames));

			this.project = Project;
			this.fileNames = FileNames.ToArray();


			progressSteps = new List<ProgressStep>();

			step = new ProgressStep() { Label="Extract devices", TaskFactory = ExtractDevicesAsync };
			step.MaximumGetter = () => fileNames.Length;
			step.Init();
			progressSteps.Add(step);

			step = new ProgressStep() { Label = "Extract messages", TaskFactory = ExtractMessagesAsync };
			step.MaximumGetter = () => fileNames.Length;
			step.Init();
			progressSteps.Add(step);

			step = new ProgressStep() { Label = "Add files to project", TaskFactory = AddFileToProjectAsync };
			step.MaximumGetter = () => fileNames.Length;
			step.Init();
			progressSteps.Add(step);

		}

		private async Task DelayAsync(CancellationToken CancellationToken,  int Index)
		{
			if (CancellationToken.IsCancellationRequested) throw new Exception("Analysis canceled");
			await Task.Delay(1000);
		}

		private async Task ExtractDevicesAsync(CancellationToken CancellationToken,  int Index)
		{
			IDataSource dataSource;

			
			dataSource = new OracleOEMDataSource();
			await foreach (Device device in dataSource.EnumerateDevicesAsync(fileNames[Index]))
			{
				if (CancellationToken.IsCancellationRequested)
				{
					Log(LogLevels.Information, "Task cancelled");
					return;
				}
				project.Devices.Add(device);
			}
		}

		private async Task ExtractMessagesAsync(CancellationToken CancellationToken,  int Index)
		{
			IDataSource dataSource;
			

			dataSource = new OracleOEMDataSource();
			await foreach (Message message in dataSource.EnumerateMessagesAsync(fileNames[Index]))
			{
				if (CancellationToken.IsCancellationRequested)
				{
					Log(LogLevels.Information, "Task cancelled");
					return;
				}
				project.Messages.Add(message);
			}
		}

		private async Task AddFileToProjectAsync(CancellationToken CancellationToken,  int Index)
		{
			if (CancellationToken.IsCancellationRequested)
			{
				Log(LogLevels.Information, "Task cancelled");
				return;
			}

			project.SourceFiles.Add(fileNames[Index]);
			await Task.Delay(100);
		}




	}
}
