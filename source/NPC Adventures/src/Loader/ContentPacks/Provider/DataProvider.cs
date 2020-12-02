/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Loader.ContentPacks.Provider
{
    class DataProvider : IDataProvider
    {
        const string DATA_ROOT = "Data/";

        public ManagedContentPack Managed { get; }

        public DataProvider(ManagedContentPack managed)
        {
            this.Managed = managed;
        }

        public Dictionary<TKey, TValue> Provide<TKey, TValue>(string path)
        {
            // TODO: For future usage, needs rewrite
            if (path.StartsWith(DATA_ROOT))
            {
                return this.ResolveDataContents<TKey, TValue>(path.Remove(0, DATA_ROOT.Length));
            }

            return null;
        }

        private Dictionary<TKey, TValue> ResolveDataContents<TKey, TValue>(string subPath)
        {
            /*switch (subPath)
            {
                case "CompanionDispositions":
                    if (this.Managed.Contents.Companions != null)
                        return AsDictionary<TKey, TValue>(this.Managed.Contents.Companions);
                    break;
            }*/

            return null;
        }

        private static Dictionary<TKey, TValue> AsDictionary<TKey, TValue>(object obj)
        {
            return CastTo<Dictionary<TKey, TValue>>(obj);
        }

        private static T CastTo<T>(object obj)
        {
            if (!(obj is T))
                throw new InvalidCastException($"The content data of type {obj.GetType().FullName} can't be converted to the requested {typeof(T).FullName}.");

            return (T)obj;
        }

        public bool Apply<TKey, TValue>(Dictionary<TKey, TValue> target, string path)
        {
            return false;
        }
    }
}
