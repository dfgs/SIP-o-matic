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
	public class TelephonyEventViewModelCollection : ListViewModel<TelephonyEvent, TelephonyEventViewModel>
	{
		public TelephonyEventViewModelCollection(ILogger Logger) : base(Logger)
		{
		}

		protected override TelephonyEventViewModel OnCreateItem()
		{
			return new TelephonyEventViewModel(Logger);
		}
		public void Clear()
		{
			Model.Clear();
			ClearInternal();
		}
		public void Add(TelephonyEvent TelephonyEvent)
		{
			TelephonyEventViewModel telephonyEventViewModel;

			Model.Add(TelephonyEvent);

			telephonyEventViewModel = new TelephonyEventViewModel(Logger);
			telephonyEventViewModel.Load(TelephonyEvent);
			AddInternal(telephonyEventViewModel);
		}

	}
}
