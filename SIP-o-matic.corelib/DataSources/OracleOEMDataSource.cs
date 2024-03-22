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

		private List<Device> devices;
		private List<Message> messages;

		public OracleOEMDataSource()
		{
			devices = new List<Device>();
			messages = new List<Message>();
		}

		public IEnumerable<string> GetSupportedFileExts()
		{
			yield return "htm";
			yield return "html";
		}

		private Address GetIPAddress(string Data)
		{
			Match match;

			match=ipRegex.Match(Data);
			if (!match.Success) return new Address(Data);
			return new Address(match.Groups["Value"].Value);
		}
		private string DecodeContent(string Base64Message)
		{
			Span<byte> buffer;
			int length;

			buffer = new Span<byte>(new byte[Base64Message.Length]);
			if (!Convert.TryFromBase64String(Base64Message, buffer, out length)) return Base64Message;

			return Encoding.UTF8.GetString(Convert.FromBase64String(Base64Message));

		}

		public async Task LoadAsync(string FileName)
		{
			string line, devicesString, dataString;
			Match match;
			JsonNode? devicesNode, dataNode;
			JsonArray? devicesArray;
			JsonArray? dataArray;
			Device _device;
			string name, addresses;
			Address sourceAddress, destinationAddress;
			int? srcDeviceIndex, dstDeviceIndex;
			string base64Message;
			Message message;
			string content;
			uint index = 1;
			DateTime timeStamp;
			Dictionary<int, Device> deviceDictionary;

			devices.Clear();
			messages.Clear();

			// internal oracle device IDs
			deviceDictionary = new Dictionary<int, Device>();

			using (StreamReader reader = new StreamReader(FileName))
			{
				line = await reader.ReadToEndAsync();
				match = devicesRegex.Match(line);
				if (!match.Success) return;

				#region read device array
				devicesString = match.Groups["Value"].Value;

				devicesNode = JsonNode.Parse(devicesString);
				if (devicesNode == null) return;

				devicesArray = devicesNode.AsArray();
				if (devicesArray == null) return;


				await foreach (JsonNode? node in devicesArray.ToAsyncEnumerable())
				{
					name = node!["name"]!.GetValue<string>();
					addresses = node!["ipranges"]!.GetValue<string>();
					srcDeviceIndex = node!["id"]!.GetValue<int>();

					_device = new Device() { Name = name };
					foreach (string address in addresses.Split('\n'))
					{
						_device.Addresses.Add(GetIPAddress(address));
					}
					devices.Add(_device);

					deviceDictionary.Add(srcDeviceIndex.Value, _device);
				}
				#endregion

				#region read messages

				match = dataRegex.Match(line);
				if (!match.Success) return;
				dataString = match.Groups["Value"].Value;

				dataNode = JsonNode.Parse(dataString);
				if (dataNode == null) return;

				dataArray = dataNode["messages"]?.AsArray();
				if (dataArray == null) return;


				await foreach (JsonNode? node in dataArray.ToAsyncEnumerable())
				{
					if (node!["type"]!.GetValue<string>() != "SIP") continue;
					
					timeStamp = node!["ts"]!.GetValue<DateTime>();

					sourceAddress = GetIPAddress(node!["src_ip"]!.GetValue<string>());
					destinationAddress = GetIPAddress(node!["dst_ip"]!.GetValue<string>());

					srcDeviceIndex = node!["src_device"]?.GetValue<int>();
					dstDeviceIndex = node!["dst_device"]?.GetValue<int>();

					if (srcDeviceIndex.HasValue)
					{
						_device = deviceDictionary[srcDeviceIndex.Value];
						if (!_device.Addresses.Contains(sourceAddress)) _device.Addresses.Add(sourceAddress);
					}

					if (dstDeviceIndex.HasValue)
					{
						_device = deviceDictionary[dstDeviceIndex.Value];
						if (!_device.Addresses.Contains(destinationAddress)) _device.Addresses.Add(destinationAddress);
					}

					base64Message = node!["data"]!.GetValue<string>();

					content = DecodeContent(base64Message);

					message = new Message(index++, timeStamp, sourceAddress, destinationAddress, content);
					messages.Add(message);
					///yield return message;
				}
				#endregion

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
