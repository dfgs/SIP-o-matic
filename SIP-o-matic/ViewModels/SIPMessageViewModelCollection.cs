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
	public class SIPMessageViewModelCollection : GenericViewModelList<ISIPMessage, SIPMessageViewModel>
	{


		

		public SIPMessageViewModelCollection(IList<ISIPMessage> Source, IDeviceNameProvider DeviceNameProvider) : base(Source,(SourceItem)=>new SIPMessageViewModel(SourceItem,DeviceNameProvider) )
		{
		}


		
		
		

	

	



	}
}
