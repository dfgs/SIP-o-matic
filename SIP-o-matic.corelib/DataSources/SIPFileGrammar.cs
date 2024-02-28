using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserLib;
using SIP_o_matic.corelib.Models;

namespace SIP_o_matic.corelib.DataSources
{
	public static class SIPFileGrammar
	{
		
		public static ISingleParser<string> DevicesHeader = Parse.String("[Devices]", true);
		public static ISingleParser<string> DeviceName =
			from _1 in Parse.Char('"')
			from name in Parse.Except('"').ReaderIncludes(' ').OneOrMoreTimes().ToStringParser()
			from _2 in Parse.Char('"')
			select name;


		public static ISingleParser<string> IPAddress = Parse.Digit().OneOrMoreTimes().ToStringParser().Then(Parse.Char('.').ToStringParser()).Then(Parse.Digit().OneOrMoreTimes().ToStringParser()).Then(Parse.Char('.').ToStringParser()).Then(Parse.Digit().OneOrMoreTimes().ToStringParser()).Then(Parse.Char('.').ToStringParser()).Then(Parse.Digit().OneOrMoreTimes().ToStringParser()).ToStringParser();



		public static IMultipleParser<string> IPAddresses = IPAddress.List(Parse.Char(','));

		public static ISingleParser<Device> Device = from name in DeviceName
													 from _1 in Parse.Char('{')
													 from adresses in IPAddresses
													 from _2 in Parse.Char('}')
													 select new Device(name,adresses);
		public static IMultipleParser<Device> Devices = Device.ZeroOrMoreTimes();




		public static ISingleParser<string> MessagesHeader = Parse.String("[Messages]", true);
		public static ISingleParser<string> SeparatorLine = Parse.Char('-').OneOrMoreTimes().ToStringParser();


		public static ISingleParser<string> LineFeed = Parse.String("\r\n").ReaderIncludes('\r', '\n', ' ');

		public static ISingleParser<string> FullLine = from content in Parse.Except('-').Then(Parse.Except('\r').ReaderIncludes('\r', '\n', ' ').OneOrMoreTimes()).ToStringParser()
														  from LF in LineFeed
														  select content+LineFeed;
		public static ISingleParser<string> ContentLine = LineFeed.Or(FullLine);

		public static IMultipleParser<string> ContentLines = ContentLine.OneOrMoreTimes();

		public static ISingleParser<DateTime> Date =
			from year in Parse.Int()
			from _1 in Parse.Char('-')
			from month in Parse.Int()
			from _2 in Parse.Char('-')
			from day in Parse.Int().ReaderIncludes(' ')
			from _3 in Parse.Char(' ').ReaderIncludes(' ')
			from hours in Parse.Int()
			from _4 in Parse.Char(':')
			from minutes in Parse.Int()
			from _5 in Parse.Char(':')
			from seconds in Parse.Int()
			from _6 in Parse.Char('.')
			from milliSeconds in Parse.Int()
			select new DateTime(year, month, day, hours, minutes, seconds, milliSeconds/10,milliSeconds%10 *100);

		public static ISingleParser<Message> Message = from timeStamp in Date
													   from _2 in Parse.String("from", true)
													   from source in IPAddress
													   from _3 in Parse.String("to", true)
													   from destination in IPAddress
													   from _4 in LineFeed
													   from content in ContentLines
													   from _1 in SeparatorLine
													   select new Message(0, timeStamp, source, destination, string.Join("",content));

		public static IMultipleParser<Message> Messages = Message.ZeroOrMoreTimes();

		public static ISingleParser<Device[]> DeviceEnumerator = from _1 in DevicesHeader
																 from devices in Devices
																 select devices.ToArray();

		public static ISingleParser<Message[]> MessageEnumerator = from _2 in MessagesHeader
																   from messages in Messages
																   select messages.ToArray();




	}
}
