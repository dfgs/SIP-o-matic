using LogLib;
using SIP_o_matic.DataSources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SIP_o_matic.ViewModels
{
	public class DialogEventViewModel : LadderEventViewModel
	{


		public static readonly DependencyProperty DisplayProperty = DependencyProperty.Register("Display", typeof(string), typeof(DialogEventViewModel), new PropertyMetadata(null));
		public override string Display
		{
			get { return (string)GetValue(DisplayProperty); }
			set { SetValue(DisplayProperty, value); }
		}

		public ObservableCollection<TransactionEventViewModel> TransactionEvents
		{
			get;
			private set;
		}


		public override string BorderColor 
		{
			get => "Blue";
		}
		public static readonly DependencyProperty EventColorProperty = DependencyProperty.Register("EventColor", typeof(string), typeof(DialogEventViewModel), new PropertyMetadata("Blue"));
		public override string EventColor
		{
			get { return (string)GetValue(EventColorProperty); }
			set { SetValue(EventColorProperty, value); }
		}

		


		public DialogEventViewModel() : base()
		{
			TransactionEvents = new ObservableCollection<TransactionEventViewModel>();
		}

		public void AddEvent(TransactionEventViewModel TransactionEvent)
		{
			TransactionEvent.DialogEvent= this;
			for (int t = 0; t < TransactionEvents.Count; t++)
			{
				if (TransactionEvents[t].Timestamp > TransactionEvent.Timestamp)
				{
					TransactionEvents.Insert(t, TransactionEvent);
					return;
				}
			}
			TransactionEvents.Add(TransactionEvent);
		}

	}
}
