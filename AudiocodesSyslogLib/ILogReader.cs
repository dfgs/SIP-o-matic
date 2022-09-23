using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudiocodesSyslogLib
{
	public interface ILogReader
	{
		
		IAsyncEnumerable<string> ReadLogsAsync(Stream Stream);

	}
}
