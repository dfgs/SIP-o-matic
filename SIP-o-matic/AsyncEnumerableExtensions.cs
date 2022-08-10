using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIP_o_matic
{
    public static class AsyncEnumerableExtensions
    {

        public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> Enumerable)
        {
            IEnumerator<T> enumerator;

            enumerator = await Task.Run(()=> Enumerable.GetEnumerator());
            try
            {
                while (await Task.Run(()=> enumerator.MoveNext()))
                {
                    yield return enumerator.Current;
                    
                }
            }
            finally { await Task.Run(()=> enumerator?.Dispose()); }
        }
    }
}
