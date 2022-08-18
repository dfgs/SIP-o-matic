using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SIP_o_matic.DataSources
{
	public class OracleOEMDataSource : IDataSource
	{
		private static Regex dataRegex = new Regex(@"var data = \((?<Value>.+)\);$", RegexOptions.Multiline);
		private static Regex devicesRegex = new Regex(@"devices: \((?<Value>.+)\),$", RegexOptions.Multiline);
		private static Regex ipRegex = new Regex(@"(?<Value>\d+\.\d+\.\d+\.\d+)");

		public string Description => "Oracle OEM";


		public OracleOEMDataSource()
		{
		}

		public IEnumerable<string> GetSupportedFileExts()
		{
			yield return "htm";
			yield return "html";
		}

		private string GetIPAddress(string Data)
		{
			Match match;

			match=ipRegex.Match(Data);
			if (!match.Success) return Data;
			return match.Groups["Value"].Value;
		}

		public async IAsyncEnumerable<Device> EnumerateDevicesAsync(string FileName)
		{
			string line, devicesString;
			Match match;
			JsonNode? devicesNode;
			JsonArray? devicesArray;
			Device _device;
			string name, addresses;
			

			using (StreamReader reader = new StreamReader(FileName))
			{
				line = await reader.ReadToEndAsync();
				match = devicesRegex.Match(line);
				if (!match.Success) yield break;
				devicesString = match.Groups["Value"].Value;

				devicesNode = JsonNode.Parse(devicesString);
				if (devicesNode == null) yield break;

				devicesArray = devicesNode.AsArray();
				if (devicesArray == null) yield break;


				await foreach (JsonNode? node in devicesArray.ToAsyncEnumerable())
				{
					name= node!["name"]!.GetValue<string>();
					addresses = node!["ipranges"]!.GetValue<string>();
					foreach (string address in addresses.Split('\n'))
					{
						_device = new Device(name, GetIPAddress( address) );
						yield return _device;
					}
				}
			}

		}


		public async IAsyncEnumerable<Event> EnumerateEventsAsync(string FileName)
		{
			string line,dataString;
			Match match;
			JsonNode? dataNode;
			JsonArray? dataArray;
			string base64Message;
			Event _event;
			DateTime timeStamp;
			string sourceAddress, destinationAddress;
			string message;

			using (StreamReader reader=new StreamReader(FileName))
			{
				line=await reader.ReadToEndAsync();
				match = dataRegex.Match(line);
				if (!match.Success) yield break;
				dataString = match.Groups["Value"].Value;

				dataNode = JsonNode.Parse(dataString);
				if (dataNode == null) yield break;

				dataArray = dataNode["messages"]?.AsArray();
				if (dataArray == null) yield break;


				await foreach (JsonNode? node in dataArray.ToAsyncEnumerable())
				{
					if (node!["type"]!.GetValue<string>() != "SIP") continue;
					timeStamp = node!["ts"]!.GetValue<DateTime>();
					sourceAddress = GetIPAddress(node!["src_ip"]!.GetValue<string>());
					destinationAddress = GetIPAddress(node!["dst_ip"]!.GetValue<string>());
					base64Message= node!["data"]!.GetValue<string>();
					message = Encoding.UTF8.GetString(Convert.FromBase64String(base64Message));
					_event = new Event(timeStamp, sourceAddress, destinationAddress, message);
					yield return _event;	
				}
			}

		}

		
	}
}
