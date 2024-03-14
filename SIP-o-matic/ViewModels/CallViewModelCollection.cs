using LogLib;
using SIP_o_matic.corelib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class CallViewModelCollection : GenericViewModelList<Call, CallViewModel>
	{
		private IDeviceNameProvider deviceNameProvider;
		public CallViewModelCollection(IList<Call> Source, IDeviceNameProvider DeviceNameProvider) : base(Source)
		{
			this.deviceNameProvider = DeviceNameProvider;
		}

		protected override CallViewModel OnCreateItem(Call SourceItem)
		{
			return new CallViewModel(SourceItem,deviceNameProvider);
		}


	}
}
