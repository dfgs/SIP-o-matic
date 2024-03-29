using LogLib;
using SIP_o_matic.corelib.Models;
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
	public class ApplicationViewModel: GenericViewModel<string>
	{


		public ProjectViewModelCollection Projects
		{
			get;
			private set;
		}

		
		public ApplicationViewModel(string Model):base(Model)
		{
			Projects = new ProjectViewModelCollection(new List<Project>());
		}
		

		public void CloseCurrentProject()
		{
			if (Projects.SelectedItem == null) return;
			Projects.Remove(Projects.SelectedItem);
		}
	}
}
