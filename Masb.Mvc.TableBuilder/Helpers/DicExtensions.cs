using System.Collections.Generic;

namespace Masb.Mvc.TableBuilder.Helpers
{
    internal static class DicExtensions
    {
        internal static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key)
        {
            TValue value;
            dic.TryGetValue(key, out value);
            return value;
        }
    }
}