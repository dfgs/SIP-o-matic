﻿using LogLib;
using SIP_o_matic.corelib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class DeviceViewModelCollection : ListViewModel<Device, DeviceViewModel>
	{


		public static readonly DependencyProperty SelectedDeviceOrAddressProperty = DependencyProperty.Register("SelectedDeviceOrAddress", typeof(object), typeof(DeviceViewModelCollection), new PropertyMetadata(null));
		public object SelectedDeviceOrAddress
		{
			get { return (object)GetValue(SelectedDeviceOrAddressProperty); }
			set { SetValue(SelectedDeviceOrAddressProperty, value); }
		}




		public DeviceViewModelCollection(ILogger Logger) : base(Logger)
		{
		}
		protected override DeviceViewModel OnCreateItem()
		{
			return new DeviceViewModel(Logger);
		}


		public DeviceViewModel? FindDeviceByName(string Name)
		{
			return this.FirstOrDefault(item => item.Name == Name);
		}
		public DeviceViewModel? FindDeviceByAddress(Address Address)
		{
			return this.FirstOrDefault(item => item.Addresses.Contains( Address));
		}

		/*private DeviceViewModel? FindDeviceByAddress(string Address)
		{
			return this.FirstOrDefault(item => item.Addresses.Contains(Address));
		}*/

		public void Clear()
		{
			Model.Clear();
			ClearInternal();
		}


		/*private void AddDevice(SourceFileViewModel FileViewModel, Device Device)
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
		*/
		public void Add(Device Device)
		{
			DeviceViewModel? deviceViewModel;
			Device newDevice;

			deviceViewModel = FindDeviceByName(Device.Name);
			if (deviceViewModel ==null)
			{
				newDevice=new Device() { Name = Device.Name };
				Model.Add(newDevice);

				deviceViewModel = OnCreateItem();
				deviceViewModel.Load(newDevice);
				AddInternal(deviceViewModel);
			}

			foreach(Address address in Device.Addresses) 
			{
				deviceViewModel.Addresses.Add(address);
			}
			
			
		}

		public void Remove(DeviceViewModel Device)
		{
			Model.Remove(Device.GetModel());
			RemoveInternal(Device);
		}

		public void Remove(AddressViewModel Address)
		{
			DeviceViewModel? device;

			device = this.FirstOrDefault(item => item.Addresses.Contains(Address));
			if (device == null) return;

			device.Addresses.Remove(Address);
		}
	}
}
