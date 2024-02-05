using LogLib;
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
	public class SourceFileViewModelCollection : ListViewModel<SourceFile, SourceFileViewModel>
	{
		public SourceFileViewModelCollection(ILogger Logger) : base(Logger)
		{
		}
		protected override SourceFileViewModel OnCreateItem()
		{
			return new SourceFileViewModel(Logger);
		}

		public void Add(string Path)
		{
			SourceFile sourceFile;
			SourceFileViewModel? sourceFileViewModel;

			if (Path == null) return;

			sourceFileViewModel = this.FirstOrDefault(item => item.Path == Path);
			if (sourceFileViewModel != null)
			{
				Log(LogLevels.Warning, $"Source file with path {Path} already exists in project");
				return;
			}

			sourceFile = new SourceFile() { Path = Path };
			Model.Add(sourceFile);

			sourceFileViewModel = new SourceFileViewModel(Logger);
			sourceFileViewModel.Load(sourceFile);
			AddInternal(sourceFileViewModel);

		}

		public void RemoveSelected()
		{
			SourceFile? sourceFile;

			if (SelectedItem == null) return;

			sourceFile=Model.FirstOrDefault(item=>item.Path == SelectedItem.Path);
			if (sourceFile == null)
			{
				Log(LogLevels.Warning, $"No source file with path {SelectedItem.Path} was found");
				return;
			}

			Model.Remove(sourceFile);
			RemoveInternal(SelectedItem);

		}

		
	}
}
