using PcapngFile;
using System;
using System.Collections.Generic;
using System.Globalization;
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
	public class AlcatelSIPTraceDataSource : IDataSource
	{
		private static Regex inRegex = new Regex(@"(?<Timestamp>... ... \d\d \d\d:\d\d:\d\d \d\d\d\d).*RECEIVE MESSAGE FROM NETWORK \((?<Address>\d+\.\d+\.\d+\.\d+)");
		private static Regex outRegex = new Regex(@"(?<Timestamp>... ... \d\d \d\d:\d\d:\d\d \d\d\d\d).*SEND MESSAGE TO NETWORK \((?<Address>\d+\.\d+\.\d+\.\d+)");
		public string Description => "Alcatel SIP Trace";
		
		public AlcatelSIPTraceDataSource()
		{
		}

		public IEnumerable<string> GetSupportedFileExts()
		{
			yield return "log";
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

			device = new Device("OXE","127.0.0.1");
			await Task.Yield();
			yield return device ;

		}

		private async Task<string> ReadMessageAsync(StreamReader Reader)
		{
			string? line;
			string buffer;

			buffer = "";
			// skip first line: ----------------------utf8-----------------------
			line = await Reader.ReadLineAsync();
			do
			{
				line = await Reader.ReadLineAsync();
				if (line == null) return buffer;
				if (line == "-------------------------------------------------") return buffer;
				buffer += line + "\r\n";
			} while (true);

		}
		
		public async IAsyncEnumerable<Event> EnumerateEventsAsync(string FileName)
		{
			string? line;
			Event _event;
			DateTime timeStamp;
			string sourceAddress, destinationAddress;
			string message;
			Match inMatch, outMatch;

			using (StreamReader reader=new StreamReader(FileName))
			{
				while(!reader.EndOfStream)
				{
					line = await reader.ReadLineAsync();
					if (line == null) break;


					inMatch = inRegex.Match(line);
					if (inMatch.Success)
					{
						message = await ReadMessageAsync(reader);
						timeStamp = DateTime.ParseExact(inMatch.Groups["Timestamp"].Value, "ddd MMM dd HH:mm:ss yyyy", CultureInfo.InvariantCulture);
						sourceAddress = inMatch.Groups["Address"].Value;
						destinationAddress = "127.0.0.1";
						_event = new Event(timeStamp, sourceAddress, destinationAddress, message);
						yield return _event;						
					}
					else
					{
						outMatch = outRegex.Match(line);
						if (outMatch.Success)
						{
							message = await ReadMessageAsync(reader);
							timeStamp = DateTime.ParseExact(outMatch.Groups["Timestamp"].Value, "ddd MMM dd HH:mm:ss yyyy",CultureInfo.InvariantCulture);
							sourceAddress = "127.0.0.1";
							destinationAddress = outMatch.Groups["Address"].Value;
							_event = new Event(timeStamp, sourceAddress, destinationAddress, message);
							yield return _event;

						}
					}
					


				}
			}

			yield break;

		}

		
	}
}
