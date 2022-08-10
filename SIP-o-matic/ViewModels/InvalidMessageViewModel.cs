using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.ViewModels
{
	public class InvalidMessageViewModel:SIPMessageViewModel
	{
		private string message;
		private string errorMessage;

		public override string Display => errorMessage;

		public InvalidMessageViewModel(int UID, DateTime Timestamp,string Message,string ErrorMessage):base(UID,Timestamp, "Undefined","Undefined")
		{
			this.message = Message;
			this.errorMessage = ErrorMessage;
		}
	}
}
