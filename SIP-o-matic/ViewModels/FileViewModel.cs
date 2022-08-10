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


		public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(FileViewModel), new PropertyMetadata(null));
		public string Path
		{
			get { return (string)GetValue(PathProperty); }
			set { SetValue(PathProperty, value); }
		}

		public string Name
		{
			get
			{
				if (Path == null) return "Undefined";
				return System.IO.Path.GetFileName(Path);
			}
		}

		public static readonly DependencyProperty MessagesProperty = DependencyProperty.Register("Messages", typeof(ObservableCollection<string>), typeof(FileViewModel), new PropertyMetadata(null));
		public ObservableCollection<string> Messages
		{
			get { return (ObservableCollection<string>)GetValue(MessagesProperty); }
			set { SetValue(MessagesProperty, value); }
		}

		public FileViewModel()
		{
			Messages = new ObservableCollection<string>();

		}

	}
}
