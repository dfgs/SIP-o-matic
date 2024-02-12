using SIP_o_matic.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.Models
{

	public class KeyFrame:ICloneable<KeyFrame>
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
