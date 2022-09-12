using EthernetFrameReaderLib;
using LogLib;
using PcapngFile;
using SIP_o_matic.DataSources;
using SIP_o_matic.ViewModels.PathNodeViewModels;
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

namespace SIP_o_matic.ViewModels
{
	public class ProjectViewModel: ViewModel, IFolderNodeProvider
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

		public ObservableCollection<PathNodeViewModel> Folders
		{
			get;
			private set;
		}

		public ObservableCollection<FilterViewModel> Filters
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

		public static readonly DependencyProperty LadderProperty = DependencyProperty.Register("Ladder", typeof(LadderViewModel), typeof(ProjectViewModel), new PropertyMetadata(null));
		public LadderViewModel? Ladder
		{
			get { return (LadderViewModel)GetValue(LadderProperty); }
			set { SetValue(LadderProperty, value); }
		}
		public static readonly DependencyProperty SelectedFilterProperty = DependencyProperty.Register("SelectedFilter", typeof(FilterViewModel), typeof(ProjectViewModel), new PropertyMetadata(null));
		public FilterViewModel? SelectedFilter
		{
			get { return (FilterViewModel)GetValue(SelectedFilterProperty); }
			set { SetValue(SelectedFilterProperty, value); }
		}


		public ProjectViewModel():base(NullLogger.Instance)
		{
			Ladder = new LadderViewModel();
			Devices = new ObservableCollection<DeviceViewModel>();
			Devices.Add(TestData.DeviceA);
			Devices.Add(TestData.DeviceB);
			Devices.Add(TestData.DeviceC);
			Devices.Add(TestData.DeviceD);
			Files = new ObservableCollection<FileViewModel>();
			Folders = new ObservableCollection<PathNodeViewModel>();
			Calls = new ObservableCollection<CallViewModel>();
			Filters = new ObservableCollection<FilterViewModel>();
		}

		public ProjectViewModel(ILogger Logger):base(Logger)
		{
			Ladder = new LadderViewModel(Logger);
			Files = new ObservableCollection<FileViewModel>();
			Folders = new ObservableCollection<PathNodeViewModel>();
			Calls = new ObservableCollection<CallViewModel>();
			Devices = new ObservableCollection<DeviceViewModel>();
			Filters = new ObservableCollection<FilterViewModel>();
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
		public DeviceViewModel? FindDeviceByAddress(string Address)
		{
			return Devices.FirstOrDefault(item => item.Addresses.Contains(Address));
		}

		public FolderNodeViewModel? FindFolderNode(string Name)
		{
			return Folders.OfType<FolderNodeViewModel>().FirstOrDefault(item => item.Name == Name);
		}

		public void AddPathNode(PathNodeViewModel PathNode)
		{
			Folders.Add(PathNode);
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
				FileViewModel.AddDevice(Device);
			}

			deviceViewModel.AddSourceFile(FileViewModel);

			addressIndex = deviceViewModel.Addresses.IndexOf(Device.Address);
			if (addressIndex >= 0) return;

			deviceViewModel.Addresses.Add(Device.Address);
		}
		public void AddDevice(FileViewModel FileViewModel, string Address)
		{
			DeviceViewModel? deviceViewModel;
			Device device;

			deviceViewModel = FindDeviceByAddress(Address);
			if (deviceViewModel == null)
			{
				
				deviceViewModel = new DeviceViewModel(Address);
				deviceViewModel.Addresses.Add(Address);
				Devices.Add(deviceViewModel);


				device = new Device(Address,Address);
				FileViewModel.AddDevice(device);
			}

			deviceViewModel.AddSourceFile(FileViewModel);
			
		}

		public void AddEvent(FileViewModel FileViewModel,Event Event)
		{
			CallViewModel? callViewModel;
			int callUID;
			SIPMessage sipMessage;
			SDP? sdp;

			try
			{
				sipMessage = SIPGrammar.SIPMessage.Parse(Event.Message, ' ');
			}
			catch (Exception ex)
			{
				Log(LogLevels.Error, ex.Message+" / "+Event.Message.ReplaceLineEndings(@"\r\n"));
				return;
			}

			sdp = null;
			if (!string.IsNullOrEmpty(sipMessage.Body ))
			{
				try
				{
					sdp = SDPGrammar.SDP.Parse(sipMessage.Body);
				}
				catch(Exception ex)
				{
					Log(LogLevels.Error, ex.Message + " / " + sipMessage.Body.ReplaceLineEndings(@"\r\n"));
					return;
				}
			}


			callUID = SIPUtils.GetCallUID(sipMessage);

			callViewModel=FindCallByUID(callUID);

			if (callViewModel == null)
			{
				callViewModel=new CallViewModel(Logger,callUID);
				Calls.Add(callViewModel);
			}
			callViewModel.AddSIPMessage(FileViewModel,Event, sipMessage,sdp);

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
		public async Task AddFileAsync(string Path,IDataSource DataSource)
		{
			string[] folders;
			FileViewModel fileViewModel;
			FolderNodeViewModel? folderNodeViewModel;
			IFolderNodeProvider currentProvider;

			fileViewModel = new FileViewModel(Logger,Path);
			Files.Add(fileViewModel);

			currentProvider = this;
			folders = Path.Split(System.IO.Path.DirectorySeparatorChar);
			foreach(string folder in folders.SkipLast(1))
			{
				folderNodeViewModel = currentProvider.FindFolderNode(folder);
				if (folderNodeViewModel==null)
				{
					folderNodeViewModel = new FolderNodeViewModel();
					folderNodeViewModel.Name = folder;
					currentProvider.AddPathNode(folderNodeViewModel);
				}
				currentProvider = folderNodeViewModel;
			}
			currentProvider.AddPathNode(new FileNodeViewModel() { Name=fileViewModel.Name });

			await foreach (Device _device in DataSource.EnumerateDevicesAsync(Path))
			{
				AddDevice(fileViewModel, _device);
			}

			await foreach (Event _event in DataSource.EnumerateEventsAsync(Path))
			{
				fileViewModel.Events.Add(_event);
				AddDevice(fileViewModel, _event.SourceAddress);
				AddDevice(fileViewModel, _event.DestinationAddress);
				

				AddEvent(fileViewModel, _event);
			}

			await foreach(CallViewModel call in Calls.ToAsyncEnumerable())
			{
				call.Analyze();
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

			if (SelectedFile==FileViewModel) SelectedFile = null;
			Files.Remove(FileViewModel);
			
			await foreach (CallViewModel call in Calls.ToAsyncEnumerable())
			{
				call.Analyze();
			}

			OnPropertiesChanged();

		}

		public async Task AddFilterAsync(FilterViewModel Filter)
		{
			await Task.Yield();
			Filters.Add(Filter);
		}
		public async Task RemoveFilterAsync(FilterViewModel Filter)
		{
			await Task.Yield();
			if (SelectedFilter == Filter) SelectedFilter = null;
			Filters.Remove(Filter);
		}

	}
}
