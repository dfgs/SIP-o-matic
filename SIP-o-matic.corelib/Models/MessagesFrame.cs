using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.corelib.Models
{
	public class MessagesFrame
	{
		public List<Device> Devices
		{
			get;
			private set;
		}

		public List<Message> Messages
		{
			get;
			private set;
		}

		public MessagesFrame()
		{
			this.Devices = new List<Device>();
			this.Messages = new List<Message>();
		}

	}
}
