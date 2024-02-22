using ParserLib;
using SIP_o_matic.corelib.DataSources;
using SIP_o_matic.corelib.Models;

namespace SIP_o_matic.corelibTest
{
	[TestClass]
	public class SIPFileGrammarUnitTest
	{
		[TestMethod]
		public void ShouldParseDevicesHeader()
		{
			IReader reader;
			string result;

			reader = new ParserLib.StringReader("[Devices]", ' ');
			result=SIPFileGrammar.DevicesHeader.Parse(reader);
			Assert.AreEqual("[Devices]",result);
		}

		[TestMethod]
		public void ShouldParseDeviceName()
		{
			IReader reader;
			string result;

			reader = new ParserLib.StringReader("LAB-VERIZON", ' ');
			result = SIPFileGrammar.DeviceName.Parse(reader);
			Assert.AreEqual("LAB-VERIZON", result);
		}

		[TestMethod]
		public void ShouldParseIPAddress()
		{
			IReader reader;
			string result;

			reader = new ParserLib.StringReader("10.46.247.14", ' ');
			result = SIPFileGrammar.IPAddress.Parse(reader);
			Assert.AreEqual("10.46.247.14", result);
		}
		[TestMethod]
		public void ShouldParseIPAddresses()
		{
			IReader reader;
			string[] result;

			reader = new ParserLib.StringReader("10.45.196.12, 10.45.213.211, 10.91.226.66, 10.91.226.67, 10.45.196.11",' ');
			result = SIPFileGrammar.IPAddresses.Parse(reader).ToArray();
			Assert.AreEqual(5, result.Length);
			Assert.AreEqual("10.45.196.12", result[0]);
			Assert.AreEqual("10.45.213.211", result[1]);
			Assert.AreEqual("10.91.226.66", result[2]);
			Assert.AreEqual("10.91.226.67", result[3]);
			Assert.AreEqual("10.45.196.11", result[4]);
		}

		[TestMethod]
		public void ShouldParseDevice()
		{
			IReader reader;
			Device result;

			reader = new ParserLib.StringReader("SVI-CMA (10.45.196.12, 10.45.213.211, 10.91.226.66, 10.91.226.67, 10.45.196.11 )", ' ');
			result = SIPFileGrammar.Device.Parse(reader);
			Assert.AreEqual("SVI-CMA", result.Name);
			Assert.AreEqual(5, result.Addresses.Count);
			Assert.AreEqual("10.45.196.12", result.Addresses[0]);
			Assert.AreEqual("10.45.213.211", result.Addresses[1]);
			Assert.AreEqual("10.91.226.66", result.Addresses[2]);
			Assert.AreEqual("10.91.226.67", result.Addresses[3]);
			Assert.AreEqual("10.45.196.11", result.Addresses[4]);
		}

	}
}