/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using OrnithologistsGuild.Game;
using StardewModdingAPI;

namespace OrnithologistsGuild
{
    public class TreePatches
    {
        private static IMonitor Monitor;

        public static void Initialize(IMonitor monitor, Harmony harmony)
        {
            Monitor = monitor;

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.TerrainFeatures.Tree), nameof(StardewValley.TerrainFeatures.Tree.performUseAction)),
                postfix: new HarmonyMethod(typeof(TreePatches), nameof(performUseAction_Postfix))
            );
        }

        public static void performUseAction_Postfix(StardewValley.TerrainFeatures.Tree __instance, Vector2 tileLocation)
        {
            try
            {
                var birdie = new Perch(__instance).GetOccupant(__instance.Location);
                if (birdie != null)
                {
                    birdie.Frighten();
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(performUseAction_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }
    }
}

