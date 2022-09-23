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
		private static Regex eolRegex = new Regex(@"\[Time:(?<date>\d\d-\d\d)@(?<time>\d\d:\d\d:\d\d\.\d\d\d)\]$");
		private static Regex inRegex = new Regex(@"Incoming SIP Message from (?<address>\d+\.\d+\.\d+\.\d+).*---- *(\r\n)?(?<content>.*)\[Time:(?<date>\d\d-\d\d)@(?<time>\d\d:\d\d:\d\d\.\d\d\d)\]", RegexOptions.Singleline);
		private static Regex outRegex = new Regex(@"Outgoing SIP Message to (?<address>\d+\.\d+\.\d+\.\d+).*---- *(\r\n)?(?<content>.*)\[Time:(?<date>\d\d-\d\d)@(?<time>\d\d:\d\d:\d\d\.\d\d\d)\]", RegexOptions.Singleline);
		private static Regex sdpRegex = new Regex(@"^.=");
		private static Regex messageRegex = new Regex(@"[^ ]+\s+[^ ]+\s+[^ ]+\s+(\[[^]]+\]\s+)+(?<message>.*)\[Time:(?<date>\d\d-\d\d)@(?<time>\d\d:\d\d:\d\d\.\d\d\d)\]", RegexOptions.Singleline);
		public string Description => "Audiocodes syslog";
		
		public AudiocodesSyslogDataSource()
		{
		}

		public IEnumerable<string> GetSupportedFileExts()
		{
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

		public async Task<string> ReadLine(StreamReader Reader)
		{
			string buffer = "";
			string? line;
			Match match;

			do
			{
				line = await Reader.ReadLineAsync();
				if (line == null) break;
				line = line.Replace(@"\n", "\r\n");
				//line = line.ReplaceLineEndings("\r\n");
				buffer += line+"\r\n";
				match = eolRegex.Match(line);
			} while (!match.Success);

			return buffer;
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
					firstSDP=sdpRegex.Match(line).Success;
					if (firstSDP) buffer += "\r\n"; 
				}
				buffer += line + "\r\n";

			}
			return buffer;
		}
		public async Task<string?> ReadMessage(StreamReader Reader)
		{
			string? buffer;
			Match messageMatch;
			string message;

			buffer = await ReadLine(Reader);

			if (string.IsNullOrEmpty(buffer)) return null;

			messageMatch = messageRegex.Match(buffer);
			if (!messageMatch.Success) 
				return null;
			message = messageMatch.Groups["message"].Value ;
			return FixSDP(message);
		}

		public async IAsyncEnumerable<Event> EnumerateEventsAsync(string FileName)
		{
			string line;
			Event _event;
			DateTime timeStamp;
			string sourceAddress, destinationAddress,content;
			string? message;
			Match inMatch, outMatch;

			using (StreamReader reader=new StreamReader(FileName))
			{
				while(!reader.EndOfStream)
				{
					line = await ReadLine(reader);

					inMatch = inRegex.Match(line);
					if (inMatch.Success)
					{
						timeStamp = DateTime.ParseExact($"{inMatch.Groups["date"].Value} {inMatch.Groups["time"].Value}", "dd-MM HH:mm:ss.fff", null);
						sourceAddress = inMatch.Groups["address"].Value;
						destinationAddress = "127.0.0.1";
						content = inMatch.Groups["content"].Value;
						if (!string.IsNullOrWhiteSpace(content))
						{
							message = FixSDP(content);
						}
						else
						{
							message = await ReadMessage(reader);
						}
						if (message != null)
						{
							_event = new Event(timeStamp, sourceAddress, destinationAddress, message);
							yield return _event;
						}
						
					}
					else
					{
						outMatch = outRegex.Match(line);
						if (outMatch.Success)
						{
							timeStamp = DateTime.ParseExact($"{outMatch.Groups["date"].Value} {outMatch.Groups["time"].Value}", "dd-MM HH:mm:ss.fff", null);
							sourceAddress = "127.0.0.1";
							destinationAddress = outMatch.Groups["address"].Value;
							content = outMatch.Groups["content"].Value;
							if (!string.IsNullOrWhiteSpace(content))
							{
								message = FixSDP(content);
							}
							else
							{
								message = await ReadMessage(reader);
							}
							if (message != null)
							{
								_event = new Event(timeStamp, sourceAddress, destinationAddress, message);
								yield return _event;
							}
							
						}
						
					}
					


				}
			}

			yield break;

		}

		
	}
}
