using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Loader.ContentPacks.Provider
{
    interface IDataProvider
    {
        bool Apply<TKey, TValue>(Dictionary<TKey, TValue> target, string path);
    }
}
