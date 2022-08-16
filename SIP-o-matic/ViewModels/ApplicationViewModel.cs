using LogLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SIP_o_matic.ViewModels
{
	public class ApplicationViewModel:ViewModel
	{


		public ObservableCollection<ProjectViewModel> Projects
		{
			get;
			private set;
		}

		

		public static readonly DependencyProperty SelectedProjectProperty = DependencyProperty.Register("SelectedProject", typeof(ProjectViewModel), typeof(ApplicationViewModel), new PropertyMetadata(null));
		public ProjectViewModel? SelectedProject
		{
			get { return (ProjectViewModel)GetValue(SelectedProjectProperty); }
			set { SetValue(SelectedProjectProperty, value); }
		}




		public ApplicationViewModel():base(NullLogger.Instance)
		{

			Projects = new ObservableCollection<ProjectViewModel>();
		}


		public async Task AddProjectAsync()
		{
			ProjectViewModel project;
			ProjectLogger logger;

			await Task.Yield();
			logger = new ProjectLogger();
			project = new ProjectViewModel(logger);
			Projects.Add(project);
			SelectedProject = project;
		}

	}
}
