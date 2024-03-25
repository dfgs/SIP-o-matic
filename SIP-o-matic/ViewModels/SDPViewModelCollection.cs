using LogLib;
using SIP_o_matic.corelib.Models;
using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class SDPViewModelCollection : GenericViewModelList<IBody, SDPViewModel>
	{


		

		public SDPViewModelCollection(IList<IBody> Source) : base(Source,(SourceItem)=>new SDPViewModel(SourceItem) )
		{
		}


		
		
		

	

	



	}
}
