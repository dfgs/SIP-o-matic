using LogLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SIP_o_matic.ViewModels
{
	public class TransactionEventViewModel : LadderEventViewModel
	{


		public static readonly DependencyProperty DisplayProperty = DependencyProperty.Register("Display", typeof(string), typeof(TransactionEventViewModel), new PropertyMetadata(null));
		public override string Display
		{
			get { return (string)GetValue(DisplayProperty); }
			set { SetValue(DisplayProperty, value); }
		}
		public ObservableCollection<SIPMessageEventViewModel> SIPMessageEvents
		{
			get;
			private set;
		}

		public override string BorderColor
		{
			get => "Green";
		}
		public static readonly DependencyProperty EventColorProperty = DependencyProperty.Register("EventColor", typeof(string), typeof(TransactionEventViewModel), new PropertyMetadata("Blue"));
		public override string EventColor
		{
			get { return (string)GetValue(EventColorProperty); }
			set { SetValue(EventColorProperty, value); }
		}
		public DialogEventViewModel? DialogEvent
		{
			get;
			set;
		}
		public TransactionEventViewModel() : base()
		{
			SIPMessageEvents = new ObservableCollection<SIPMessageEventViewModel>();
		}

		public void AddEvent(SIPMessageEventViewModel MessageEvent)
		{
			MessageEvent.TransactionEvent = this;
			for (int t = 0; t < SIPMessageEvents.Count; t++)
			{
				if (SIPMessageEvents[t].Timestamp > MessageEvent.Timestamp)
				{
					SIPMessageEvents.Insert(t, MessageEvent);
					return;
				}
			}
			SIPMessageEvents.Add(MessageEvent);
		}


	}
}
