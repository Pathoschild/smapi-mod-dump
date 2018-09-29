using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMultiplayerInfo.Helpers
{
    public static class DictionaryExtensions
    {
        public static TTypeB GetOrCreateDefault<TTypeA, TTypeB>(this Dictionary<TTypeA, TTypeB> input, TTypeA key)
        where TTypeB : class, new()
        {
            if (!input.ContainsKey(key))
            {
                input.Add(key, new TTypeB());
            }

            return input[key];
        }

    }
}
