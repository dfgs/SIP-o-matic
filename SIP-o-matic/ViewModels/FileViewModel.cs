using LogLib;
using SIP_o_matic.DataSources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SIP_o_matic.ViewModels
{
	public class FileViewModel:ViewModel
	{


		public string Path
		{
			get;
			private set;
		}

		public string Name
		{
			get
			{
				if (Path == null) return "Undefined";
				return System.IO.Path.GetFileName(Path);
			}
		}

		public ObservableCollection<Event> Events
		{
			get;
			private set;
		}
		public ObservableCollection<Device> Devices
		{
			get;
			private set;
		}
		public FileViewModel(ILogger Logger,string Path):base(Logger)
		{
			Events = new ObservableCollection<Event>();
			Devices = new ObservableCollection<Device>();
			this.Path = Path;
		}

		public void AddDevice(Device Device)
		{
			if (Devices.Contains(Device)) return;
			Devices.Add(Device);
		}


	}
}
