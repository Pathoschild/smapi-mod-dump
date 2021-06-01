/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlashShifter/StardewValleyExpanded
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewValley;
using Harmony;
using System.Collections.Generic;
using System.Linq;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using StardewValley.Monsters;
using System.Diagnostics;
using StardewValley.Objects;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Events;
using StardewValley.Characters;
using xTile.Dimensions;
using Netcode;
using StardewValley.Network;
using System.Reflection.Emit;
using System.Reflection;
using xTile.ObjectModel;

namespace StardewValleyExpanded
{

    internal static class EndNexusMusic

    {
        private static IMonitor Monitor;

        public static void Hook(HarmonyInstance harmony, IMonitor monitor)
        {
            EndNexusMusic.Monitor = monitor;

            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), "resetLocalState"),
                prefix: new HarmonyMethod(typeof(EndNexusMusic), nameof(EndNexusMusic.After_ResetLocalState))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.cleanupBeforePlayerExit)),
                prefix: new HarmonyMethod(typeof(EndNexusMusic), nameof(EndNexusMusic.After_CleanupBeforePlayerExit))
            );

        }


        private static void After_ResetLocalState(GameLocation __instance)
        {
            if (Game1.currentLocation.NameOrUniqueName == "Custom_EnchantedGrove")
            {
                Game1.changeMusicTrack("cm:Nexus", music_context: Game1.MusicContext.Default);
            }

            if (Game1.currentLocation.NameOrUniqueName == "Custom_JojaEmporium")
            {
                Game1.changeMusicTrack("movieTheater", music_context: Game1.MusicContext.Default);
            }
        }

        private static void After_CleanupBeforePlayerExit(GameLocation __instance)
        {
            if (Game1.currentLocation.NameOrUniqueName == "Custom_EnchantedGrove")
            {
                Game1.changeMusicTrack("none", music_context: Game1.MusicContext.Default);
            }

            if (Game1.currentLocation.NameOrUniqueName == "Custom_JojaEmporium")
            {
                Game1.changeMusicTrack("none", music_context: Game1.MusicContext.Default);
            }
        }


    }
}
