﻿using LogLib;
using SIP_o_matic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class TelephonyEventViewModel : ViewModel<TelephonyEvent>
	{
		public TelephonyEventViewModel(ILogger Logger) : base(Logger)
		{
		}
	}
}
