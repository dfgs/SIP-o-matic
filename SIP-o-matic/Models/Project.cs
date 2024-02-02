using SIP_o_matic.DataSources;
using SIP_o_matic.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using ViewModelLib;

namespace SIP_o_matic.Models
{
	public class Project
	{
		

		public ObservableCollection<SourceFile> SourceFiles
		{
			get;
			set;
		}

		public Project()
		{
			SourceFiles = new ObservableCollection<SourceFile>();
		}



	}
}
