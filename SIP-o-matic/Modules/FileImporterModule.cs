﻿using LogLib;
using ModuleLib;
using SIP_o_matic.corelib.DataSources;
using SIP_o_matic.corelib.Models;
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

		private List<IDataSource> dataSources;
		private string fileSource;

		public FileImporterModule(ILogger Logger, ProjectViewModel Project,string FileSource, IEnumerable<string> FileNames) : base(Logger)
		{
			ProgressStep step;


			if (Project == null) throw new ArgumentNullException(nameof(Project));
			if (FileNames == null) throw new ArgumentNullException(nameof(FileNames));
			if (FileSource == null) throw new ArgumentNullException(nameof(FileSource));

			this.project = Project;
			this.fileSource = FileSource;
			this.fileNames = FileNames.ToArray();
			dataSources = new List<IDataSource>();

			progressSteps = new List<ProgressStep>();

			step = new ProgressStep() { Label = "Import file", TaskFactory = ImportFilesAsync };
			step.MaximumGetter = () => fileNames.Length;
			step.Init();
			progressSteps.Add(step);

			step = new ProgressStep() { Label="Extract devices", TaskFactory = ExtractDevicesAsync };
			step.MaximumGetter = () => fileNames.Length;
			step.Init();
			progressSteps.Add(step);

			step = new ProgressStep() { Label = "Extract messages", TaskFactory = ExtractMessagesAsync };
			step.MaximumGetter = () => fileNames.Length;
			step.Init();
			progressSteps.Add(step);

			/*step = new ProgressStep() { Label = "Add files to project", TaskFactory = AddFileToProjectAsync };
			step.MaximumGetter = () => fileNames.Length;
			step.Init();
			progressSteps.Add(step);*/

		}

		private async Task DelayAsync(CancellationToken CancellationToken,  int Index)
		{
			if (CancellationToken.IsCancellationRequested) throw new Exception("Analysis canceled");
			await Task.Delay(1000);
		}
		
		private async Task ImportFilesAsync(CancellationToken CancellationToken, int Index)
		{
			IDataSource dataSource;


			switch (fileSource)
			{
				case "EOM":
					dataSource = new OracleOEMDataSource();
					break;
				case "Alcatel":
					dataSource = new AlcatelSIPTraceDataSource();
					break;
				case "SIP":
					dataSource = new GenericSIPDataSource();
					break;
				default:
					dataSource = new OracleOEMDataSource();
					break;
			}
			dataSources.Add(dataSource);
			await dataSource.LoadAsync(fileNames[Index]);
		}

		private async Task ExtractDevicesAsync(CancellationToken CancellationToken,  int Index)
		{
			IDataSource dataSource;


			dataSource = dataSources[Index];
			foreach (Device device in dataSource.EnumerateDevices())
			{
				if (CancellationToken.IsCancellationRequested)
				{
					Log(LogLevels.Information, "Task cancelled");
					return;
				}
				project.Devices.Add(device);
				await Task.Delay(1);
			}
		}

		private async Task ExtractMessagesAsync(CancellationToken CancellationToken,  int Index)
		{
			IDataSource dataSource;

			dataSource = dataSources[Index];
			foreach (Message message in dataSource.EnumerateMessages())
			{
				if (CancellationToken.IsCancellationRequested)
				{
					Log(LogLevels.Information, "Task cancelled");
					return;
				}
				Log(LogLevels.Debug, $"Adding message:\r\n{message.Content}");
				project.Messages.Add(message);
				await Task.Delay(1);
			}
		}

		/*private async Task AddFileToProjectAsync(CancellationToken CancellationToken,  int Index)
		{
			if (CancellationToken.IsCancellationRequested)
			{
				Log(LogLevels.Information, "Task cancelled");
				return;
			}

			project.SourceFiles.Add(fileNames[Index]);
			await Task.Delay(100);
		}*/




	}
}
