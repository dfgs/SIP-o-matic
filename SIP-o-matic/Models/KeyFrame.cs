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

		public List<Call> Calls
		{
			get; 
			set;
		}

		[SetsRequiredMembers]
		public KeyFrame(DateTime Timestamp)
		{
			this.Calls= new List<Call>();
			this.Timestamp = Timestamp;
		}

		[SetsRequiredMembers]
		public KeyFrame(DateTime Timestamp,KeyFrame? PreviousKeyFrame)
		{
			this.Calls = new List<Call>();
			this.Timestamp = Timestamp;

			if (PreviousKeyFrame == null) return;

			foreach (Call previousCall in PreviousKeyFrame.Calls)
			{
				Calls.Add(previousCall.Clone());
			}

		}

	}
}
