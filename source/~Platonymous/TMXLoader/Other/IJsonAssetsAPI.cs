/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMXLoader.Other
{
    public interface IJsonAssetsAPI
    {
        bool FixIdsInItem(Item item);
        void FixIdsInItemList(List<Item> items);
        void FixIdsInLocation(GameLocation location);
    }
}
