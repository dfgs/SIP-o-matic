using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class SDPViewModel : GenericViewModel<IBody>
	{
		public IEnumerable<SDPField> Fields
		{
			get
			{
				if (Model is SDP sdp) return sdp.Fields;
				else return Enumerable.Empty<SDPField>();
			}
		}
		public SDPViewModel(IBody Model) : base(Model)
		{
		}


	}
}
