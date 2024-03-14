using LogLib;
using SIP_o_matic.corelib.Models;
using SIP_o_matic.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewModelLib;

namespace SIP_o_matic.ViewModels
{
	public class KeyFrameViewModel : GenericViewModel<KeyFrame>
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

		public IEnumerable<DeviceViewModel> Devices
		{
			get => Calls.SelectMany(call=>call.Devices).Distinct();
		}

		
		public TimeSpan TimeSpan
		{
			get => Model.TimeSpan;
		}



		public string TimeSpanDisplay
		{
			get => Model.TimeSpanDisplay;
		}

				
		public uint MessageIndex
		{
			get => Model.MessageIndex;
		}


		private IDeviceNameProvider deviceNameProvider;

		public KeyFrameViewModel(KeyFrame Model, IDeviceNameProvider DeviceNameProvider) : base(Model)
		{
			this.deviceNameProvider = DeviceNameProvider;
			Calls = new CallViewModelCollection(Model.Calls,deviceNameProvider);
		}

		

		internal static KeyFrameViewModel CreateTestData()
		{
			KeyFrameViewModel keyFrame;
			KeyFrame model;

			model = new KeyFrame(DateTime.Now);
			model.MessageIndex = 1;
			model.Calls.Add(new Call("abc-def1",new Device("SBC1",new Address[] { }) , new Device("SBC2", new Address[] { }), "tagabc", "+33144556677", "+33699884455", Call.States.OnHook, false));
			model.Calls.Add(new Call("abc-def2", new Device("SBC2", new Address[] { }), new Device("SBC3", new Address[] { }), "tagabc", "+33144556677", "+33699884455", Call.States.OnHook, false));

			keyFrame = new KeyFrameViewModel(model,null);

			return keyFrame;
		}
	}
}
