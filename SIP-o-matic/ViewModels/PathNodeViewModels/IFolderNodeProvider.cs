using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.ViewModels.PathNodeViewModels
{
	public interface IFolderNodeProvider
	{
		FolderNodeViewModel? FindFolderNode(string Name);
		void AddPathNode(PathNodeViewModel PathNode);
	}
}
