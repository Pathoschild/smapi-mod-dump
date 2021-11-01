/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/barteke22/StardewMods
**
*************************************************/

using DiagonalAim;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace DiagonalAim
{
    public class ModEntry : Mod
    {
        public static ModConfig config;

        public override void Entry(IModHelper helper)
        {
            Log.Monitor = Monitor;
            config = Helper.ReadConfig<ModConfig>();


            var harmony = new Harmony(ModManifest.UniqueID);//this might summon Cthulhu
            //harmony.Patch(
            //    original: AccessTools.Method(typeof(Character), "GetToolLocation", new Type[] { typeof(Vector2), typeof(bool) }),
            //    postfix: new HarmonyMethod(typeof(HarmonyPatches), "GetToolLocation_Diagonals")
            //);
            harmony.Patch(
                original: AccessTools.Method(typeof(Character), nameof(Character.GetToolLocation), new Type[] { typeof(Vector2), typeof(bool) }),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.GetToolLocation_Diagonals))
            );
        }
    }
}
