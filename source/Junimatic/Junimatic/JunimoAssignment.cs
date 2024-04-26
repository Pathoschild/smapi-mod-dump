/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/Junimatic
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Inventories;


namespace NermNermNerm.Junimatic
{
    public record JunimoAssignment(
        JunimoType projectType,
        GameLocation location,
        StardewValley.Object hut,
        Point origin,
        GameInteractiveThing source,
        GameInteractiveThing target,
        List<Item>? itemsToRemoveFromChest);
}
