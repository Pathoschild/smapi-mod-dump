/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/CustomCompanions
**
*************************************************/

using CustomCompanions.Framework.Companions;
using CustomCompanions.Framework.Managers;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCompanions.Framework.Patches
{
    internal class UtilityPatch
    {
        private static IMonitor monitor;
        private readonly Type _utility = typeof(Utility);

        internal UtilityPatch(IMonitor modMonitor)
        {
            monitor = modMonitor;
        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_utility, nameof(Utility.GetNpcsWithinDistance), new[] { typeof(Vector2), typeof(int), typeof(GameLocation) }), postfix: new HarmonyMethod(GetType(), nameof(GetNpcsWithinDistancePostfix)));
            harmony.Patch(AccessTools.Method(_utility, nameof(Utility.checkForCharacterInteractionAtTile), new[] { typeof(Vector2), typeof(Farmer) }), postfix: new HarmonyMethod(GetType(), nameof(CheckForCharacterInteractionAtTilePostfix)));
        }

        private static void GetNpcsWithinDistancePostfix(Utility __instance, ref IEnumerable<NPC> __result, Vector2 centerTile, int tilesAway, GameLocation location)
        {
            if (__result != null)
            {
                __result = __result.Where(n => CompanionManager.IsCustomCompanion(n) is false);
            }
        }

        private static void CheckForCharacterInteractionAtTilePostfix(Utility __instance, Vector2 tileLocation, Farmer who)
        {
            NPC character = Game1.currentLocation.isCharacterAtTile(tileLocation);
            if (CompanionManager.IsCustomCompanion(character))
            {
                Companion companion = character as Companion;
                if (companion.owner is null && companion.model != null && String.IsNullOrEmpty(companion.GetDialogue(probe: true).Text) is false)
                {
                    Game1.mouseCursor = 4;
                    Game1.mouseCursorTransparency = Utility.tileWithinRadiusOfPlayer((int)tileLocation.X, (int)tileLocation.Y, 1, who) ? 1f : 0.5f;
                }
            }
        }
    }
}
