using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AudiocodesSyslogLib
{
	public  class SyslogReader: ISyslogReader
	{
		private static Regex logRegex = new Regex(@"(?<Timestamp>\d\d:\d\d:\d\d.\d\d\d) +(?<Address>\d+\.\d+\.\d+\.\d+) +(?<Severity>[^ ]+) +\[S=(?<SequenceID>\d+)\] +(?<Content>.*)");
		private static Regex sessionLogRegex = new Regex(@"(?<Timestamp>\d\d:\d\d:\d\d.\d\d\d) +(?<Address>\d+\.\d+\.\d+\.\d+) +(?<Severity>[^ ]+) +\[S=(?<SequenceID>\d+)\] +\[SID=(?<SessionID>[^]]+)\] +(\[[^]]+\] +)?(?<Content>.*)$", RegexOptions.Singleline);
		private static Regex boardLogRegex = new Regex(@"(?<Timestamp>\d\d:\d\d:\d\d.\d\d\d) +(?<Address>\d+\.\d+\.\d+\.\d+) +(?<Severity>[^ ]+) +\[S=(?<SequenceID>\d+)\] +\[BID=(?<BoardID>[^]]+)\] +(\[[^]]+\] +)?(?<Content>.*)$", RegexOptions.Singleline);


		public async IAsyncEnumerable<string> ReadBlocksAsync(Stream Stream)
		{
			Match logMatch;
			StreamReader reader;
			string? line;
			string? currentBlock = null;

			if (Stream == null) throw new ArgumentNullException(nameof(Stream));

			reader = new StreamReader(Stream);
			while (!reader.EndOfStream)
			{
				line = await reader.ReadLineAsync();
				if (line == null) break;
				line = line.TrimStart().TrimEnd();

				logMatch = logRegex.Match(line);
				if (logMatch.Success)
				{
					if (!string.IsNullOrEmpty(currentBlock)) yield return currentBlock;
					currentBlock = line;
				}
				else
				{
					if (!string.IsNullOrEmpty(currentBlock)) currentBlock += "\r\n"+line ;
				}
			}
			if (!string.IsNullOrEmpty(currentBlock)) yield return currentBlock;
		}


		public async IAsyncEnumerable<Syslog> ReadSyslogsAsync(Stream Stream)
		{
			Match match;

			if (Stream == null) throw new ArgumentNullException(nameof(Stream));

			await foreach (string block in ReadBlocksAsync(Stream))
			{
				match = sessionLogRegex.Match(block);
				if (match.Success)
				{
					yield return new SessionSyslog(DateTime.ParseExact(match.Groups["Timestamp"].Value, "HH:mm:ss.fff", null), match.Groups["Address"].Value, match.Groups["Severity"].Value, ulong.Parse(match.Groups["SequenceID"].Value), match.Groups["SessionID"].Value, match.Groups["Content"].Value);
					continue;
				}
				match = boardLogRegex.Match(block);
				if (match.Success)
				{
					yield return new BoardSyslog(DateTime.ParseExact(match.Groups["Timestamp"].Value, "HH:mm:ss.fff", null), match.Groups["Address"].Value, match.Groups["Severity"].Value, ulong.Parse(match.Groups["SequenceID"].Value), match.Groups["BoardID"].Value, match.Groups["Content"].Value);
					continue;
				}
				match = logRegex.Match(block);
				if (!match.Success) throw new FormatException("Invalid block format"); ; 

				yield return new UndefinedSyslog(DateTime.ParseExact(match.Groups["Timestamp"].Value, "HH:mm:ss.fff",null), match.Groups["Address"].Value, match.Groups["Severity"].Value, ulong.Parse(match.Groups["SequenceID"].Value), match.Groups["Content"].Value);

				
			}
		}



	}
}
