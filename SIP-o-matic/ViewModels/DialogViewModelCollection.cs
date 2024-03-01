using LogLib;
using SIP_o_matic.corelib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class DialogViewModelCollection : ListViewModel<Dialog, DialogViewModel>
	{
		public DialogViewModelCollection(ILogger Logger) : base(Logger)
		{
		}

		protected override DialogViewModel OnCreateItem()
		{
			return new DialogViewModel(Logger);
		}

		public bool ContainsDialogForMessage(MessageViewModel Message)
		{
			return this.FirstOrDefault(item => item.Match(Message.SIPMessage)) != null;
		}
		public bool ContainsCheckedDialogForMessage(MessageViewModel Message)
		{
			DialogViewModel? item;

			item=this.FirstOrDefault(item => item.Match(Message.SIPMessage));
			if (item == null) return false;
			return item.IsChecked;
		}

		public void Add(Dialog Dialog)
		{
			DialogViewModel dialogViewModel;

			Model.Add(Dialog);

			dialogViewModel = new DialogViewModel(Logger);
			dialogViewModel.Load(Dialog);

			AddInternal(dialogViewModel);

		}
	}
}
