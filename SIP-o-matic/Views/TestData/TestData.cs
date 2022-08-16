using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic.Views.TestData
{
	internal static class TestData
	{
		public static TestRow[] Rows = new TestRow[] {
			new TestRow() { LeftColumn = "B", RightColumn = "C", Text = "Line 1" },
			new TestRow() { LeftColumn = "A", RightColumn = "B", Text = "Line 2" },
			new TestRow() { LeftColumn = "A", RightColumn = "C", Text = "Line 3" },
		};

	}
}
