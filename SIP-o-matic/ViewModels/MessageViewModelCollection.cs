using LogLib;
using SIP_o_matic.corelib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class MessageViewModelCollection : GenericViewModelList<Message, MessageViewModel>
	{


		

		public MessageViewModelCollection(IList<Message> Source, IDeviceNameProvider DeviceNameProvider) : base(Source,(SourceItem)=>new MessageViewModel(SourceItem,DeviceNameProvider) )
		{
		}


		
		
		

	

	



	}
}
