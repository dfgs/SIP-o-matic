using LogLib;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SIP_o_matic.ViewModels.PathNodeViewModels;

namespace SIP_o_matic.ViewModels
{
	public class FolderNodeViewModel : PathNodeViewModel,IFolderNodeProvider
	{


		public static readonly DependencyProperty NodesProperty = DependencyProperty.Register("Nodes", typeof(ObservableCollection<PathNodeViewModel>), typeof(FolderNodeViewModel));
		public ObservableCollection<PathNodeViewModel> Nodes
		{
			get { return (ObservableCollection<PathNodeViewModel>)GetValue(NodesProperty); }
			set { SetValue(NodesProperty, value); }
		}



		public FolderNodeViewModel() : base()
		{
			Nodes = new ObservableCollection<PathNodeViewModel>();
		}
		public FolderNodeViewModel? FindFolderNode(string Name)
		{
			return Nodes.OfType<FolderNodeViewModel>().FirstOrDefault(item => item.Name == Name);
		}

		
		public void AddPathNode(PathNodeViewModel PathNode)
		{
			Nodes.Add(PathNode);
		}


	}
}
