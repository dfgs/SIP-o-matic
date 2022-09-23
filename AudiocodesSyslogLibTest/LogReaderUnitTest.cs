using AudiocodesSyslogLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AudiocodesSyslogLibTest
{
	[TestClass]
	public class LogReaderUnitTest
	{
		
		private static Stream GetStream(string Name)
		{
			Stream? stream;
			stream=Assembly.GetExecutingAssembly().GetManifestResourceStream($"AudiocodesSyslogLibTest.Data.{Name}");
			if (stream==null) throw new FileNotFoundException(Name);
			return stream;
		}

		[TestMethod]
		public async Task ShouldCheckReadLogsAsyncParameters()
		{
			LogReader reader;

			reader = new LogReader(new SyslogReader());
			await Assert.ThrowsExceptionAsync<ArgumentNullException>(()=>reader.ReadLogsAsync(null).ToArrayAsync());
		}
		
		
		[TestMethod]
		public async Task ShouldReadOneLog()
		{
			string[] lines;
			LogReader reader;

			using (Stream stream = GetStream("OneLog.txt"))
			{
				reader = new LogReader(new SyslogReader());
				lines = await reader.ReadLogsAsync(stream).ToArrayAsync();
			}

			Assert.AreEqual(1, lines.Length);
			Assert.AreEqual("(N 17906355)  (#268)Route found (8999), Route by IPGroup, IP Group 39 -> 0 (IPG_ACCESSIT -> IPG_VOIX_LINKER) ", lines[0]);
		}
		[TestMethod]
		public async Task ShouldReadThreeLogs()
		{
			string[] lines;
			LogReader reader;

			using (Stream stream = GetStream("ThreeLogs.txt"))
			{
				reader = new LogReader(new SyslogReader());
				lines = await reader.ReadLogsAsync(stream).ToArrayAsync();
			}

			Assert.AreEqual(3, lines.Length);
			Assert.AreEqual("(N 17906355)  (#268)Route found (8999), Route by IPGroup, IP Group 39 -> 0 (IPG_ACCESSIT -> IPG_VOIX_LINKER) ", lines[0]);
		}
	}



}