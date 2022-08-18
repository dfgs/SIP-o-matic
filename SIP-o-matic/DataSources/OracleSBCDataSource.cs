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
		private static Regex dataRegex = new Regex(@"<pre onmouseover=""mouseOverTooltip\(this\)"">(?<Timestamp>\d\d\d\d-\d\d-\d\d \d\d:\d\d:\d\d.\d\d\d)[ \n]*(?<Message>[^<]+)</pre>", RegexOptions.Multiline);
		private static Regex ipRegex = new Regex(@"(?<Value>\d+\.\d+\.\d+\.\d+)");

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
			await Task.Yield();
			yield break;

		}


		public async IAsyncEnumerable<Event> EnumerateEventsAsync(string FileName)
		{
			string line;
			MatchCollection matches;
			Event _event;
			DateTime timeStamp;
			string sourceAddress, destinationAddress;
			string message;

			using (StreamReader reader=new StreamReader(FileName))
			{
				line=await reader.ReadToEndAsync();
				matches = dataRegex.Matches(line);
						
				

				await foreach (Match match in matches.ToAsyncEnumerable())
				{

					timeStamp = DateTime.ParseExact(match.Groups["Timestamp"].Value, "yyyy-MM-dd HH:mm:ss.fff", null);

					sourceAddress ="tbd";
					destinationAddress = "tbd2";
					message = match.Groups["Message"].Value;
					_event = new Event(timeStamp, sourceAddress, destinationAddress, HttpUtility.HtmlDecode(message).ReplaceLineEndings("\r\n"));
					yield return _event;	
				}
			}

		}

		
	}
}
