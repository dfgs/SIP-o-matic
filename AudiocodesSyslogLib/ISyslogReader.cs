using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudiocodesSyslogLib
{
	public interface ISyslogReader
	{
		IAsyncEnumerable<string> ReadBlocksAsync(Stream Stream);
		IAsyncEnumerable<Syslog> ReadSyslogsAsync(Stream Stream);

	}
}
