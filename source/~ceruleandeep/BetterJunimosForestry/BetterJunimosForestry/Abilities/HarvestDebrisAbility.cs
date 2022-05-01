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
using BetterJunimos.Abilities;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Objects;
using StardewValley.Tools;
using StardewModdingAPI;
using SObject = StardewValley.Object;

// bits of this are from Tractor Mod; https://github.com/Pathoschild/StardewMods/blob/68628a40f992288278b724984c0ade200e6e4296/TractorMod/Framework/BaseAttachment.cs#L132

namespace BetterJunimosForestry.Abilities {
    public class HarvestDebrisAbility : IJunimoAbility {

        private readonly IMonitor Monitor;
        private readonly FakeFarmer FakeFarmer = new();
        private readonly Pickaxe FakePickaxe = new();
        private readonly Axe FakeAxe = new();
        private readonly MeleeWeapon Scythe = new(47);

        internal HarvestDebrisAbility(IMonitor Monitor) {
            this.Monitor = Monitor;
            FakeAxe.IsEfficient = true;
            FakePickaxe.IsEfficient = true;
            Scythe.IsEfficient = true;
        }

        public string AbilityName() {
            return "HarvestDebris";
        }

        private bool IsDebris(SObject so) {
            var debris = IsTwig(so) || IsWeed(so) || IsStone(so);
            return debris;
        }

        private static bool IsTwig(SObject obj) {
            return obj?.ParentSheetIndex is 294 or 295;
        }

        private static bool IsWeed(SObject obj) {
            return obj is not Chest && obj?.Name == "Weeds";
        }

        private static bool IsStone(SObject obj) {
            return obj is not Chest && obj?.Name == "Stone";
        }

        public bool IsActionAvailable(GameLocation location, Vector2 pos, Guid guid) {
            var (x, y) = pos;
            var up = new Vector2(x, y + 1);
            var right = new Vector2(x + 1, y);
            var down = new Vector2(x, y - 1);
            var left = new Vector2(x - 1, y);

            Vector2[] positions = { up, right, down, left };
            foreach (var nextPos in positions) {
                if (!Util.IsWithinRadius(location, Util.GetHutFromId(guid), nextPos)) continue;
                if (location.objects.ContainsKey(nextPos) && IsDebris(location.objects[nextPos])) {
                    return true;
                }
            }
            return false;
        }

        public bool PerformAction(GameLocation location, Vector2 pos, JunimoHarvester junimo, Guid guid) {
            var (x, y) = pos;
            var up = new Vector2(x, y + 1);
            var right = new Vector2(x + 1, y);
            var down = new Vector2(x, y - 1);
            var left = new Vector2(x - 1, y);

            var direction = 0;
            Vector2[] positions = { up, right, down, left };
            foreach (var nextPos in positions) {
                if (!Util.IsWithinRadius(location, Util.GetHutFromId(guid), nextPos)) continue;
                if (location.objects.ContainsKey(nextPos) && IsDebris(location.objects[nextPos])) {

                    junimo.faceDirection(direction);
                    // SetForageQuality(location, nextPos);

                    var item = location.objects[nextPos];

                    if (IsStone(item)) {
                        UseToolOnTile(FakePickaxe, nextPos, location);
                    }

                    if (IsTwig(item)) {
                        UseToolOnTile(FakeAxe, nextPos, location);
                    }

                    if (IsWeed(item)) {
                        UseToolOnTile(Scythe, nextPos, location);
                        item.performToolAction(Scythe, location);
                        location.removeObject(nextPos, false);
                    }
                    
                    return true;
                }
                direction++;
            }

            return false;
        }

        private bool UseToolOnTile(Tool t, Vector2 tile, GameLocation location) {
            FakeFarmer.currentLocation = location;

            // use tool on center of tile
            var (x, y) = GetToolPixelPosition(tile);
            
            // just before we get going
            if (t is null) Monitor.Log($"t is null", LogLevel.Warn);
            if (FakeFarmer.currentLocation.debris is null) Monitor.Log($"FakeFarmer.currentLocation.debris is null", LogLevel.Warn);

            t?.DoFunction(location, (int) x, (int) y, 0, FakeFarmer);
            return true;
        }

        private static Vector2 GetToolPixelPosition(Vector2 tile) {
            return tile * Game1.tileSize + new Vector2(Game1.tileSize / 2f);
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