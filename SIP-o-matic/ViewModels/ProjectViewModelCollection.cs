using LogLib;
using SIP_o_matic.corelib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class ProjectViewModelCollection : GenericViewModelList<Project, ProjectViewModel>
	{
		public ProjectViewModelCollection(IList<Project> Source) : base(Source)
		{
		}
		
		protected override ProjectViewModel OnCreateItem(Project SourceItem)
		{
			return new ProjectViewModel(SourceItem);
		}

		public void AddNew()
		{
			Project project;
			ProjectViewModel projectViewModel;

			project= new Project();
			projectViewModel = new ProjectViewModel(project);

			AddInternal(projectViewModel);

			SelectedItem= projectViewModel;
		}

		
		public async Task<ProjectViewModel> AddAsync(string Path)
		{
			ProjectViewModel projectViewModel;

			projectViewModel = await ProjectViewModel.LoadAsync(Path); 
			AddInternal(projectViewModel);
			SelectedItem = projectViewModel;
			return projectViewModel;
		}

	}
}
