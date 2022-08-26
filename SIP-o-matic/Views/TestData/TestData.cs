using SIP_o_matic.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.Views
{
	internal static class TestData
	{
		public static DeviceViewModel DeviceA = new DeviceViewModel("Device A");
		public static DeviceViewModel DeviceB = new DeviceViewModel("Device B");
		public static DeviceViewModel DeviceC = new DeviceViewModel("Device C");
		public static DeviceViewModel DeviceD = new DeviceViewModel("Device D");

		public static DialogEventViewModel Event1 = new DialogEventViewModel() { SourceDevice = DeviceA, DestinationDevice = DeviceB, Display = "INVITE", Timestamp = DateTime.Parse("10:00:00"), EventColor = "GoldenRod", Status = Statuses.Success };
		public static LadderEventViewModel Event2 = new TransactionEventViewModel() { SourceDevice = DeviceA, DestinationDevice = DeviceB, Display = "INVITE", Timestamp = DateTime.Parse("10:00:02"), EventColor = "GoldenRod", Status = Statuses.Incomplete };
		public static DialogEventViewModel Event3 = new DialogEventViewModel() { SourceDevice = DeviceB, DestinationDevice = DeviceC, Display = "INVITE", Timestamp = DateTime.Parse("10:01:00"), EventColor = "Lime", Status = Statuses.Redirected };
		public static LadderEventViewModel Event4 = new TransactionEventViewModel() { SourceDevice = DeviceA, DestinationDevice = DeviceB, Display = "ACK", Timestamp = DateTime.Parse("10:00:02"), EventColor = "GoldenRod", Status = Statuses.Failed };
		public static DialogEventViewModel Event5 = new DialogEventViewModel() { SourceDevice = DeviceB, DestinationDevice = DeviceD, Display = "REFER", Timestamp = DateTime.Parse("10:01:17"), EventColor = "Crimson", Status = Statuses.Undefined };
		public static DialogEventViewModel Event6 = new DialogEventViewModel() { SourceDevice = DeviceD, DestinationDevice = DeviceC, Display = "BYE", Timestamp = DateTime.Parse("10:01:32"), EventColor = "Lime", Status = Statuses.Success };
		public static LadderEventViewModel Event7 = new TransactionEventViewModel() { SourceDevice = DeviceB, DestinationDevice = DeviceA, Display = "BYE", Timestamp = DateTime.Parse("10:01:50"), EventColor = "GoldenRod", Status = Statuses.Success };

		public static DeviceViewModel[] Devices = new DeviceViewModel[] { DeviceA, DeviceB, DeviceC, DeviceD };
		public static LadderEventViewModel[] Rows = new LadderEventViewModel[] { Event1, Event2, Event3, Event4, Event5, Event6, Event7 };

		public static TimestampViewModel Timestamp1 = new TimestampViewModel() { Value = DateTime.Parse("10:00:00") };
		public static TimestampViewModel Timestamp2 = new TimestampViewModel() { Value = DateTime.Parse("10:02:00") };
		public static TimestampViewModel Timestamp3 = new TimestampViewModel() { Value = DateTime.Parse("10:05:31") };
		public static TimestampViewModel Timestamp4 = new TimestampViewModel() { Value = DateTime.Parse("10:15:31") };
		public static TimestampViewModel Timestamp5 = new TimestampViewModel() { Value = DateTime.Parse("10:20:00") };
		public static TimestampViewModel Timestamp6 = new TimestampViewModel() { Value = DateTime.Parse("10:25:01") };

		public static SessionEventViewModel Session1 = new SessionEventViewModel() { StartTime = Timestamp1 , StopTime=Timestamp3, SourceAddress = "192.168.1.1", SourcePort = 21000, DestinationAddress = "192.168.1.2", DestinationPort = 31000 ,DialogEvent=Event1};
		public static SessionEventViewModel Session2 = new SessionEventViewModel() { StartTime = Timestamp2, StopTime = Timestamp4, SourceAddress = "192.168.1.4", SourcePort = 21002, DestinationAddress = "50.56.1.12", DestinationPort = 42123, DialogEvent = Event3 };
		public static SessionEventViewModel Session3 = new SessionEventViewModel() { StartTime = Timestamp5, StopTime = Timestamp6, SourceAddress = "10.10.1.1", SourcePort = 31400, DestinationAddress = "192.168.1.2", DestinationPort = 28788, DialogEvent = Event5 };
		public static SessionEventViewModel[] Sessions = new SessionEventViewModel[] { Session1, Session2, Session3 };
		public static TimestampViewModel[] SessionTimestamps = new TimestampViewModel[] { Timestamp1, Timestamp2, Timestamp3, Timestamp4, Timestamp5, Timestamp6};

	}
}
