using LogLib;
using PcapngFile;
using SIP_o_matic.corelib.DataSources;
using SIP_o_matic.corelib.Models;
using SIP_o_matic.Views;
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

namespace SIP_o_matic.ViewModels
{
	public class ProjectViewModel : ViewModel<Project>, IDeviceNameProvider
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

		public DialogViewModelCollection Dialogs
		{
			get;
			private set;
		}

		public static readonly DependencyProperty MessagesFrameProperty = DependencyProperty.Register("MessagesFrame", typeof(MessagesFrameViewModel), typeof(ProjectViewModel), new PropertyMetadata(null));
		public MessagesFrameViewModel MessagesFrame
		{
			get { return (MessagesFrameViewModel)GetValue(MessagesFrameProperty); }
			set { SetValue(MessagesFrameProperty, value); }
		}




		public ProjectViewModel(ILogger Logger):base(Logger)
		{
			Devices = new DeviceViewModelCollection(Logger);
			Messages = new MessageViewModelCollection(Logger,this);
			KeyFrames = new KeyFrameViewModelCollection(Logger);
			MessagesFrame = new MessagesFrameViewModel(Logger,this);
			Dialogs = new DialogViewModelCollection(Logger,this);
		}

		protected override void OnLoaded()
		{
			base.OnLoaded();
			Devices.Load(Model.Devices);
			Messages.Load(Model.Messages);
			KeyFrames.Load(Model.KeyFrames);
			MessagesFrame.Load("");
			Dialogs.Load(Model.Dialogs);
		}

		public void ClearKeyFrames()
		{
			KeyFrames.Clear();
			MessagesFrame.Clear();
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

			this.Path = Path;
			this.Name = System.IO.Path.GetFileName(Path);

			await TryAsync(() => Project.LoadAsync(Path)).Then(result => project = result).OrThrow("Failed to open project");
			Load(project!);
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

		public string GetDeviceName(Address Address)
		{
			if (Address == null) return "Undefined";
			return this.Devices.FindDeviceByAddress(Address)?.Name ?? Address.Value;
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
			Device.Addresses.Add(Address);
			if (DeviceNameUpdated != null) DeviceNameUpdated(this, EventArgs.Empty);
		}

	}
}
