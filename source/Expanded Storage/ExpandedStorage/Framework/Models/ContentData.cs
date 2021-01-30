/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/ExpandedStorage
**
*************************************************/

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable ClassNeverInstantiated.Global
namespace ExpandedStorage.Framework.Models
{
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
    internal class ContentData
    {
        public IList<StorageContentData> ExpandedStorage = new List<StorageContentData>();
        public IList<TabContentData> StorageTabs = new List<TabContentData>();
    }
}