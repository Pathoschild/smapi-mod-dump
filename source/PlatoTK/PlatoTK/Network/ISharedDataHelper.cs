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

namespace PlatoTK.Network
{
    public interface ISharedDataHelper
    {
        TModel ReadSharedData<TModel>(string collection, string key, bool allowNull = true) where TModel : class, new();

        void WriteSharedData<TModel>(string collection, string key, TModel data) where TModel : class, new();

        void RemoveSharedData(string collection, string key);

        void RemoveCollection(string collection, string key);

        void CreateCollection(string collection, Action<CollectionChangeArgs> onCollectionChanged = null);

        void ObserveCollection(string collection, Action<CollectionChangeArgs> onCollectionChanged);
    }
}
