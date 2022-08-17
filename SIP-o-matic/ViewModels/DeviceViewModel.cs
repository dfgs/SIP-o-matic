using LogLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.ViewModels
{
	public class DeviceViewModel : ViewModel
	{
		public string Name
		{
			get;
			private set;
		}
		public ObservableCollection<string> Addresses
		{
			get;
			private set;
		}
		public ObservableCollection<FileViewModel> SourceFiles
		{
			get;
			private set;
		}

		public int Count
		{
			get => SourceFiles.Count;
		}



		public DeviceViewModel(string Name) : base(NullLogger.Instance)
		{
			this.Name = Name;
			Addresses = new ObservableCollection<string>();
			SourceFiles = new ObservableCollection<FileViewModel>();
		}
		protected void OnPropertiesChanged()
		{
			OnPropertyChanged(nameof(Count));

		}

		public void AddSourceFile(FileViewModel FileViewModel)
		{
			SourceFiles.Add(FileViewModel);
			OnPropertiesChanged();
		}

		public void RemoveSourceFile(FileViewModel FileViewModel)
		{
			SourceFiles.Remove(FileViewModel);
			OnPropertiesChanged();
		}


	}
}
