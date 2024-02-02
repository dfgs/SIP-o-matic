using LogLib;
using SIP_o_matic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class ProjectViewModelCollection : ViewModelCollection<ObservableCollection<Project>,ProjectViewModel>
	{
		public ProjectViewModelCollection(ILogger Logger, ObservableCollection<Project> DataSource) : base(Logger,DataSource)
		{
		}

		public void AddNew()
		{
			Project project;
			ProjectViewModel projectViewModel;

			project= new Project();
			projectViewModel = new ProjectViewModel(Logger, project);
			AddInternal(projectViewModel);

			SelectedItem= projectViewModel;
		}

	}
}
