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

		public FileImporterModule(ILogger Logger, ProjectViewModel Project,IEnumerable<string> FileNames) : base(Logger)
		{
			ProgressStep step;


			if (Project == null) throw new ArgumentNullException(nameof(Project));
			if (FileNames == null) throw new ArgumentNullException(nameof(FileNames));

			this.project = Project;
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
			string extension;

			extension = System.IO.Path.GetExtension(fileNames[Index]);
			if (extension.ToLower() == ".sip")
			{
				dataSource = new GenericSIPDataSource();
			}
			else
			{
				dataSource = new OracleOEMDataSource();
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
			MessageViewModel messageViewModel;

			dataSource = dataSources[Index];
			foreach (Message message in dataSource.EnumerateMessages())
			{
				if (CancellationToken.IsCancellationRequested)
				{
					Log(LogLevels.Information, "Task cancelled");
					return;
				}

				messageViewModel=project.Messages.Add(message);

				messageViewModel.SourceDevice = project.Devices.FindDeviceByAddress(messageViewModel.SourceAddress)?.Name ?? messageViewModel.SourceAddress;
				messageViewModel.DestinationDevice = project.Devices.FindDeviceByAddress(messageViewModel.DestinationAddress)?.Name ?? messageViewModel.DestinationAddress;

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
