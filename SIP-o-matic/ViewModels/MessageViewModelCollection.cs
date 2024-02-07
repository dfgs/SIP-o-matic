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
	public class MessageViewModelCollection : ListViewModel<Message, MessageViewModel>
	{
		public MessageViewModelCollection(ILogger Logger) : base(Logger)
		{
		}

		protected override MessageViewModel OnCreateItem()
		{
			return new MessageViewModel(Logger);
		}
		public void Clear()
		{
			Model.Clear();
			ClearInternal();
		}

		public void Add(Message Message)
		{
			MessageViewModel messageViewModel;

			Model.Add(Message);

			messageViewModel=new MessageViewModel(Logger);
			messageViewModel.Load(Message);
			AddInternal(messageViewModel);
		}


	}
}
