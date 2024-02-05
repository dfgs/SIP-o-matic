using LogLib;
using SIP_o_matic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
    public class SourceFileViewModel:ViewModel<SourceFile>
	{


		public string Path
		{
			get => Model.Path;
		}

		public string Name
		{
			get => Model.Name;
		}

		public ObservableCollection<Message> Events
		{
			get;
			private set;
		}
		/*public ObservableCollection<Device> Devices
		{
			get;
			private set;
		}*/
		public SourceFileViewModel(ILogger Logger):base(Logger)
		{
			Events = new ObservableCollection<Message>();
			//Devices = new ObservableCollection<Device>();
		}
		protected override void OnLoaded()
		{
			base.OnLoaded();
			//Events.Load(Model.Messages);
		}

		/*public void AddDevice(Device Device)
		{
			if (Devices.Contains(Device)) return;
			Devices.Add(Device);
		}*/


	}
}
