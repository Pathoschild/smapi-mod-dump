/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using ImJustMatt.Common.Extensions;
using ImJustMatt.ExpandedStorage.API;
using ImJustMatt.ExpandedStorage.Framework.Models;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewValley;

namespace ImJustMatt.ExpandedStorage.Framework.Controllers
{
    public class TabController : TabModel
    {
        /// <summary>The UniqueId of the Content Pack that storage data was loaded from.</summary>
        protected internal string ModUniqueId = "";

        /// <summary>The Asset path to the mod's Tab Image.</summary>
        internal string Path = "";

        /// <summary>Display Name for tab.</summary>
        public string TabName;

        [JsonConstructor]
        internal TabController(ITab storageTab = null)
        {
            if (storageTab == null)
                return;

            TabImage = storageTab.TabImage;
            AllowList = new HashSet<string>(storageTab.AllowList);
            BlockList = new HashSet<string>(storageTab.BlockList);
        }

        internal TabController(string tabImage, params string[] allowList)
        {
            TabImage = tabImage;
            AllowList = new HashSet<string>(allowList);
        }

        internal Func<Texture2D> Texture { get; set; }

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
    }
}