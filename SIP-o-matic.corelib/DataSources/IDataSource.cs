using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIP_o_matic.corelib.Models;

namespace SIP_o_matic.corelib.DataSources
{
    public interface IDataSource
	{
		string Description
		{
			get;
		}

		IEnumerable<Message> EnumerateMessages();
		IEnumerable<Device> EnumerateDevices();

		IEnumerable<string> GetSupportedFileExts();

		Task LoadAsync(string FileName);


	}
}
