using AudiocodesSyslogLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AudiocodesSyslogLibTest
{
	[TestClass]
	public class SyslogReaderUnitTest
	{
		
		private static Stream GetStream(string Name)
		{
			Stream? stream;
			stream=Assembly.GetExecutingAssembly().GetManifestResourceStream($"AudiocodesSyslogLibTest.Data.{Name}");
			if (stream==null) throw new FileNotFoundException(Name);
			return stream;
		}

		[TestMethod]
		public async Task ShouldCheckReadBlocksAsyncParameters()
		{
			SyslogReader reader;

			reader = new SyslogReader();
			await Assert.ThrowsExceptionAsync<ArgumentNullException>(()=>reader.ReadBlocksAsync(null).ToArrayAsync());
		}
		[TestMethod]
		public async Task ShouldCheckReadLogsAsyncParameters()
		{
			SyslogReader reader;

			reader = new SyslogReader();
			await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => reader.ReadSyslogsAsync(null).ToArrayAsync());
		}
		[TestMethod]
		public async Task ShouldNotReadBlock()
		{
			string[] lines;
			SyslogReader reader;

			using (Stream stream = GetStream("BadBlock.txt"))
			{
				reader = new SyslogReader();
				lines = await reader.ReadBlocksAsync(stream).ToArrayAsync();
			}

			Assert.AreEqual(0, lines.Length);	
		}

		[TestMethod]
		public async Task ShouldReadOneBlock()
		{
			string[] lines;
			SyslogReader reader;

			using (Stream stream = GetStream("OneBlock.txt"))
			{
				reader = new SyslogReader();
				lines = await reader.ReadBlocksAsync(stream).ToArrayAsync();
			}

			Assert.AreEqual(1, lines.Length);
			Assert.AreEqual(10, lines[0].Split("\r\n").Length);
		}
		[TestMethod]
		public async Task ShouldReadTwoBlocks()
		{
			string[] lines;
			SyslogReader reader;

			using (Stream stream = GetStream("TwoBlocks.txt"))
			{
				reader = new SyslogReader();
				lines = await reader.ReadBlocksAsync(stream).ToArrayAsync();
			}

			Assert.AreEqual(2, lines.Length);
			Assert.AreEqual(1, lines[0].Split("\r\n").Length);
			Assert.AreEqual(5, lines[1].Split("\r\n").Length);
		}
		[TestMethod]
		public async Task ShouldSkipBadBlockAndReadTwoBlocks()
		{
			string[] lines;
			SyslogReader reader;

			using (Stream stream = GetStream("BadBlockThenTwoBlocks.txt"))
			{
				reader = new SyslogReader();
				lines = await reader.ReadBlocksAsync(stream).ToArrayAsync();
			}

			Assert.AreEqual(2, lines.Length);
			Assert.AreEqual(1, lines[0].Split("\r\n").Length);
			Assert.AreEqual(5, lines[1].Split("\r\n").Length);
		}
		[TestMethod]
		public async Task ShouldReadFiveBlocks()
		{
			string[] lines;
			SyslogReader reader;

			using (Stream stream = GetStream("ThreeLogs.txt"))
			{
				reader = new SyslogReader();
				lines = await reader.ReadBlocksAsync(stream).ToArrayAsync();
			}

			Assert.AreEqual(5, lines.Length);
			Assert.AreEqual(3, lines[0].Split("\r\n").Length);
			Assert.AreEqual(29, lines[1].Split("\r\n").Length);
		}


		[TestMethod]
		public async Task ShouldNotReadLog()
		{
			Syslog[] logs;
			SyslogReader reader;

			using (Stream stream = GetStream("BadBlock.txt"))
			{
				reader = new SyslogReader();
				logs = await reader.ReadSyslogsAsync(stream).ToArrayAsync();
			}

			Assert.AreEqual(0, logs.Length);
		}
		
		[TestMethod]
		public async Task ShouldReadOneLog()
		{
			Syslog[] logs;
			SyslogReader reader;

			using (Stream stream = GetStream("OneBlock.txt"))
			{
				reader = new SyslogReader();
				logs = await reader.ReadSyslogsAsync(stream).ToArrayAsync();
			}

			Assert.AreEqual(1, logs.Length);
			Assert.AreEqual(10, logs[0].Content.Split("\r\n").Length);
		}
		[TestMethod]
		public async Task ShouldReadTwoLogs()
		{
			Syslog[] logs;
			SyslogReader reader;

			using (Stream stream = GetStream("TwoBlocks.txt"))
			{
				reader = new SyslogReader();
				logs = await reader.ReadSyslogsAsync(stream).ToArrayAsync();
			}

			Assert.AreEqual(2, logs.Length);
			Assert.AreEqual(1, logs[0].Content.Split("\r\n").Length);
			Assert.AreEqual(5, logs[1].Content.Split("\r\n").Length);
		}
		[TestMethod]
		public async Task ShouldSkipBadLogAndReadTwoLogs()
		{
			Syslog[] logs;
			SyslogReader reader;

			using (Stream stream = GetStream("BadBlockThenTwoBlocks.txt"))
			{
				reader = new SyslogReader();
				logs = await reader.ReadSyslogsAsync(stream).ToArrayAsync();
			}

			Assert.AreEqual(2, logs.Length);
			Assert.AreEqual(1, logs[0].Content.Split("\r\n").Length);
			Assert.AreEqual(5, logs[1].Content.Split("\r\n").Length);
		}
		[TestMethod]
		public async Task ShouldReadFiveLogs()
		{
			Syslog[] logs;
			SyslogReader reader;

			using (Stream stream = GetStream("ThreeLogs.txt"))
			{
				reader = new SyslogReader();
				logs = await reader.ReadSyslogsAsync(stream).ToArrayAsync();
			}

			Assert.AreEqual(5, logs.Length);
			Assert.AreEqual(3, logs[0].Content.Split("\r\n").Length);
			Assert.AreEqual(29, logs[1].Content.Split("\r\n").Length);
		}

		[TestMethod]
		public async Task ShouldReadSessionLogs()
		{
			Syslog[] logs;
			SyslogReader reader;

			using (Stream stream = GetStream("SessionLog.txt"))
			{
				reader = new SyslogReader();
				logs = await reader.ReadSyslogsAsync(stream).ToArrayAsync();
			}

			Assert.AreEqual(1, logs.Length);
			Assert.IsInstanceOfType(logs[0], typeof(SessionSyslog));
			Assert.AreEqual((ulong)159026163, ((SessionSyslog)logs[0]).SequenceNumber);
			Assert.AreEqual("local0.notice", ((SessionSyslog)logs[0]).Severity);
			Assert.AreEqual("c81f29:26:631315", ((SessionSyslog)logs[0]).SessionId);
		}
		[TestMethod]
		public async Task ShouldReadBoardLogs()
		{
			Syslog[] logs;
			SyslogReader reader;

			using (Stream stream = GetStream("BoardLog.txt"))
			{
				reader = new SyslogReader();
				logs = await reader.ReadSyslogsAsync(stream).ToArrayAsync();
			}

			Assert.AreEqual(1, logs.Length);
			Assert.IsInstanceOfType(logs[0], typeof(BoardSyslog));
			Assert.AreEqual("local0.notice", ((BoardSyslog)logs[0]).Severity);
			Assert.AreEqual((ulong)2506, ((BoardSyslog)logs[0]).SequenceNumber);
			Assert.AreEqual("b0883a:27", ((BoardSyslog)logs[0]).BoardId);
		}


	}
}