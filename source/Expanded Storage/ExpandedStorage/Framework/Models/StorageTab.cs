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
using System.Linq;
using ImJustMatt.ExpandedStorage.API;
using ImJustMatt.ExpandedStorage.Framework.Extensions;
using StardewValley;

// ReSharper disable MemberCanBePrivate.Global

namespace ImJustMatt.ExpandedStorage.Framework.Models
{
    public class StorageTab : IStorageTab
    {
        /// <summary>The UniqueId of the Content Pack that storage data was loaded from.</summary>
        protected internal string ModUniqueId = "";

        /// <summary>The Asset path to the mod's Tab Image.</summary>
        internal string Path = "";

        internal StorageTab()
        {
        }

        internal StorageTab(string tabImage, params string[] allowList)
        {
            TabImage = tabImage;
            AllowList = new HashSet<string>(allowList);
        }

        public string TabName { get; set; }
        public string TabImage { get; set; }
        public HashSet<string> AllowList { get; set; } = new();
        public HashSet<string> BlockList { get; set; } = new();

        private bool IsAllowed(Item item)
        {
            return !AllowList.Any() || AllowList.Any(item.MatchesTagExt);
        }

        private bool IsBlocked(Item item)
        {
            return BlockList.Any() && BlockList.Any(item.MatchesTagExt);
        }

        internal bool Filter(Item item)
        {
            return IsAllowed(item) && !IsBlocked(item);
        }

        internal void CopyFrom(IStorageTab storageTab)
        {
            TabName = storageTab.TabName;
            TabImage = storageTab.TabImage;

            foreach (var allowItem in storageTab.AllowList)
            {
                AllowList.Add(allowItem);
            }

            foreach (var blockItem in storageTab.BlockList)
            {
                BlockList.Add(blockItem);
            }
        }
    }
}