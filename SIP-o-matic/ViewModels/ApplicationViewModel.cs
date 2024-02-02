using LogLib;
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
	public class ApplicationViewModel:ViewModel<string>
	{


		public ViewModelCollection<ProjectViewModel> Projects
		{
			get;
			private set;
		}

		
		public ApplicationViewModel(ILogger Logger):base(Logger)
		{

			Projects = new ViewModelCollection<ProjectViewModel>(Logger);
		}


		public async Task AddProjectAsync()
		{
			ProjectViewModel project;

			await Task.Yield();
			project = new ProjectViewModel(Logger);
			Projects.Add(project);
			Projects.SelectedItem= project; 
		}

	}
}
