using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.ViewModels
{
	public abstract class FilterViewModel : ViewModel
	{
		public abstract string Description
		{
			get;
		}
		protected FilterViewModel() : base(NullLogger.Instance)
		{
		}

		public abstract void CopyFrom(FilterViewModel Other);

		public abstract bool Match(SIPMessageViewModel MessageViewModel);
	}
}
