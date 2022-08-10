using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.ViewModels
{
	public abstract class SIPMessageViewModel:ViewModel
	{
		public abstract string Display
		{
			get;
		}
		public string From
		{
			get;
			private set;
		}
		public string To
		{
			get;
			private set;
		}

		public int UID
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

		public SIPMessageViewModel(int UID,string From,string To)
		{
			SourceFiles = new ObservableCollection<FileViewModel>();
			this.UID = UID; this.From = From;this.To = To;
		}


		public void AddSourceFile(FileViewModel File)
		{
			SourceFiles.Add(File);
			OnPropertyChanged("Count");
		}

		public void RemoveSourceFile(FileViewModel File)
		{
			SourceFiles.Remove(File);
			OnPropertyChanged("Count");
		}
	}
}
