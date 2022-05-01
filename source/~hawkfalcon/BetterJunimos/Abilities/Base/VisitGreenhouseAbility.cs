/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hawkfalcon/Stardew-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using BetterJunimos.Utils;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.TerrainFeatures;

namespace BetterJunimos.Abilities {
    public class VisitGreenhouseAbility : IJunimoAbility {
        public string AbilityName() {
            return "VisitGreenhouse";
        }

        public bool IsActionAvailable(GameLocation location, Vector2 pos, Guid guid) {
            if (!BetterJunimos.Config.JunimoImprovements.CanWorkInGreenhouse) return false;
            if (!location.IsFarm) return false;

            var greenhouse = Game1.getLocationFromName("Greenhouse");
            if (greenhouse.characters.Count(npc => npc is JunimoHarvester) > Util.Progression.MaxJunimosUnlocked / 2) {
                // greenhouse already kinda full
                return false;
            }

            var hut = Util.GetHutFromId(guid);
            if (!Util.Abilities.lastKnownCropLocations.TryGetValue((hut, greenhouse), out var lkc)) {
                if (!Patches.PatchSearchAroundHut.SearchGreenhouseGrid(Util.GetHutFromId(guid), guid)) {
                    // no work to be done in greenhouse
                    // BetterJunimos.SMonitor.Log("VisitGreenhouse IsActionAvailable: no work", LogLevel.Debug);
                    return false;
                }
            }
            
            var (x, y) = pos;
            var up = new Vector2(x, y + 1);
            var right = new Vector2(x + 1, y);
            var down = new Vector2(x, y - 1);
            var left = new Vector2(x - 1, y);

            Vector2[] positions = {up, right, down, left};
            if (!positions.Select(nextPos => JunimoGreenhouse.GreenhouseBuildingAtPos(location, nextPos))
                .Any(greenhouseBuilding => greenhouseBuilding is not null)) return false;
            // BetterJunimos.SMonitor.Log("VisitGreenhouse IsActionAvailable: available", LogLevel.Debug);
            return true;
        }

        public bool PerformAction(GameLocation location, Vector2 pos, JunimoHarvester junimo, Guid guid) {
            // BetterJunimos.SMonitor.Log($"VisitGreenhouse #{junimo.whichJunimoFromThisHut}: PerformAction: begins",
            //     LogLevel.Debug);
            if (!IsActionAvailable(location, pos, guid)) {
                // BetterJunimos.SMonitor.Log($"VisitGreenhouse #{junimo.whichJunimoFromThisHut}: PerformAction: unavail",
                //     LogLevel.Debug);
                return false;
            }

            // BetterJunimos.SMonitor.Log($"VisitGreenhouse #{junimo.whichJunimoFromThisHut}: PerformAction: doing",
            //     LogLevel.Trace);

            if (Utility.isOnScreen(Utility.Vector2ToPoint(pos), 64, location)) {
                location.playSound("junimoMeep1");
            }

            // spawn a new Junimo in greenhouse
            var hut = Util.GetHutFromId(guid);
            var greenhouse = Game1.getLocationFromName("Greenhouse");

            var spawnAt = new Vector2(10 * 64 + 32, 21 * 64 + 32);
            // var spawnAt = new Vector2(14 * 64 + 32, 32 * 64 + 32);
            var rand = new Random();

            var junimoNumber = rand.Next(4, 100);
            Util.SpawnJunimoAtPosition(greenhouse, spawnAt, hut, junimoNumber);
            // BetterJunimos.SMonitor.Log(
            //     $"VisitGreenhouse PerformAction: #{junimoNumber} spawned in {greenhouse.Name} at {spawnAt.X} {spawnAt.Y}",
            //     LogLevel.Trace);

            // schedule this Junimo for despawn
            junimo.junimoReachedHut(junimo, junimo.currentLocation);
            return true;
        }

        public List<int> RequiredItems() {
            return new();
        }

        /* older API compat */
        public bool IsActionAvailable(Farm farm, Vector2 pos, Guid guid) {
            return IsActionAvailable((GameLocation) farm, pos, guid);
        }

        public bool PerformAction(Farm farm, Vector2 pos, JunimoHarvester junimo, Guid guid) {
            return PerformAction((GameLocation) farm, pos, junimo, guid);
        }
    }
}