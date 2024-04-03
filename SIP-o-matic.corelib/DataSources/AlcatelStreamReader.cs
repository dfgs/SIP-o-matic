using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.corelib.DataSources
{

	// some Alcatel files contains invalid lines ending with \r\r\n
	public class AlcatelStreamReader : StreamReader
	{
		public AlcatelStreamReader(Stream Stream) : base(Stream)
		{

		}

		public override string? ReadLine()
		{
			StringBuilder stringBuilder;
			char car;

			stringBuilder = new StringBuilder();
			if (this.EndOfStream) return base.ReadLine();

			do
			{
				car = (char)Read();
				if (car == '\r') continue;
				if (car == '\n') return stringBuilder.ToString();
				stringBuilder.Append(car);
			} while (!this.EndOfStream);
			return stringBuilder.ToString();

		}


	}
}
