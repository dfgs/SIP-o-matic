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
		protected FilterViewModel() : base(NullLogger.Instance)
		{
		}


	}
}
