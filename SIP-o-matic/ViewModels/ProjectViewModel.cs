﻿using EthernetFrameReaderLib;
using LogLib;
using PcapngFile;
using SIP_o_matic.DataSources;
using SIP_o_matic.Models;
using SIP_o_matic.Views;
using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
    public class ProjectViewModel: ViewModel<Project>
	{

		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(ProjectViewModel), new PropertyMetadata("new project"));
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}

		public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(ProjectViewModel), new PropertyMetadata(null));
		public string Path
		{
			get { return (string)GetValue(PathProperty); }
			set { SetValue(PathProperty, value); }
		}

		public SourceFileViewModelCollection SourceFiles
		{
			get;
			private set;
		}
				
		public DeviceViewModelCollection Devices
		{
			get;
			private set;
		}

		public MessageViewModelCollection Messages
		{
			get;
			private set;
		}

		
		public KeyFrameViewModelCollection KeyFrames
		{
			get;
			private set;
		}


		public ProjectViewModel(ILogger Logger):base(Logger)
		{
			SourceFiles = new SourceFileViewModelCollection(Logger);
			Devices = new DeviceViewModelCollection(Logger);
			Messages = new MessageViewModelCollection(Logger);
			KeyFrames = new KeyFrameViewModelCollection(Logger);
		}

		protected override void OnLoaded()
		{
			base.OnLoaded();
			SourceFiles.Load(Model.SourceFiles);
			Devices.Load(Model.Devices);
			Messages.Load(Model.Messages);
			KeyFrames.Load(Model.KeyFrames);
		}

		public void ClearKeyFrames()
		{
			KeyFrames.Clear();
		}

		public async Task SaveAsync(string Path)
		{
			if (Path == null) throw new ArgumentNullException(nameof(Path));

			this.Path = Path;
			this.Name = System.IO.Path.GetFileName(Path);
			await TryAsync(() => Model.SaveAsync(Path)).OrThrow("Failed to save project file");

		}

		public async Task LoadAsync(string Path)
		{
			Project? project = null;

			await TryAsync(() => Project.LoadAsync(Path)).Then(result => project = result).OrThrow("Failed to open project");
			Load(project!);
		}


	}
}
