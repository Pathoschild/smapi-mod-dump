/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DotSharkTeeth/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using StardewValley;
using System.Reflection;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using HarmonyLib;
using Netcode;

namespace NoHatTreasureSkull
{
    public partial class ModEntry
    {
        public static void MineShaftGetTreasureRoomItem_postfix(ref Item __result)
        {
            
            if (__result.Category != -95)
                return;

            (Vector2, int)[] itemObjects = new[]
            {
                (Vector2.Zero, 21),
                (Vector2.Zero, 25),
                (Vector2.Zero, 165),
                (Vector2.Zero, 272)
            };
            int randomIndex = Game1.random.Next(itemObjects.Length);
            __result = new StardewValley.Object(itemObjects[randomIndex].Item1, itemObjects[randomIndex].Item2);

        }

        // Always Treasure room
        
        public static void MineShaftaddLevelChests_prefix(MineShaft __instance)
        {
            SHelper.Reflection.GetField<NetBool>(__instance, "netIsTreasureRoom").GetValue().Value = true;
            //__instance.netIsTreasureRoom.Value = true
        }
    }
}
