using SIP_o_matic.corelib.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SIP_o_matic.corelib.DataSources
{
	public class AlcatelSIPTraceDataSource : IDataSource
	{
		private static Regex inRegex = new Regex(@"(\d+ -\> )?(?<Timestamp>.*) RECEIVE MESSAGE FROM NETWORK \((?<Address>\d+\.\d+\.\d+\.\d+)");
		private static Regex outRegex = new Regex(@"(\d+ -\> )?(?<Timestamp>.*) SEND MESSAGE TO NETWORK \((?<Address>\d+\.\d+\.\d+\.\d+)");

		private List<Device> devices;
		private List<Message> messages;

		public string Description => "Alcatel SIP Trace";


		public AlcatelSIPTraceDataSource()
		{
			devices = new List<Device>();
			messages = new List<Message>();
		}

		public IEnumerable<string> GetSupportedFileExts()
		{
			yield return "log";
			yield return "txt";
		}


		private async Task<string> ReadMessageAsync(AlcatelStreamReader Reader)
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

		private DateTime GetTimeStamp(string Value)
		{
			DateTime timeStamp;

			if (DateTime.TryParseExact(Value, "ddd MMM d HH:mm:ss yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out timeStamp)) return timeStamp;
			if (DateTime.TryParseExact(Value, "dd/MM/yy HH:mm:ss.f", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out timeStamp)) return timeStamp;

			throw new InvalidOperationException($"Unsupported date format: {Value}");
		}

		public async Task LoadAsync(string FileName)
		{
			Device device;
			Device? device2;
			string? line;
			Message _event;
			DateTime timeStamp;
			Address sourceAddress, destinationAddress;
			string message;
			Match inMatch, outMatch;
			uint index;
			AlcatelStreamReader reader;
			string dateString;

			index = 1;
			devices.Clear();
			messages.Clear();

			device = new Device() { Name = "OXE" };
			device.Addresses.Add(new Address( "127.0.0.1"));
			devices.Add(device);


			using (FileStream stream = new FileStream(FileName, FileMode.Open))
			{
				reader = new AlcatelStreamReader(stream);
				while (!reader.EndOfStream)
				{
					line = await reader.ReadLineAsync();
					if (line == null) break;

					inMatch = inRegex.Match(line);
					if (inMatch.Success)
					{
						message = await ReadMessageAsync(reader);
						dateString = inMatch.Groups["Timestamp"].Value.Replace("  "," ");
						timeStamp = GetTimeStamp(dateString);
						
						sourceAddress = new Address(inMatch.Groups["Address"].Value);
						destinationAddress = new Address("127.0.0.1");

						device2 = devices.FirstOrDefault(item => item.Name == sourceAddress.Value);
						if (device2==null)
						{
							device2 = new Device(sourceAddress.Value, new Address[] { sourceAddress });
							devices.Add(device2);
						}

						_event = new Message(index++, timeStamp, sourceAddress, destinationAddress, message);
						messages.Add(_event);
					}
					else
					{
						outMatch = outRegex.Match(line);
						if (outMatch.Success)
						{
							message = await ReadMessageAsync(reader);
							dateString = outMatch.Groups["Timestamp"].Value.Replace("  ", " ");
							timeStamp = GetTimeStamp(dateString);

							sourceAddress = new Address("127.0.0.1");
							destinationAddress = new Address(outMatch.Groups["Address"].Value);

							device2 = devices.FirstOrDefault(item => item.Name == destinationAddress.Value);
							if (device2 == null)
							{
								device2 = new Device(destinationAddress.Value, new Address[] { destinationAddress });
								devices.Add(device2);
							}

							_event = new Message(index++, timeStamp, sourceAddress, destinationAddress, message);
							messages.Add(_event);
						}
					}



				}
			}
		}

		public IEnumerable<Device> EnumerateDevices()
		{
			return devices;
		}



		public IEnumerable<Message> EnumerateMessages()
		{
			return messages;
		}


		public IEnumerable<UDPStream> EnumerateUDPStreams()
		{
			yield break;
		}




	}

}
