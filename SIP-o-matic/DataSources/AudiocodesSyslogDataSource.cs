using AudiocodesSyslogLib;
using PcapngFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Shapes;

namespace SIP_o_matic.DataSources
{
	public class AudiocodesSyslogDataSource : IDataSource
	{
		//private static Regex eolRegex = new Regex(@"\[Time:(?<date>\d\d-\d\d)@(?<time>\d\d:\d\d:\d\d\.\d\d\d)\]$");
		private static Regex inRegex = new Regex(@"Incoming SIP Message from (?<address>\d+\.\d+\.\d+\.\d+).*---- *(\r\n)?(?<content>.*)", RegexOptions.Singleline);
		private static Regex outRegex = new Regex(@"Outgoing SIP Message to (?<address>\d+\.\d+\.\d+\.\d+).*---- *(\r\n)?(?<content>.*)", RegexOptions.Singleline);
		private static Regex sdpRegex = new Regex(@"^.=");
		private static Regex timeRegex = new Regex(@" *\[Time:[^]]+\](\r\n)?");
		//private static Regex messageRegex = new Regex(@"[^ ]+\s+[^ ]+\s+[^ ]+\s+(\[[^]]+\]\s+)+(?<message>.*)\[Time:(?<date>\d\d-\d\d)@(?<time>\d\d:\d\d:\d\d\.\d\d\d)\]", RegexOptions.Singleline);
		public string Description => "Audiocodes syslog";
		
		public AudiocodesSyslogDataSource()
		{
		}

		public IEnumerable<string> GetSupportedFileExts()
		{
			yield return "log";
			yield return "txt";
		}

		/*private string GetIPAddress(string Data)
		{
			Match match;

			match=ipRegex.Match(Data);
			if (!match.Success) return Data;
			return match.Groups["Value"].Value;
		}*/

		public async IAsyncEnumerable<Device> EnumerateDevicesAsync(string FileName)
		{
			Device device;

			device = new Device("Audiocodes","127.0.0.1");
			await Task.Yield();
			yield return device ;

		}


		// must add line feed before SDP
		private string FixSDP(string Message)
		{
			bool firstSDP;
			string buffer="";

			firstSDP = false;
			foreach(string line in Message.Split("\r\n").Select(item=>item.Trim()))
			{
				if (!firstSDP)
				{
					if (line == "")
					{
						firstSDP = true;
					}
					else
					{
						firstSDP = sdpRegex.Match(line).Success;
						if (firstSDP) buffer += "\r\n";
					}
				}
				buffer += line + "\r\n";

			}
			return buffer;
		}
		

		public async IAsyncEnumerable<Event> EnumerateEventsAsync(string FileName)
		{
			Event _event;
			string sourceAddress, destinationAddress,content;
			Match inMatch, outMatch;
			NotificationReader notificationReader;
			string message;

			using (Stream stream=new FileStream(FileName,FileMode.Open))
			{
				notificationReader = new NotificationReader(new SyslogReader());
				await foreach(Notification notification in notificationReader.ReadNotificationsAsync(stream))
				{
					inMatch = inRegex.Match(notification.Content);
					if (inMatch.Success)
					{
						sourceAddress = inMatch.Groups["address"].Value;
						destinationAddress = "127.0.0.1";
						content = inMatch.Groups["content"].Value;
						message = timeRegex.Replace(content, "");
						if (message != null)
						{
							_event = new Event(notification.Timestamp, sourceAddress, destinationAddress, FixSDP(message));
							yield return _event;
						}

					}
					else
					{
						outMatch = outRegex.Match(notification.Content);
						if (outMatch.Success)
						{
							sourceAddress = "127.0.0.1";
							destinationAddress = outMatch.Groups["address"].Value;
							content = outMatch.Groups["content"].Value;
							message = timeRegex.Replace(content, "");
							if (message != null)
							{
								_event = new Event(notification.Timestamp, sourceAddress, destinationAddress, FixSDP(message));
								yield return _event;
							}

						}

					}
				}
				
			}


		}

		
	}
}
