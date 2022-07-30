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
using Microsoft.Xna.Framework.Graphics;
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
    internal class EventPatch
    {
        private static IMonitor _monitor;
        private static CustomCompanions _customCompanions;
        private readonly Type _event = typeof(Event);

        internal EventPatch(IMonitor modMonitor, CustomCompanions customCompanions)
        {
            _monitor = modMonitor;
            _customCompanions = customCompanions;
        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_event, "checkAction", new[] { typeof(xTile.Dimensions.Location), typeof(xTile.Dimensions.Rectangle), typeof(Farmer) }), postfix: new HarmonyMethod(GetType(), nameof(CheckActionPostfix)));
            harmony.Patch(AccessTools.Method(_event, "setUpCharacters", new[] { typeof(string), typeof(GameLocation) }), postfix: new HarmonyMethod(GetType(), nameof(SetUpCharactersPostfix)));
            harmony.Patch(AccessTools.Method(_event, "_checkForNextCommand", new[] { typeof(GameLocation), typeof(GameTime) }), prefix: new HarmonyMethod(GetType(), nameof(CheckForNextCommandPrefix)));
            harmony.Patch(AccessTools.Method(_event, "_checkForNextCommand", new[] { typeof(GameLocation), typeof(GameTime) }), postfix: new HarmonyMethod(GetType(), nameof(CheckForNextCommandPostfix)));
            harmony.Patch(AccessTools.Method(_event, "command_changeToTemporaryMap", new[] { typeof(GameLocation), typeof(GameTime), typeof(string[]) }), postfix: new HarmonyMethod(GetType(), nameof(ChangeToTemporaryMapPostfix)));
        }

        private static void CheckActionPostfix(Event __instance, xTile.Dimensions.Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who, GameLocation ___temporaryLocation, ref bool __result)
        {
            if (__result is true)
            {
                return;
            }

            foreach (NPC actor in __instance.actors)
            {
                if (actor.getTileX() == tileLocation.X && actor.getTileY() == tileLocation.Y && actor is Companion companion && companion is not null && String.IsNullOrEmpty(companion.GetDialogue(probe: true).Text) is false)
                {
                    companion.checkAction(who, ___temporaryLocation);
                    __result = true;
                }
            }
        }

        private static void SetUpCharactersPostfix(Event __instance, string description, GameLocation location)
        {
            foreach (MapCompanion companion in CompanionManager.sceneryCompanions.Where(c => c.Location == location).SelectMany(c => c.Companions))
            {
                if (companion.model.EnableEventAppearance)
                {
                    __instance.actors.Add(companion.Clone(true));
                }
            }
        }

        private static void CheckForNextCommandPrefix(Event __instance, GameLocation location, GameTime time, out List<MapCompanion> __state)
        {
            __state = new List<MapCompanion>();
            foreach (MapCompanion actor in __instance.actors.Where(a => a is MapCompanion).ToList())
            {
                actor.update(time, location);

                __state.Add(actor);
                __instance.actors.Remove(actor);
            }
        }

        private static void CheckForNextCommandPostfix(Event __instance, GameLocation location, GameTime time, List<MapCompanion> __state)
        {
            foreach (MapCompanion companion in __state)
            {
                __instance.actors.Add(companion);
            }
        }

        private static void ChangeToTemporaryMapPostfix(Event __instance, GameLocation ___temporaryLocation, GameLocation location, GameTime time, string[] split)
        {
            if (___temporaryLocation is not null)
            {
                _customCompanions.SpawnSceneryCompanions(___temporaryLocation, false);
            }

            foreach (MapCompanion companion in CompanionManager.sceneryCompanions.Where(c => c.Location == ___temporaryLocation).SelectMany(c => c.Companions))
            {
                if (companion.model.EnableEventAppearance)
                {
                    __instance.actors.Add(companion.Clone(true));
                }
            }

            foreach (MapCompanion companion in CompanionManager.sceneryCompanions.Where(c => c.Location == ___temporaryLocation).SelectMany(c => c.Companions).ToList())
            {
                companion.Despawn();
            }
        }
    }
}
