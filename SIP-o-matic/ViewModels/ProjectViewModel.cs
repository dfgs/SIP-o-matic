using EthernetFrameReaderLib;
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


		public ProjectViewModel(ILogger Logger):base(Logger)
		{
			SourceFiles = new SourceFileViewModelCollection(Logger);
			Devices = new DeviceViewModelCollection(Logger);
		}

		protected override void OnLoaded()
		{
			base.OnLoaded();
			SourceFiles.Load(Model.SourceFiles);
			Devices.Load(Model.Devices);
		}

		public void Clear()
		{
			Devices.Clear();
		}


	}
}
