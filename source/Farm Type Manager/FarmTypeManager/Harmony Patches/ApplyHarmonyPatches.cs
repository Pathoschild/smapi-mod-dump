/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using Harmony;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Applies any Harmony patches used by this mod.</summary>
        private void ApplyHarmonyPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create(ModManifest.UniqueID); //create this mod's Harmony instance

            //apply all patches
            HarmonyPatch_AddSpawnedMineralsToCollections.ApplyPatch(harmony);
            HarmonyPatch_UpdateCursorOverPlacedItem.ApplyPatch(harmony);
        }
    }
}
