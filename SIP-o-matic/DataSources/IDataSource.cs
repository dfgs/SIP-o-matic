using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIP_o_matic.Models;

namespace SIP_o_matic.DataSources
{
    public interface IDataSource
	{
		IAsyncEnumerable<Message> EnumerateMessagesAsync(string FileName);
		IAsyncEnumerable<Device> EnumerateDevicesAsync(string FileName);

		IEnumerable<string> GetSupportedFileExts();

		string Description
		{
			get;
		}

	}
}
