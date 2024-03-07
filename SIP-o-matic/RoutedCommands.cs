using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SIP_o_matic
{
	public static  class RoutedCommands
	{
		public static RoutedCommand AddFile = new RoutedCommand();
		public static RoutedCommand RemoveFile = new RoutedCommand();

		public static RoutedCommand Analyze = new RoutedCommand();

		public static RoutedCommand OK = new RoutedCommand();
		public static RoutedCommand Cancel = new RoutedCommand();

		public static RoutedCommand ExportSIP = new RoutedCommand();
		public static RoutedCommand ExportPPT = new RoutedCommand();


		public static RoutedCommand AddDevice = new RoutedCommand();
		public static RoutedCommand AddAddress = new RoutedCommand();
		public static RoutedCommand RemoveDeviceOrAddress = new RoutedCommand();
		public static RoutedCommand EditDeviceOrAddress = new RoutedCommand();
	}
}
