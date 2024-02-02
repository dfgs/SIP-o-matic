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
				

		/*public ViewModelCollection<DeviceViewModel> Devices
		{
			get;
			private set;
		}*/

		public ProjectViewModel(ILogger Logger,Project DataSource):base(Logger,DataSource)
		{
			SourceFiles = new SourceFileViewModelCollection(Logger,DataSource.SourceFiles);
			//Devices = new ViewModelCollection<DeviceViewModel>(Logger);
		}
		
		

		/*private DeviceViewModel? FindDeviceByName(string Name)
		{
			return Devices.FirstOrDefault(item => item.Name == Name);
		}
		private DeviceViewModel? FindDeviceByAddress(string Address)
		{
			return Devices.FirstOrDefault(item => item.Addresses.Contains(Address));
		}


		private void AddDevice(SourceFileViewModel FileViewModel, Device Device)
		{
			DeviceViewModel? deviceViewModel;

			deviceViewModel = FindDeviceByName(Device.Name);
			if (deviceViewModel == null)
			{
				deviceViewModel = new DeviceViewModel(Logger);
				deviceViewModel.Load(Device);
				Devices.Add(deviceViewModel);
			}
			
		}
		private void AddDevice(SourceFileViewModel FileViewModel, string Address)
		{
			DeviceViewModel? deviceViewModel;
			Device device;

			deviceViewModel = FindDeviceByAddress(Address);
			if (deviceViewModel == null)
			{
				device = new Device() { Name = Address };
				device.Addresses.Add(Address);
				deviceViewModel = new DeviceViewModel(Logger);
				deviceViewModel.Load(device);
				Devices.Add(deviceViewModel);
			}
			
		}*/

		
		



	}
}
