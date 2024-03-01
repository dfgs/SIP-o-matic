using LogLib;
using SIP_o_matic.corelib;
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
	public class DialogViewModel : ViewModel<Dialog>,ISIPMessageMatch
	{



		public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(DialogViewModel), new PropertyMetadata(false));
		public bool IsChecked
		{
			get { return (bool)GetValue(IsCheckedProperty); }
			set { SetValue(IsCheckedProperty, value); }
		}




		public DateTime TimeStamp
		{
			get => Model.TimeStamp;
		}

		public string CallID
		{
			get => Model.CallID;
		}

		public string SourceDevice
		{
			get=>Model.SourceDevice;
		}



		public string DestinationDevice
		{
			get=> Model.DestinationDevice;
		}

		public IEnumerable<string> Devices
		{
			get
			{
				yield return SourceDevice;
				yield return DestinationDevice;
			}
		}
		public string Caller
		{
			get=>Model.Caller;
		}

		public string Callee
		{
			get => Model.Callee;
		}
		
			

		
		public DialogViewModel(ILogger Logger) : base(Logger)
		{
			
		}

		public bool Match(ISIPMessage MessageInfo)
		{
			return Model.Match(MessageInfo);
		}



	}
}
