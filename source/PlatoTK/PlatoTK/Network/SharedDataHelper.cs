/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace PlatoTK.Network
{
    public class SharedDataHelper : ISharedDataHelper
    {
        private readonly IPlatoHelper Helper;

        private static HashSet<SharedDataCollection> Collections;

        public SharedDataHelper(IPlatoHelper helper)
        {
            Helper = helper;
            if (Collections == null)
                Collections = new HashSet<SharedDataCollection>();
        }

        public TModel ReadSharedData<TModel>(string collection, string key, bool allowNull = true) where TModel : class, new()
        {
            if (!Collections.Any(c => c.Id == collection && c.Helper == Helper))
                Collections.Add(new SharedDataCollection(Helper, collection));

            if (Collections.FirstOrDefault(c => c.Id == collection && c.Helper == Helper) is SharedDataCollection dataCollection)
                return dataCollection.Get<TModel>(key, allowNull);

            return null;
        }

        public void WriteSharedData<TModel>(string collection, string key, TModel data) where TModel : class, new()
        {
            if (!Collections.Any(c => c.Id == collection && c.Helper == Helper))
                Collections.Add(new SharedDataCollection(Helper, collection));

            if (Collections.First(c => c.Id == collection && c.Helper == Helper) is SharedDataCollection dataCollection)
                dataCollection.Set(key, data);
        }

        public void RemoveSharedData(string collection, string key)
        {
            if (Collections.FirstOrDefault(c => c.Id == collection && c.Helper == Helper) is SharedDataCollection dataCollection)
                dataCollection.Remove(key);
        }

        public void RemoveCollection(string collection, string key)
        {
            if (Collections.FirstOrDefault(c => c.Id == collection && c.Helper == Helper) is SharedDataCollection dataCollection)
            {
                dataCollection.Dispose();
                Collections.Remove(dataCollection);
            }
        }

        public void CreateCollection(string collection, Action<CollectionChangeArgs> onCollectionChanged = null)
        {
            if (!Collections.Any(c => c.Id == collection && c.Helper == Helper))
                Collections.Add(new SharedDataCollection(Helper, collection));

            Collections.First(e => e.Id == collection && e.Helper == Helper).RegisterObserver(onCollectionChanged);
        }

        public void ObserveCollection(string collection, Action<CollectionChangeArgs> onCollectionChanged)
        {
            if (Collections.FirstOrDefault(c => c.Id == collection && c.Helper == Helper) is SharedDataCollection dataCollection)
            {
                dataCollection.RegisterObserver(onCollectionChanged);
            }
        }

    }
}
