using LogLib;
using SIP_o_matic.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic
{
	public static class Consts
	{
		public static KeyFrameViewModelCollection KeyFrames
		{
			get;
			set;
		}


		static Consts()
		{

			KeyFrames = KeyFrameViewModelCollection.CreateTestData();
			

		}

	}
}
