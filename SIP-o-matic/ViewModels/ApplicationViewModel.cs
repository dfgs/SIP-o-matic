﻿using LogLib;
using SIP_o_matic.Models;
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


		public ProjectViewModelCollection Projects
		{
			get;
			private set;
		}

		
		public ApplicationViewModel(ILogger Logger):base(Logger,"")
		{

			Projects = new ProjectViewModelCollection(Logger,new ObservableCollection<Project>());
		}


		

	}
}
