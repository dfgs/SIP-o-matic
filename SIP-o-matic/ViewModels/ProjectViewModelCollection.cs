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
	public class ProjectViewModelCollection : ListViewModel<Project, ProjectViewModel>
	{
		public ProjectViewModelCollection(ILogger Logger) : base(Logger)
		{
		}

		protected override ProjectViewModel OnCreateItem()
		{
			return new ProjectViewModel(Logger);
		}

		public void AddNew()
		{
			Project project;
			ProjectViewModel projectViewModel;

			project= new Project();
			projectViewModel = new ProjectViewModel(Logger);
			projectViewModel.Load(project);

			AddInternal(projectViewModel);

			SelectedItem= projectViewModel;
		}

		public void Remove(ProjectViewModel Item)
		{
			if (Item == null) return;
			RemoveInternal(Item);
			SelectedItem = this.FirstOrDefault();
		}
		public async Task AddAsync(string Path)
		{
			ProjectViewModel projectViewModel;

			projectViewModel = new ProjectViewModel(Logger);
			AddInternal(projectViewModel);
			SelectedItem = projectViewModel;

			await projectViewModel.LoadAsync(Path);
		}

	}
}
