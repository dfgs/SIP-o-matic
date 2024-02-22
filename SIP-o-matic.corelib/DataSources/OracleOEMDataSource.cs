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
			string line, devicesString, dataString;
			Match match;
			JsonNode? devicesNode,dataNode;
			JsonArray? devicesArray;
			JsonArray? dataArray;
			Device _device;
			string name, addresses;
			string sourceAddress, destinationAddress;
			List<Device> devices;
			int srcDeviceIndex,dstDeviceIndex;
			Dictionary<int, Device> deviceDictionary;

			devices = new List<Device>();

			// internal oracle device IDs
			deviceDictionary=new Dictionary<int, Device>();

			using (StreamReader reader = new StreamReader(FileName))
			{


				line = await reader.ReadToEndAsync();
				match = devicesRegex.Match(line);
				if (!match.Success) yield break;


				#region read device array
				devicesString = match.Groups["Value"].Value;

				devicesNode = JsonNode.Parse(devicesString);
				if (devicesNode == null) yield break;

				devicesArray = devicesNode.AsArray();
				if (devicesArray == null) yield break;


				await foreach (JsonNode? node in devicesArray.ToAsyncEnumerable())
				{
					name= node!["name"]!.GetValue<string>();
					addresses = node!["ipranges"]!.GetValue<string>();
					srcDeviceIndex = node!["id"]!.GetValue<int>();


					_device = new Device() { Name = name };
					foreach(string address in addresses.Split('\n'))
					{
						_device.Addresses.Add(GetIPAddress(address));
					}
					devices.Add(_device);

					deviceDictionary.Add(srcDeviceIndex, _device);
				}
				#endregion

				#region read devices from messages

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
					sourceAddress = GetIPAddress(node!["src_ip"]!.GetValue<string>());
					destinationAddress = GetIPAddress(node!["dst_ip"]!.GetValue<string>());

					srcDeviceIndex = node!["src_device"]!.GetValue<int>();
					dstDeviceIndex = node!["dst_device"]!.GetValue<int>();

					_device = deviceDictionary[srcDeviceIndex];
					if (!_device.Addresses.Contains(sourceAddress)) _device.Addresses.Add(sourceAddress);

					_device = deviceDictionary[dstDeviceIndex];
					if (!_device.Addresses.Contains(destinationAddress)) _device.Addresses.Add(destinationAddress);
					///yield return message;
				}
				#endregion




			}

			await foreach(Device device in devices.ToAsyncEnumerable())
			{
				yield return device;
			}

		}//*/


		private string DecodeContent(string Base64Message)
		{
			Span<byte> buffer;
			int length;

			buffer = new Span<byte>(new byte[Base64Message.Length]);
			if (!Convert.TryFromBase64String(Base64Message, buffer, out length)) return Base64Message;

			return Encoding.UTF8.GetString(Convert.FromBase64String(Base64Message));

		}
		public async IAsyncEnumerable<Message> EnumerateMessagesAsync(string FileName)
		{
			string line,dataString;
			Match match;
			JsonNode? dataNode;
			JsonArray? dataArray;
			string base64Message;
			Message message;
			DateTime timeStamp;
			string sourceAddress, destinationAddress;
			string content;
			uint index = 1;
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

					content = DecodeContent(base64Message);
					
					message = new Message(index++,timeStamp, sourceAddress, destinationAddress, content);
					yield return message;	
				}
			}

		}

		
	}
}
