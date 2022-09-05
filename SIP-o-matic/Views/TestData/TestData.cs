using LogLib;
using SIP_o_matic.ViewModels;
using SIPParserLib;
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

		public static SessionEventViewModel Session1 = new SessionEventViewModel() { StartTime = Timestamp1 , StopTime=Timestamp3, SourceAddress = "192.168.1.1", SourcePort = 21000, DestinationAddress = "192.168.1.2", DestinationPort = 31000 ,DialogEvent=Event1, Codec="PCMA"};
		public static SessionEventViewModel Session2 = new SessionEventViewModel() { StartTime = Timestamp2, StopTime = Timestamp4, SourceAddress = "192.168.1.4", SourcePort = 21002, DestinationAddress = "50.56.1.12", DestinationPort = 42123, DialogEvent = Event3, Codec = "PCMA" };
		public static SessionEventViewModel Session3 = new SessionEventViewModel() { StartTime = Timestamp5, StopTime = Timestamp6, SourceAddress = "10.10.1.1", SourcePort = 31400, DestinationAddress = "192.168.1.2", DestinationPort = 28788, DialogEvent = Event5, Codec = "PCMA" };
		public static SessionEventViewModel[] Sessions = new SessionEventViewModel[] { Session1, Session2, Session3 };
		public static TimestampViewModel[] SessionTimestamps = new TimestampViewModel[] { Timestamp1, Timestamp2, Timestamp3, Timestamp4, Timestamp5, Timestamp6};

		public static string InviteString1 = "INVITE sip:+33140143960@ecb.core.nord:5060;user=phone SIP/2.0\r\nVia: SIP/2.0/UDP 22.19.16.38:5060;branch=z9hG4bKgf8qu4309gm3f6ljhkc0.1\r\nAllow: INVITE, ACK, CANCEL, BYE, PRACK, NOTIFY, SUBSCRIBE, OPTIONS, UPDATE, INFO\r\nSupported: path,100rel\r\nUser-Agent: OmniPCX Enterprise R12.1 m2.300.23\r\nP-Asserted-Identity: \"ITGP TEL05 T NT1\" <sip:+33157437620@22.19.16.38;user=phone>\r\nContent-Type: application/sdp\r\nTo: <sip:+33140143960@22.19.1.52;user=phone>\r\nFrom: \"ITGP TEL05 T NT1\" <sip:+33157437620@alcatel;user=phone>;tag=SD1duf601-c2810c7ef9af3af338277ef2e8bad0e9\r\nContact: <sip:+33157437620@22.19.16.38:5060;transport=udp>\r\nCall-ID: SD1duf601-56e1a9f579471d1ae274d6db70de1aa7-mo420q0\r\nCSeq: 1568068945 INVITE\r\nMax-Forwards: 69\r\nContent-Length: 288\r\n    \r\nv = 0\r\no =OXE 1657028298 1657028298 IN IP4 22.19.16.38\r\ns = abs\r\nc = IN IP4 22.19.16.38\r\nt = 0 0\r\nm =audio 20664 RTP/AVP 8 18 101\r\na = sendrecv\r\na = rtpmap:8 PCMA/8000\r\na = ptime:20\r\na = maxptime:30\r\na =rtpmap:18 G729/8000\r\na =fmtp:18 annexb=no\r\na = ptime:20\r\na = maxptime:40\r\na =rtpmap:101 telephone-event/8000";
		public static Request Request1 =(Request)SIPGrammar.SIPMessage.Parse(InviteString1,' ') ;
		public static SDP SDP1=SDPGrammar.SDP.Parse(Request1.Body,' ') ;

		public static string ResponseString1 = "SIP / 2.0 200 OK\r\nVia: SIP/2.0/UDP 10.105.9.205:5060;branch=z9hG4bKac2032292049\r\nTo: <sip:+33986011458@176.161.242.37;user=phone>;tag=h7g4Esbg_916984333-1653917221092\r\nFrom: <sip:+33663422020@ent.bouyguestelecom.fr;user=phone>;tag=1c187532784\r\nCall-ID: 13874654713052022152659@10.105.9.205\r\nCSeq: 1 INVITE\r\nContact: <sip:sgc_c@176.161.242.37;transport=udp>\r\nRecord-Route: <sip:176.161.242.37;transport=udp;lr>\r\nRequire: timer\r\nSession-Expires: 1800;refresher=uac\r\nSupported: timer\r\nSupported:\r\nContent-Type: application/sdp\r\nContent-Length: 274\r\nAllow: ACK,BYE,CANCEL,INFO,INVITE,OPTIONS,PRACK,REFER,NOTIFY,UPDATE\r\nAccept: application/media_control+xml,application/sdp\r\n\r\nv=0\r\no=BroadWorks 1179974073 1 IN IP4 176.161.242.37\r\ns=-\r\nc=IN IP4 176.161.242.225\r\nt=0 0\r\nm=audio 22290 RTP/AVP 18 8 101\r\nb=AS:128\r\na=sendrecv\r\na=rtpmap:18 G729/8000\r\na=fmtp:18 annexb=no\r\na=ptime:20\r\na=maxptime:40\r\na=rtpmap:8 PCMA/8000\r\na=rtpmap:101 telephone-event/8000";
		public static Response Response1 = (Response)SIPGrammar.SIPMessage.Parse(ResponseString1, ' ');
		public static SDP SDP2 = SDPGrammar.SDP.Parse(Response1.Body, ' ');



		public static RequestViewModel Invite1 = new RequestViewModel(NullLogger.Instance, new DataSources.Event(), Request1, SDP1);
		public static ResponseViewModel OK1 = new ResponseViewModel(NullLogger.Instance, new DataSources.Event(), Response1, SDP2);


	}
}
