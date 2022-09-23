using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudiocodesSyslogLib
{
	public interface INotificationReader
	{
		
		IAsyncEnumerable<Notification> ReadNotificationsAsync(Stream Stream);

	}
}
