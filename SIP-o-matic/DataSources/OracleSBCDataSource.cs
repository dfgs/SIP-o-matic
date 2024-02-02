using HtmlAgilityPack;
using SIP_o_matic.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SIP_o_matic.DataSources
{
    public class OracleSBCDataSource : IDataSource
	{
		//private static Regex dataRegex = new Regex(@"<pre onmouseover=""mouseOverTooltip\(this\)"">(?<Timestamp>\d\d\d\d-\d\d-\d\d \d\d:\d\d:\d\d.\d\d\d)[ \n]*(?<Message>[^<]+)</pre>", RegexOptions.Multiline);
		private static Regex ipRegex = new Regex(@"(?<Value>\d+\.\d+\.\d+\.\d+)");
		private static Regex messageRegex = new Regex(@"(?<Timestamp>\d\d\d\d-\d\d-\d\d \d\d:\d\d:\d\d.\d\d\d)[ \n]*(?<Message>[^<]+)");

		public string Description => "Oracle SBC";
		
		public OracleSBCDataSource()
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
			HtmlDocument document;
			HtmlNode? div, table, header;
			int columnsCount;
			Device device;

			await Task.Yield();

			document = new HtmlDocument();
			document.Load(FileName);
			div = document.DocumentNode.Descendants(0).Where(n => n.HasClass("sipMsgFlowSection")).FirstOrDefault();
			if (div == null) yield break;
			table = div.Element("table");
			if (table == null) yield break;

			header = table.Elements("tr").ElementAt(1);
			if (header == null) yield break;

			columnsCount = header.Elements("td").Count();
			foreach(HtmlNode node in header.Elements("td"))
			{
				if (string.IsNullOrEmpty(node.InnerText)) continue;
				device=new Device() { Name = node.InnerText };
				device.Addresses.Add(node.InnerText);
				yield return device;
			}
		}


		public async IAsyncEnumerable<Message> EnumerateMessagesAsync(string FileName)
		{
			Message _event;
			DateTime timeStamp;
			string sourceAddress, destinationAddress;
			string message;
			string[] addresses;
			Match match;

			HtmlDocument document;
			HtmlNode? div,table,header,messageDiv,messageRow;
			HtmlNode[] tds;

			document = new HtmlDocument();
			document.Load(FileName);
			div = document.DocumentNode.Descendants(0).Where(n => n.HasClass("sipMsgFlowSection")).FirstOrDefault();
			if (div == null) yield break;
			table = div.Element("table");
			if (table == null) yield break;

			header = table.Elements("tr").ElementAt(1);
			if (header == null) yield break;

			addresses = header.Elements("td").Select(item => item.InnerText).Where(item=>!string.IsNullOrEmpty(item)).ToArray();

			foreach (HtmlNode row in table.Elements("tr").Where(n => n.HasClass("sipRow")))
			{
				messageRow = row.Elements("td").Where(item => item.HasClass("highlightRow")).FirstOrDefault();
				if (messageRow == null) continue;
				messageDiv = messageRow.Element("div");
				if (messageDiv == null) continue;
				match = messageRegex.Match(messageDiv.InnerText);
				if (!match.Success) continue;

				tds = row.Elements("td").ToArray();
				if (tds[4].HasClass("lside-arrowright"))
				{
					sourceAddress = addresses[0];
					destinationAddress = addresses[1];
				} 
				else if (tds[3].HasClass("rside-arrowleft"))
				{
					sourceAddress = addresses[1];
					destinationAddress = addresses[0];
				}
				else if (tds[8].HasClass("lside-arrowright"))
				{
					sourceAddress = addresses[2];
					destinationAddress = addresses[3];
				}
				else if (tds[7].HasClass("rside-arrowleft"))
				{
					sourceAddress = addresses[3];
					destinationAddress = addresses[2];
				}
				else
				{
					sourceAddress = "Undefined source";
					destinationAddress = "Undefined destination";
				}



				timeStamp = DateTime.ParseExact(match.Groups["Timestamp"].Value, "yyyy-MM-dd HH:mm:ss.fff",null);
				message = HttpUtility.HtmlDecode(match.Groups["Message"].Value).ReplaceLineEndings("\r\n");
				_event = new Message(timeStamp, sourceAddress, destinationAddress, message);
				yield return _event;
			}

			yield break;
		}

		
	}
}
