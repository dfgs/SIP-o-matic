using LogLib;
using ModuleLib;
using SIP_o_matic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.Modules
{
	public abstract class BaseProgressModule : Module
	{

		public abstract IEnumerable<ProgressStep> ProgressSteps
		{
			get;
		}

		protected BaseProgressModule(ILogger Logger) : base(Logger)
		{
		}
	}
}
