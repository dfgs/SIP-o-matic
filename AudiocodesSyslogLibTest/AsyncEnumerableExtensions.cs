using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudiocodesSyslogLibTest
{
	internal static class AsyncEnumerableExtensions
	{
		public static async Task<T[]> ToArrayAsync<T>(this IAsyncEnumerable<T> Items)
		{
			List<T> items;

			items = new List<T>();
			await foreach(T item in Items)
			{
				items.Add(item);
			}
			return items.ToArray();
		}
	}
}
