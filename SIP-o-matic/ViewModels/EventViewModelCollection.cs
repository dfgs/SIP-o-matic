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

		public override int GetNewItemIndex(IEventViewModel Item)
		{
			if (Count == 0) return 0;
			if (Item.Timestamp > this[Count - 1].Timestamp) return Count;
			for(int t=0;t<Count;t++)
			{
				if (Item.Timestamp < this[t].Timestamp) return t;
			}
			return Count;
		}


		private static IEventViewModel CreateEvent(IEvent Item,IDeviceNameProvider DeviceNameProvider)
		{
			switch(Item)
			{
				case Message message:return new MessageViewModel(message, DeviceNameProvider);
				case RTPStart rtpStart:return new RTPStartViewModel(rtpStart, DeviceNameProvider);
				default:throw new InvalidOperationException();
			}
		}
		
		
		

	

	



	}
}
