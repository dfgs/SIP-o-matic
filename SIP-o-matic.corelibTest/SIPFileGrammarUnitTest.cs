using ParserLib;
using SIP_o_matic.corelib.DataSources;

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

			reader = new ParserLib.StringReader("[Devices]");
			result=SIPFileGrammar.DevicesHeader.Parse(reader);
			Assert.AreEqual("[Devices]",result);

		}
	}
}