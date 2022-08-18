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
		public static RoutedCommand CopyLogs = new RoutedCommand();
		public static RoutedCommand ZoomIn = new RoutedCommand();
		public static RoutedCommand ZoomOut = new RoutedCommand();
		public static RoutedCommand Refresh = new RoutedCommand();
		public static RoutedCommand OK = new RoutedCommand();
		public static RoutedCommand Cancel = new RoutedCommand();
	}
}
