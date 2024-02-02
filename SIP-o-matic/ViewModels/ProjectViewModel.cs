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
    public class ProjectViewModel: ViewModel<string>
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

		public ViewModelCollection<SourceFileViewModel> Files
		{
			get;
			private set;
		}
				

		public ViewModelCollection<DeviceViewModel> Devices
		{
			get;
			private set;
		}

		
		

	

		public ProjectViewModel(ILogger Logger):base(Logger)
		{
			Files = new ViewModelCollection<SourceFileViewModel>(Logger);
			Devices = new ViewModelCollection<DeviceViewModel>(Logger);
		}

		
		private DeviceViewModel? FindDeviceByName(string Name)
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
			//int addressIndex;

			deviceViewModel = FindDeviceByName(Device.Name);
			if (deviceViewModel == null)
			{
				deviceViewModel = new DeviceViewModel(Logger);
				deviceViewModel.Load(Device);
				Devices.Add(deviceViewModel);
				//FileViewModel.AddDevice(Device);
			}

			/*addressIndex = deviceViewModel.Addresses.IndexOf(Device.Address);
			if (addressIndex >= 0) return;

			deviceViewModel.Addresses.Add(Device.Address);*/
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
			
		}
	
		public async Task AddFileAsync(string Path,IDataSource DataSource)
		{
			SourceFile sourceFile;
			SourceFileViewModel fileViewModel;

			sourceFile = new SourceFile() { Path=Path};

			fileViewModel = new SourceFileViewModel(Logger);
			fileViewModel.Load(sourceFile);
			
			Files.Add(fileViewModel);
						
			await foreach (Device device in DataSource.EnumerateDevicesAsync(Path))
			{
				AddDevice(fileViewModel, device);
			}

			await foreach (Message message in DataSource.EnumerateMessagesAsync(Path))
			{
				fileViewModel.Events.Add(message);
				AddDevice(fileViewModel, message.SourceAddress);
				AddDevice(fileViewModel, message.DestinationAddress);
			}
	
		}

		

		
	}
}
