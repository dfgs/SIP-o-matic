﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SIP_o_matic.DataSources
{
	public class OracleDataSource : IDataSource
	{
		private string fileName;
		private static Regex regex = new Regex(@"var data = \((?<Value>.+)\);$", RegexOptions.Multiline);
		private static Regex ipRegex = new Regex(@"(?<Value>\d+\.\d+\.\d+\.\d+)");
		public OracleDataSource(string FileName)
		{
			this.fileName = FileName;
		}

		private string GetIPAddress(string Data)
		{
			Match match;

			match=ipRegex.Match(Data);
			if (!match.Success) return Data;
			return match.Groups["Value"].Value;
		}

		public async IAsyncEnumerable<Event> EnumerateEventsAsync()
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

			using (StreamReader reader=new StreamReader(fileName))
			{
				line=await reader.ReadToEndAsync();
				match = regex.Match(line);
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
