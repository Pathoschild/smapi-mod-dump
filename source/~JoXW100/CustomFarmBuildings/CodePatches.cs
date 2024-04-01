/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomFarmBuildings
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry
    {
        private static void Building_prefix(Building __instance, BluePrint blueprint, Vector2 tileLocation)
        {
            if (!Config.EnableMod || !buildingDict.TryGetValue(blueprint.name, out CustomBuildingData data))
                return;

            __instance.tilesWide.Value = data.width;
            __instance.tilesHigh.Value = data.height;
            __instance.humanDoor.Value = ;

        }

   }
}