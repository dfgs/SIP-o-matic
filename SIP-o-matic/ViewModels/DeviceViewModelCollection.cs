using GongSolutions.Wpf.DragDrop;
using LogLib;
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
	public class DeviceViewModelCollection : GenericViewModelList<Device, DeviceViewModel>,IDropTarget
	{


		public static readonly DependencyProperty SelectedDeviceOrAddressProperty = DependencyProperty.Register("SelectedDeviceOrAddress", typeof(object), typeof(DeviceViewModelCollection), new PropertyMetadata(null));
		public object SelectedDeviceOrAddress
		{
			get { return (object)GetValue(SelectedDeviceOrAddressProperty); }
			set { SetValue(SelectedDeviceOrAddressProperty, value); }
		}




		public DeviceViewModelCollection(IList<Device> Source) : base(Source)
		{
		}
		protected override DeviceViewModel OnCreateItem(Device SourceItem)
		{
			return new DeviceViewModel(SourceItem);
		}


		public DeviceViewModel? FindDeviceByName(string Name)
		{
			return this.FirstOrDefault(item => item.Name == Name);
		}
		public DeviceViewModel? FindDeviceByAddress(Address Address)
		{
			return this.FirstOrDefault(item => item.Addresses.Contains( Address));
		}

		
		public void Add(Device Device)
		{
			DeviceViewModel? deviceViewModel;
			Device newDevice;

			deviceViewModel = FindDeviceByName(Device.Name);
			if (deviceViewModel ==null)
			{
				newDevice=new Device() { Name = Device.Name };
				Source.Add(newDevice);

				deviceViewModel = OnCreateItem(newDevice);
				AddInternal(deviceViewModel);
			}

			foreach(Address address in Device.Addresses) 
			{
				deviceViewModel.Addresses.Add(new AddressViewModel(address));
			}
			
			
		}
		

		public void Remove(AddressViewModel Address)
		{
			DeviceViewModel? device;

			device = this.FirstOrDefault(item => item.Addresses.Contains(Address));
			if (device == null) return;

			device.Addresses.Remove(Address);
		}

		public void DragOver(IDropInfo dropInfo)
		{
			DeviceViewModel? sourceItem = dropInfo.Data as DeviceViewModel;
			DeviceViewModel? targetItem = dropInfo.TargetItem as DeviceViewModel;
			
			if ((sourceItem != null) && (targetItem != null) )
			{
				dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
				dropInfo.Effects = DragDropEffects.Move;
			}
		}

		public void Drop(IDropInfo dropInfo)
		{
			DeviceViewModel? sourceItem = dropInfo.Data as DeviceViewModel;
			DeviceViewModel? targetItem = dropInfo.TargetItem as DeviceViewModel;

			if ((sourceItem == null) || (targetItem == null)) return;
			RemoveInternal(sourceItem);
			InsertInternal(sourceItem, IndexOf(targetItem));

		}


	}
}
