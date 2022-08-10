using LogLib;
using SIP_o_matic.DataSources;
using SIPParserLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.ViewModels
{
	public class CallViewModel:ViewModel
	{
		
		public string From
		{
			get;
			private set;
		}
		public string To
		{
			get;
			private set;
		}


		public ObservableCollection<SIPMessageViewModel> SIPMessages
		{
			get;
			private set;
		}

		public int UID
		{
			get;
			private set;
		}

		public CallViewModel(ILogger Logger, int UID):base(Logger)
		{
			From = "Undefined";To = "Undefined";
			SIPMessages = new ObservableCollection<SIPMessageViewModel>();
			this.UID = UID;
		}

		public SIPMessageViewModel? FindMessageByUID(int UID)
		{
			return SIPMessages.FirstOrDefault(item => item.UID == UID);
		}

		public void AddSIPMessage(FileViewModel FileViewModel,Event Event, SIPMessage SIPMessage)
		{
			SIPMessageViewModel? sipMessageViewModel;
			int messageUID;

			messageUID = SIPUtils.GetMessageUID(Event.Message);

			sipMessageViewModel = FindMessageByUID(messageUID);

			if (sipMessageViewModel == null)
			{
				switch(SIPMessage)
				{
					case Response response:
						sipMessageViewModel = new ResponseViewModel(Logger,Event, response);
						break;
					case Request request:
						sipMessageViewModel = new RequestViewModel(Logger, Event, request);
						if (request.RequestLine.Method=="INVITE")
						{
							From = request.GetHeader<FromHeader>()?.Value.ToString() ?? "Undefined";
							To = request.GetHeader<ToHeader>()?.Value.ToString() ?? "Undefined";
						}
						break;
					default:
						return;
				}

				

				SIPMessages.Add(sipMessageViewModel);
			}

			sipMessageViewModel.AddSourceFile(FileViewModel);

		}



	}
}
