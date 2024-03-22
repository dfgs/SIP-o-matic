using SIP_o_matic.corelib.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using System.Buffers.Text;
using System.Reflection.Metadata;
using System.Text.Unicode;
using System.Reflection;

namespace SIP_o_matic.corelib.DataSources
{
	public class GenericSIPDataSource : IDataSource
	{
		private List<Device> devices;
		private List<Message> messages;

		public string Description => "Generic SIP";


		public GenericSIPDataSource()
		{
			devices = new List<Device>();
			messages = new List<Message>();
		}

		public IEnumerable<string> GetSupportedFileExts()
		{
			yield return "sip";
		}



		public async Task LoadAsync(string FileName)
		{
			ParserLib.StreamReader streamReader;
			uint index;

			index = 0;
			devices.Clear();
			messages.Clear();

			using (FileStream stream = new FileStream(FileName, FileMode.Open))
			{
				streamReader = new ParserLib.StreamReader(stream, ' ', '\r', '\n');
				await foreach (Device device in SIPFileGrammar.DeviceEnumerator.Parse(streamReader).ToAsyncEnumerable())
				{
					devices.Add(device);
				}
				await foreach (Message message in SIPFileGrammar.MessageEnumerator.Parse(streamReader).ToAsyncEnumerable())
				{
					message.Index = index;
					index++;
					messages.Add(message);
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
