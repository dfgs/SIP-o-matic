using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.DataSources
{
	public interface IDataSource
	{
		IAsyncEnumerable<Event> EnumerateEventsAsync(string FileName);
		IAsyncEnumerable<Device> EnumerateDevicesAsync(string FileName);

		IEnumerable<string> GetSupportedFileExts();

	}
}
