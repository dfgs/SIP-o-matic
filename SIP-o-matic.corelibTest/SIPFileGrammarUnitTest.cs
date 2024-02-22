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

			reader = new ParserLib.StringReader("[Devices]", ' ', '\r', '\n');
			result=SIPFileGrammar.DevicesHeader.Parse(reader);
			Assert.AreEqual("[Devices]",result);
		}

		[TestMethod]
		public void ShouldParseDeviceName()
		{
			IReader reader;
			string result;

			reader = new ParserLib.StringReader("LAB-VERIZON", ' ', '\r', '\n');
			result = SIPFileGrammar.DeviceName.Parse(reader);
			Assert.AreEqual("LAB-VERIZON", result);
		}

		[TestMethod]
		public void ShouldParseIPAddress()
		{
			IReader reader;
			string result;

			reader = new ParserLib.StringReader("10.46.247.14", ' ', '\r', '\n');
			result = SIPFileGrammar.IPAddress.Parse(reader);
			Assert.AreEqual("10.46.247.14", result);
		}
		[TestMethod]
		public void ShouldParseIPAddresses()
		{
			IReader reader;
			string[] result;

			reader = new ParserLib.StringReader("10.45.196.12, 10.45.213.211, 10.91.226.66, 10.91.226.67, 10.45.196.11",' ', '\r', '\n');
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

			reader = new ParserLib.StringReader("SVI-CMA (10.45.196.12, 10.45.213.211, 10.91.226.66, 10.91.226.67, 10.45.196.11 )", ' ', '\r', '\n');
			result = SIPFileGrammar.Device.Parse(reader);
			Assert.AreEqual("SVI-CMA", result.Name);
			Assert.AreEqual(5, result.Addresses.Count);
			Assert.AreEqual("10.45.196.12", result.Addresses[0]);
			Assert.AreEqual("10.45.213.211", result.Addresses[1]);
			Assert.AreEqual("10.91.226.66", result.Addresses[2]);
			Assert.AreEqual("10.91.226.67", result.Addresses[3]);
			Assert.AreEqual("10.45.196.11", result.Addresses[4]);
		}

		[TestMethod]
		public void ShouldParseDevices()
		{
			IReader reader;
			Device[] result;

			reader = new ParserLib.StringReader("SVI-CMA (10.45.196.12, 10.45.213.211, 10.91.226.66, 10.91.226.67, 10.45.196.11 )\r\nGenesys-SIP-Proxy-CMA (10.91.219.12, 10.45.213.147 )", ' ','\r','\n');
			result = SIPFileGrammar.Devices.Parse(reader).ToArray();

			Assert.AreEqual(2, result.Length);
			Assert.AreEqual("SVI-CMA", result[0].Name);
			Assert.AreEqual("Genesys-SIP-Proxy-CMA", result[1].Name);
		}
		[TestMethod]
		public void ShouldParseDevicesEnumerator()
		{
			IReader reader;
			Device[] result;

			reader = new ParserLib.StringReader("[Devices]\r\n\r\nSVI-CMA (10.45.196.12, 10.45.213.211, 10.91.226.66, 10.91.226.67, 10.45.196.11 )\r\nGenesys-SIP-Proxy-CMA (10.91.219.12, 10.45.213.147 )", ' ', '\r', '\n');
			result = SIPFileGrammar.DeviceEnumerator.Parse(reader).ToArray();

			Assert.AreEqual(2, result.Length);
			Assert.AreEqual("SVI-CMA", result[0].Name);
			Assert.AreEqual("Genesys-SIP-Proxy-CMA", result[1].Name);
		}



		[TestMethod]
		public void ShouldParseMessagesHeader()
		{
			IReader reader;
			string result;

			reader = new ParserLib.StringReader("[Messages]", ' ', '\r', '\n');
			result = SIPFileGrammar.MessagesHeader.Parse(reader);
			Assert.AreEqual("[Messages]", result);
		}


		[TestMethod]
		public void ShouldParseSeparatorLine()
		{
			IReader reader;
			string result;

			reader = new ParserLib.StringReader("-------------", ' ', '\r', '\n');
			result = SIPFileGrammar.SeparatorLine.Parse(reader);
			Assert.AreEqual("-------------", result);
		}


		[TestMethod]
		public void ShouldParseDate()
		{
			IReader reader;
			DateTime result;
		
			reader = new ParserLib.StringReader("2023-10-25 21:19:38.4846", ' ', '\r', '\n');
			result = SIPFileGrammar.Date.Parse(reader);
			Assert.AreEqual(2023, result.Year);
			Assert.AreEqual(10, result.Month);
			Assert.AreEqual(25, result.Day);
			Assert.AreEqual(21, result.Hour);
			Assert.AreEqual(19, result.Minute);
			Assert.AreEqual(38, result.Second);
			Assert.AreEqual(484, result.Millisecond);
			Assert.AreEqual(600, result.Microsecond);
		}


	}
}