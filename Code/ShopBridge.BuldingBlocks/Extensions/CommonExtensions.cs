using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopBridge.BuldingBlocks.Extensions
{
    public static class CommonExtensions
    {
        public static bool IsNullOrEmpty<T>(this IList<T> items)
        {
            if (items == null || items.Count == 0)
                return true;
            return false;
        }
    }
}
