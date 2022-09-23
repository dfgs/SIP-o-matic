using AudiocodesSyslogLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AudiocodesSyslogLibTest
{
	[TestClass]
	public class NotificationReaderUnitTest
	{
		
		private static Stream GetStream(string Name)
		{
			Stream? stream;
			stream=Assembly.GetExecutingAssembly().GetManifestResourceStream($"AudiocodesSyslogLibTest.Data.{Name}");
			if (stream==null) throw new FileNotFoundException(Name);
			return stream;
		}

		[TestMethod]
		public async Task ShouldCheckReadNotificationsAsyncParameters()
		{
			NotificationReader reader;

			reader = new NotificationReader(new SyslogReader());
			await Assert.ThrowsExceptionAsync<ArgumentNullException>(()=>reader.ReadNotificationsAsync(null).ToArrayAsync());
		}
		
		
		[TestMethod]
		public async Task ShouldReadOneNotification()
		{
			Notification[] notifications;
			NotificationReader reader;

			using (Stream stream = GetStream("OneNotification.txt"))
			{
				reader = new NotificationReader(new SyslogReader());
				notifications = await reader.ReadNotificationsAsync(stream).ToArrayAsync();
			}

			Assert.AreEqual(1, notifications.Length);
			Assert.AreEqual((ulong)17906355, notifications[0].NotificationID);
			Assert.AreEqual("(#268)Route found (8999), Route by IPGroup, IP Group 39 -> 0 (IPG_ACCESSIT -> IPG_VOIX_LINKER) ", notifications[0].Content);
		}
		[TestMethod]
		public async Task ShouldReadThreeNotifications()
		{
			Notification[] notifications;
			NotificationReader reader;

			using (Stream stream = GetStream("ThreeNotifications.txt"))
			{
				reader = new NotificationReader(new SyslogReader());
				notifications = await reader.ReadNotificationsAsync(stream).ToArrayAsync();
			}

			Assert.AreEqual(3, notifications.Length);
			Assert.AreEqual((ulong)17906355, notifications[0].NotificationID);
			Assert.AreEqual("(#268)Route found (8999), Route by IPGroup, IP Group 39 -> 0 (IPG_ACCESSIT -> IPG_VOIX_LINKER) ", notifications[0].Content);
			Assert.AreEqual((ulong)17906356, notifications[1].NotificationID);
			Assert.AreEqual((ulong)17906357, notifications[2].NotificationID);
		}
	}



}