/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;

namespace FoodOnTheTable
{
    public interface IFurnitureDisplayFrameworkAPI
    {
        public int GetTotalSlots(Furniture f);
        Rectangle? GetSlotRect(Furniture f, int i);
        List<Object> GetSlotObjects(Furniture f);
    }
}