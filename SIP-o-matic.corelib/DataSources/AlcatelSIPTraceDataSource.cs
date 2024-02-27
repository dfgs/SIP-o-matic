using SIP_o_matic.corelib.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SIP_o_matic.corelib.DataSources
{
	public class AlcatelSIPTraceDataSource : IDataSource
	{
		private static Regex inRegex = new Regex(@"(?<Timestamp>.*)RECEIVE MESSAGE FROM NETWORK \((?<Address>\d+\.\d+\.\d+\.\d+)");
		private static Regex outRegex = new Regex(@"(?<Timestamp>.*)SEND MESSAGE TO NETWORK \((?<Address>\d+\.\d+\.\d+\.\d+)");

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

		

		public async Task LoadAsync(string FileName)
		{
			Device device;
			Device? device2;
			string? line;
			Message _event;
			DateTime timeStamp;
			string sourceAddress, destinationAddress;
			string message;
			Match inMatch, outMatch;
			uint index;
			StreamReader reader;

			index = 1;
			devices.Clear();
			messages.Clear();

			device = new Device() { Name = "OXE" };
			device.Addresses.Add("127.0.0.1");

			using (FileStream stream = new FileStream(FileName, FileMode.Open))
			{
				reader = new StreamReader(stream);
				while (!reader.EndOfStream)
				{
					line = await reader.ReadLineAsync();
					if (line == null) break;

					inMatch = inRegex.Match(line);
					if (inMatch.Success)
					{
						message = await ReadMessageAsync(reader);
						DateTime.TryParse(inMatch.Groups["Timestamp"].Value, out timeStamp);
						sourceAddress = inMatch.Groups["Address"].Value;
						destinationAddress = "127.0.0.1";

						device2 = devices.FirstOrDefault(item => item.Name == sourceAddress);
						if (device2==null)
						{
							device2 = new Device(sourceAddress, new string[] { sourceAddress });
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
							DateTime.TryParse(inMatch.Groups["Timestamp"].Value, out timeStamp);
							sourceAddress = "127.0.0.1";
							destinationAddress = outMatch.Groups["Address"].Value;

							device2 = devices.FirstOrDefault(item => item.Name == destinationAddress);
							if (device2 == null)
							{
								device2 = new Device(destinationAddress, new string[] { destinationAddress });
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


		

		


	}

}
