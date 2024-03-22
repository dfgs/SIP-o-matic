using LogLib;
using SIP_o_matic.corelib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class EventViewModelCollection : GenericViewModelList<IEvent, IEventViewModel>
	{

		
		

		public EventViewModelCollection(IList<IEvent> Source, IDeviceNameProvider DeviceNameProvider) : base(Source, (SourceItem) => CreateEvent(SourceItem, DeviceNameProvider))
		{
		}

		private static IEventViewModel CreateEvent(IEvent Item,IDeviceNameProvider DeviceNameProvider)
		{
			switch(Item)
			{
				case Message message:return new MessageViewModel(message, DeviceNameProvider);
				default:throw new InvalidOperationException();
			}
		}
		
		
		

	

	



	}
}
