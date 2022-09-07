using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.ViewModels
{
	public class SetupSessionTrigger : SessionTrigger
	{
		public string SourceAddress
		{
			get;
			private set;
		}
		public ushort SourcePort
		{
			get;
			private set;
		}
		public string DestinationAddress
		{
			get;
			private set;
		}
		public ushort DestinationPort
		{
			get;
			private set;
		}

		public string Codec
		{
			get;
			private set;
		}

		public SetupSessionTrigger(DateTime Timestamp,string SourceAddress, ushort SourcePort, string DestinationAddress, ushort DestinationPort, string Codec) : base(Timestamp)
		{
			this.SourceAddress = SourceAddress; this.SourcePort = SourcePort; this.DestinationAddress = DestinationAddress; this.DestinationPort = DestinationPort;
			this.Codec = Codec;
		}
	}
}
