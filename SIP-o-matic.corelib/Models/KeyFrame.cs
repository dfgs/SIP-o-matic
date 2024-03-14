using SIP_o_matic.corelib.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.corelib.Models
{

	public class KeyFrame:ICloneable<KeyFrame>
	{
		public required DateTime Timestamp
		{
			get;
			set;
		}
		public TimeSpan TimeSpan
		{
			get;
			set;
		}
		public string TimeSpanDisplay
		{
			get;
			set;
		}
		public List<Call> Calls
		{
			get;
			set;
		}
		
		public uint MessageIndex
		{
			get;
			set;
		}

		[SetsRequiredMembers]
		public KeyFrame(DateTime Timestamp)
		{
			this.Calls= new List<Call>();
			this.Timestamp = Timestamp;
			this.TimeSpan = TimeSpan.Zero;
			this.TimeSpanDisplay = "";
		}

		
		public KeyFrame Clone()
		{
			KeyFrame keyFrame;

			keyFrame = new KeyFrame(this.Timestamp);
			foreach (Call previousCall in this.Calls)
			{
				keyFrame.Calls.Add(previousCall.Clone());
			}
			
			return keyFrame;
		}


	}
}
