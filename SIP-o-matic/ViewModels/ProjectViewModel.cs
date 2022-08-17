using EthernetFrameReaderLib;
using LogLib;
using PcapngFile;
using SIP_o_matic.DataSources;
using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SIP_o_matic.ViewModels
{
	public class ProjectViewModel: ViewModel
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

		public ObservableCollection<FileViewModel> Files
		{
			get;
			private set;
		}

		public static readonly DependencyProperty SelectedFileProperty = DependencyProperty.Register("SelectedFile", typeof(FileViewModel), typeof(ProjectViewModel), new PropertyMetadata(null));
		public FileViewModel? SelectedFile
		{
			get { return (FileViewModel)GetValue(SelectedFileProperty); }
			set { SetValue(SelectedFileProperty, value); }
		}

		public ObservableCollection<CallViewModel> Calls
		{
			get;
			private set;
		}

		public static readonly DependencyProperty SelectedCallProperty = DependencyProperty.Register("SelectedCall", typeof(CallViewModel), typeof(ProjectViewModel), new PropertyMetadata(null));
		public CallViewModel? SelectedCall
		{
			get { return (CallViewModel)GetValue(SelectedCallProperty); }
			set { SetValue(SelectedCallProperty, value); }
		}

		public ObservableCollection<DeviceViewModel> Devices
		{
			get;
			private set;
		}

		public static readonly DependencyProperty SelectedDeviceProperty = DependencyProperty.Register("SelectedDevice", typeof(DeviceViewModel), typeof(ProjectViewModel), new PropertyMetadata(null));
		public DeviceViewModel? SelectedDevice
		{
			get { return (DeviceViewModel)GetValue(SelectedDeviceProperty); }
			set { SetValue(SelectedDeviceProperty, value); }
		}


		public ProjectViewModel(ILogger Logger):base(Logger)
		{
			Files = new ObservableCollection<FileViewModel>();
			Calls = new ObservableCollection<CallViewModel>();
			Devices = new ObservableCollection<DeviceViewModel>();
		}

		protected void OnPropertiesChanged()
		{
			
		}


		public CallViewModel? FindCallByUID(int UID)
		{
			return Calls.FirstOrDefault(item => item.UID == UID);
		}
		public DeviceViewModel? FindDeviceByName(string Name)
		{
			return Devices.FirstOrDefault(item => item.Name == Name);
		}
		public void AddDevice(FileViewModel FileViewModel, Device Device)
		{
			DeviceViewModel? deviceViewModel;
			int addressIndex;

			deviceViewModel = FindDeviceByName(Device.Name);
			if (deviceViewModel == null)
			{
				deviceViewModel = new DeviceViewModel(Device.Name);
				Devices.Add(deviceViewModel);
			}

			deviceViewModel.AddSourceFile(FileViewModel);

			addressIndex = deviceViewModel.Addresses.IndexOf(Device.Address);
			if (addressIndex >= 0) return;

			deviceViewModel.Addresses.Add(Device.Address);
			
		}

		public void AddEvent(FileViewModel FileViewModel,Event Event)
		{
			CallViewModel? callViewModel;
			int callUID;
			SIPMessage sipMessage;

			try
			{
				sipMessage = SIPGrammar.SIPMessage.Parse(Event.Message, ' ');
			}
			catch (Exception ex)
			{
				Log(LogLevels.Error, ex.Message+" / "+Event.Message.ReplaceLineEndings(@"\r\n"));
				return;
			}

			callUID = SIPUtils.GetCallUID(sipMessage);

			callViewModel=FindCallByUID(callUID);

			if (callViewModel == null)
			{
				callViewModel=new CallViewModel(Logger,callUID);
				Calls.Add(callViewModel);
			}
			callViewModel.AddSIPMessage(FileViewModel,Event, sipMessage);

		}
		public void RemoveEvent(FileViewModel FileViewModel, Event Event)
		{
			CallViewModel? callViewModel;
			int callUID;
			SIPMessage sipMessage;

			try
			{
				sipMessage = SIPGrammar.SIPMessage.Parse(Event.Message, ' ');
			}
			catch (Exception ex)
			{
				Log(LogLevels.Error, ex.Message);
				return;
			}

			callUID = SIPUtils.GetCallUID(sipMessage);

			callViewModel = FindCallByUID(callUID);

			if (callViewModel == null) return;
			
			callViewModel.RemoveSIPMessage(FileViewModel, Event, sipMessage);
			if (callViewModel.Dialogs.Count == 0) Calls.Remove(callViewModel);

		}
		public void RemoveDevice(FileViewModel FileViewModel, Device Device)
		{
			DeviceViewModel? deviceViewModel;

			deviceViewModel = FindDeviceByName(Device.Name);
			if (deviceViewModel == null) return;

			deviceViewModel.RemoveSourceFile(FileViewModel);

			if (deviceViewModel.Count == 0) Devices.Remove(deviceViewModel);
		}
		public async Task AddFileAsync(string Path)
		{
			IDataSource dataSource;
			FileViewModel fileViewModel;

			fileViewModel = new FileViewModel(Logger);
			fileViewModel.Path = Path;
			Files.Add(fileViewModel);

			//dataSource = new WiresharkDataSource(Path);
			dataSource = new OracleDataSource(Path);

			await foreach (Device _device in dataSource.EnumerateDevicesAsync())
			{
				fileViewModel.Devices.Add(_device);
				AddDevice(fileViewModel, _device);
			}

			await foreach (Event _event in dataSource.EnumerateEventsAsync())
			{
				fileViewModel.Events.Add(_event);
				AddEvent(fileViewModel, _event);
			}
			OnPropertiesChanged();
		}
		public async Task RemoveFileAsync(FileViewModel FileViewModel)
		{


			await foreach (Event _event in FileViewModel.Events.ToAsyncEnumerable())
			{
				RemoveEvent(FileViewModel, _event);
			}
			await foreach (Device _device in FileViewModel.Devices.ToAsyncEnumerable())
			{
				RemoveDevice(FileViewModel, _device);
			}

			SelectedFile = null;
			Files.Remove(FileViewModel);
			OnPropertiesChanged();

		}






	}
}
