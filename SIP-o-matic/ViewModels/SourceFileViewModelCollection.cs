using LogLib;
using SIP_o_matic.DataSources;
using SIP_o_matic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class SourceFileViewModelCollection : ViewModelCollection<ObservableCollection<SourceFile>,SourceFileViewModel>
	{
		public SourceFileViewModelCollection(ILogger Logger, ObservableCollection<SourceFile> Source) : base(Logger,Source)
		{
		}

		public void Add(string Path)
		{
			SourceFile sourceFile;
			SourceFileViewModel? sourceFileViewModel;

			if (Path == null) return;

			sourceFileViewModel = this.FirstOrDefault(item => item.Path == Path);
			if (sourceFileViewModel != null) return;

			sourceFile = new SourceFile() { Path = Path };
			DataSource.Add(sourceFile);

			sourceFileViewModel = new SourceFileViewModel(Logger,sourceFile);
			AddInternal(sourceFileViewModel);
		}

		public void RemoveSelected()
		{
			if (SelectedItem == null) return;

			DataSource.Remove(SelectedItem.DataSource);

			RemoveInternal(SelectedItem);

		}



	}
}
