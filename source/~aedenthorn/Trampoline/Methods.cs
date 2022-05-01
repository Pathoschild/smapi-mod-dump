/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using Object = StardewValley.Object;

namespace Trampoline
{
    public partial class ModEntry
    {
        private static bool IsOnTrampoline(Farmer farmer = null)
        {
            if (farmer == null)
                farmer = Game1.player;
            return farmer.IsSitting() && farmer.sittingFurniture is Furniture && (farmer.sittingFurniture as Furniture).modData.ContainsKey(trampolineKey);
        }
    }
}