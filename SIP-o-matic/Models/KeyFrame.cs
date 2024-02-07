using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.Models
{
	public class KeyFrame
	{
		public required DateTime Timestamp
		{
			get;
			set;
		}

		[SetsRequiredMembers]
		public KeyFrame(DateTime Timestamp)
		{
			this.Timestamp = Timestamp;
		}

	}
}
