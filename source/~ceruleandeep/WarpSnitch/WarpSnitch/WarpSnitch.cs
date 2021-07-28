/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using Harmony;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using StardewValley.Network;

namespace WarpSnitch
{
    /// <summary>The mod entry point.</summary>
    public class WarpSnitch : Mod
    {
        public static IMonitor SMonitor;
        
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            SMonitor = Monitor;

            HarmonyInstance harmony = HarmonyInstance.Create("ceruleandeep.warpsnitch");
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), "warpCharacter", parameters: new Type[] { typeof(NPC), typeof(GameLocation), typeof(Vector2) }),
                prefix: new HarmonyMethod(typeof(WarpSnitch), nameof(Game1_warpCharacter_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), "warpCharacter", parameters: new Type[] { typeof(NPC), typeof(string), typeof(Vector2) }),
                prefix: new HarmonyMethod(typeof(WarpSnitch), nameof(Game1_warpCharacter_String_Prefix))
            );
        }

        [HarmonyPriority(Priority.High)]
        public static bool Game1_warpCharacter_String_Prefix(Game1 __instance, NPC character, string targetLocationName)
        {
            if (character is null)
            {
                SMonitor.Log($"warpCharacter: character is null", LogLevel.Warn);
                return false;
            }

            if (targetLocationName is null)
            {
                SMonitor.Log($"warpCharacter: {character.displayName}'s targetLocationName is null", LogLevel.Warn);
                return false;
            }

            if (Game1.getLocationFromName(targetLocationName) is null)
            {
                SMonitor.Log($"warpCharacter: getLocationFromName returns null for {targetLocationName} [NPC: {character.displayName}]", LogLevel.Warn);
                return false;
            }
            return true;
        }
        
        [HarmonyPriority(Priority.High)]
        public static bool Game1_warpCharacter_Prefix(Game1 __instance, NPC character, GameLocation targetLocation)
        {
            if (character is null)
            {
                SMonitor.Log($"warpCharacter: character is null", LogLevel.Warn);
                return false;
            }

            if (targetLocation is null)
            {
                SMonitor.Log($"warpCharacter: {character.displayName}'s targetLocation is null", LogLevel.Warn);
                return false;
            }
            
            SMonitor.Log($"Warping {character.displayName} to {targetLocation.Name}", LogLevel.Trace);
            return true;
        }
    }
}