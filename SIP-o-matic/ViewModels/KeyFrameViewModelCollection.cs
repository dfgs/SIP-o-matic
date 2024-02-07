using LogLib;
using SIP_o_matic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class KeyFrameViewModelCollection : ListViewModel<KeyFrame, KeyFrameViewModel>
	{
		public KeyFrameViewModelCollection(ILogger Logger) : base(Logger)
		{
		}

		protected override KeyFrameViewModel OnCreateItem()
		{
			return new KeyFrameViewModel(Logger);
		}

		public void Clear()
		{
			Model.Clear();
			ClearInternal();
		}
		public void Add(KeyFrame KeyFrame)
		{
			KeyFrameViewModel keyFrameViewModel;

			Model.Add(KeyFrame);

			keyFrameViewModel = new KeyFrameViewModel(Logger);
			keyFrameViewModel.Load(KeyFrame);
			AddInternal(keyFrameViewModel);
		}


	}
}
