using SIP_o_matic.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.Views.TestData
{
	internal static class TestData
	{
		public static DeviceViewModel DeviceA = new DeviceViewModel("Device A");
		public static DeviceViewModel DeviceB = new DeviceViewModel("Device B");
		public static DeviceViewModel DeviceC = new DeviceViewModel("Device C");
		public static DeviceViewModel DeviceD = new DeviceViewModel("Device D");

		public static LadderEventViewModel Event1 = new DialogEventViewModel() { SourceDevice = DeviceA, DestinationDevice = DeviceB , Display="INVITE", Timestamp = DateTime.Parse("10:00:00") , IsExpanded=true };
		public static LadderEventViewModel Event2 = new TransactionEventViewModel() { SourceDevice = DeviceA, DestinationDevice = DeviceB, Display = "INVITE", Timestamp = DateTime.Parse("10:00:02") };
		public static LadderEventViewModel Event3 = new DialogEventViewModel() { SourceDevice = DeviceB, DestinationDevice = DeviceC, Display = "INVITE", Timestamp = DateTime.Parse("10:01:00") };
		public static LadderEventViewModel Event4 = new TransactionEventViewModel() { SourceDevice = DeviceA, DestinationDevice = DeviceB, Display = "ACK", Timestamp = DateTime.Parse("10:00:02") };
		public static LadderEventViewModel Event5 = new DialogEventViewModel() { SourceDevice = DeviceB, DestinationDevice = DeviceD, Display = "REFER", Timestamp = DateTime.Parse("10:01:17") };
		public static LadderEventViewModel Event6 = new DialogEventViewModel() { SourceDevice = DeviceD, DestinationDevice = DeviceC, Display = "BYE", Timestamp = DateTime.Parse("10:01:32") };
		public static LadderEventViewModel Event7 = new TransactionEventViewModel() { SourceDevice = DeviceB, DestinationDevice = DeviceA, Display = "BYE", Timestamp = DateTime.Parse("10:01:50") };

		public static DeviceViewModel[] Devices = new DeviceViewModel[] { DeviceA,DeviceB,DeviceC,DeviceD };
		public static LadderEventViewModel[] Rows = new LadderEventViewModel[] { Event1, Event2, Event3, Event4, Event5, Event6, Event7 };


	}
}
