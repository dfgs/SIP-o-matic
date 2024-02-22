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

		public string Description => "Generic SIP";


		public GenericSIPDataSource()
		{
		}

		public IEnumerable<string> GetSupportedFileExts()
		{
			yield return "sip";
		}

		

		public async IAsyncEnumerable<Device> EnumerateDevicesAsync(string FileName)
		{
			ParserLib.StreamReader streamReader;

			using (FileStream stream = new FileStream(FileName, FileMode.Open))
			{
				streamReader = new ParserLib.StreamReader(stream, ' ', '\r', '\n');
				await foreach(Device device in SIPFileGrammar.DeviceEnumerator.Parse(streamReader).ToAsyncEnumerable())
				{
					yield return device;
				}
			}

		}//*/


		public async IAsyncEnumerable<Message> EnumerateMessagesAsync(string FileName)
		{
			uint index;
			ParserLib.StreamReader streamReader;

			index = 0;
			using (FileStream stream = new FileStream(FileName, FileMode.Open))
			{
				streamReader = new ParserLib.StreamReader(stream, ' ', '\r', '\n');
				await foreach (Message message in SIPFileGrammar.MessageEnumerator.Parse(streamReader).ToAsyncEnumerable())
				{
					message.Index = index;
					index++;
					yield return message;
				}
			}
		}

		
	}
}
