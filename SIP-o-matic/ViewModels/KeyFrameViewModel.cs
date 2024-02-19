using LogLib;
using SIP_o_matic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class KeyFrameViewModel : ViewModel<KeyFrame>
	{
		public DateTime Timestamp
		{
			get => Model.Timestamp;
		}

		public CallViewModelCollection Calls
		{
			get;
			private set;
		}

		public IEnumerable<string> Devices
		{
			get => Calls.SelectMany(call=>call.Devices).Distinct();
		}

		
		public TimeSpan TimeSpan
		{
			get;
			set;
		}



		public static readonly DependencyProperty TimeSpanDisplayProperty = DependencyProperty.Register("TimeSpanDisplay", typeof(string), typeof(KeyFrameViewModel), new PropertyMetadata(null));
		public string TimeSpanDisplay
		{
			get { return (string)GetValue(TimeSpanDisplayProperty); }
			set { SetValue(TimeSpanDisplayProperty, value); }
		}

				
		public uint MessageIndex
		{
			get => Model.MessageIndex;
		}



		public KeyFrameViewModel(ILogger Logger) : base(Logger)
		{
			Calls = new CallViewModelCollection(Logger);
		}

		protected override void OnLoaded()
		{
			base.OnLoaded();
			Calls.Load(Model.Calls);

			
		}

	}
}
