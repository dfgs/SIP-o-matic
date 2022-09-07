using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.ViewModels
{
	public abstract class SessionTrigger
	{
		public DateTime Timestamp
		{
			get;
			private set;
		}

		public SessionTrigger(DateTime Timestamp)		{
			this.Timestamp = Timestamp;
		}

	}
}
