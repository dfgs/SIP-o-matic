using LogLib;
using PcapngFile;
using SIP_o_matic.corelib.DataSources;
using SIP_o_matic.corelib.Models;
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
using System.Xml.Serialization;
using ViewModelLib;
using Address = SIP_o_matic.corelib.Models.Address;

namespace SIP_o_matic.ViewModels
{
	public class ProjectViewModel : GenericViewModel<Project>, IDeviceNameProvider
	{
		public event EventHandler? DeviceNameUpdated;


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


		public static readonly DependencyProperty DevicesProperty = DependencyProperty.Register("Devices", typeof(DeviceViewModelCollection), typeof(ProjectViewModel), new PropertyMetadata(null));
		public DeviceViewModelCollection Devices
		{
			get { return (DeviceViewModelCollection)GetValue(DevicesProperty); }
			private set { SetValue(DevicesProperty, value); }
		}





		public static readonly DependencyProperty MessagesProperty = DependencyProperty.Register("Messages", typeof(MessageViewModelCollection), typeof(ProjectViewModel), new PropertyMetadata(null));
		public MessageViewModelCollection Messages
		{
			get { return (MessageViewModelCollection)GetValue(MessagesProperty); }
			private set { SetValue(MessagesProperty, value); }
		}




		public static readonly DependencyProperty KeyFramesProperty = DependencyProperty.Register("KeyFrames", typeof(KeyFrameViewModelCollection), typeof(ProjectViewModel), new PropertyMetadata(null));
		public KeyFrameViewModelCollection KeyFrames
		{
			get { return (KeyFrameViewModelCollection)GetValue(KeyFramesProperty); }
			set { SetValue(KeyFramesProperty, value); }
		}


		public static readonly DependencyProperty DialogsProperty = DependencyProperty.Register("Dialogs", typeof(DialogViewModelCollection), typeof(ProjectViewModel), new PropertyMetadata(null));
		public DialogViewModelCollection Dialogs
		{
			get { return (DialogViewModelCollection)GetValue(DialogsProperty); }
			private set { SetValue(DialogsProperty, value); }
		}

		public static readonly DependencyProperty EventsFrameProperty = DependencyProperty.Register("EventsFrame", typeof(EventsFrameViewModel), typeof(ProjectViewModel), new PropertyMetadata(null));
		public EventsFrameViewModel EventsFrame
		{
			get { return (EventsFrameViewModel)GetValue(EventsFrameProperty); }
			set { SetValue(EventsFrameProperty, value); }
		}

		public ProjectViewModel(Project Model) : base(Model)
		{
			Devices = new DeviceViewModelCollection(Model.Devices);
			Messages = new MessageViewModelCollection(Model.Messages,this);
			KeyFrames = new KeyFrameViewModelCollection(Model.KeyFrames,this);
			EventsFrame = new EventsFrameViewModel(Model.MessagesFrame, this);
			Dialogs = new DialogViewModelCollection(Model.Dialogs,this);
		}

		public void RefreshDeviceAndMessages()
		{
			Devices = new DeviceViewModelCollection(Model.Devices);
			Messages = new MessageViewModelCollection(Model.Messages, this);
			Dialogs = new DialogViewModelCollection(Model.Dialogs, this);
		}

		public void RefreshFrames()
		{
			KeyFrames = new KeyFrameViewModelCollection(Model.KeyFrames,this);
			EventsFrame = new EventsFrameViewModel(Model.MessagesFrame, this);
		}

		

		public async Task SaveAsync(string Path)
		{
			if (Path == null) throw new ArgumentNullException(nameof(Path));

			this.Path = Path;
			this.Name = System.IO.Path.GetFileName(Path);
			await TryAsync(() => Model.SaveAsync(Path)).OrThrow("Failed to save project file");

		}

		public static async Task<ProjectViewModel> LoadAsync(string Path)
		{
			Project model;
			ProjectViewModel project;

			model = await Project.LoadAsync(Path);
			project = new ProjectViewModel(model);
			project.Path = Path;
			project.Name= System.IO.Path.GetFileName(Path);
			
			return project;
		}
		public async Task ExportSIPAsync(string Path)
		{
			if (Path == null) throw new ArgumentNullException(nameof(Path));

			await TryAsync(() => Model.ExportSIPAsync(Path)).OrThrow("Failed to export project file");
		}

		public void UpdateAddresses(string OldValue, string NewValue)
		{
			foreach (DialogViewModel dialog in Dialogs)
			{
				if (dialog.SourceAddress.Value == OldValue) dialog.SourceAddress = new Address(NewValue);
				if (dialog.DestinationAddress.Value == OldValue) dialog.DestinationAddress = new Address(NewValue);
			}
			foreach (MessageViewModel message in Messages)
			{
				if (message.SourceAddress.Value == OldValue) message.SourceAddress = new Address(NewValue);
				if (message.DestinationAddress.Value == OldValue) message.DestinationAddress = new Address(NewValue);
			}
		}

		public void RenameDevice(DeviceViewModel Device,string Name)
		{
			Device.Name = Name;
			if (DeviceNameUpdated != null) DeviceNameUpdated(this, EventArgs.Empty);
		}

		public DeviceViewModel GetDevice(Address Address)
		{
			DeviceViewModel? device;
			device=Devices.FirstOrDefault(item=>item.Addresses.Contains(Address));
			if (device == null) device=new DeviceViewModel(new Device(Address.ToString(),new Address[] {Address }));
			return device;
		}

		public DeviceViewModel GetDevice(Device Model)
		{
			DeviceViewModel? device;
			device = Devices.FirstOrDefault(item => item.GetModel()==Model);
			if (device == null) device = new DeviceViewModel(Model);
			return device;
		}
		public SIPMessage? GetSIPMessage(Message Message)
		{
			int index;
			index = Model.Messages.IndexOf(Message);
			return Model.SIPMessages[index];
		}

		public SDP? GetSDPBody(Message Message)
		{
			int index;
			index = Model.Messages.IndexOf(Message);
			return Model.SDPBodies[index];
		}


		public void RemoveDevice(DeviceViewModel Device)
		{
			Devices.Remove(Device);
			if (DeviceNameUpdated != null) DeviceNameUpdated(this, EventArgs.Empty);
		}

		public void RemoveAddress(AddressViewModel Address)
		{
			Devices.Remove(Address);
			if (DeviceNameUpdated != null) DeviceNameUpdated(this, EventArgs.Empty);
		}

		public void AddDevice(Device Device)
		{
			Devices.Add(Device);
			if (DeviceNameUpdated != null) DeviceNameUpdated(this, EventArgs.Empty);
		}

		public void AddAddressToDevice(DeviceViewModel Device,Address Address)
		{
			Device.Addresses.Add(new AddressViewModel(Address));
			if (DeviceNameUpdated != null) DeviceNameUpdated(this, EventArgs.Empty);
		}

		public Project GetModel()
		{
			return Model;
		}

		
	}
}
