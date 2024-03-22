using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.corelib.Models
{
	public class EventsFrame
	{
		public List<Device> Devices
		{
			get;
			private set;
		}

		public List<IEvent> Events
		{
			get;
			private set;
		}

		public EventsFrame()
		{
			this.Devices = new List<Device>();
			this.Events = new List<IEvent>();
		}

	}
}
