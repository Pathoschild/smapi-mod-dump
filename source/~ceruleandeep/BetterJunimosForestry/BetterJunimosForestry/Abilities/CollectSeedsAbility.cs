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
using StardewValley.TerrainFeatures;
using StardewModdingAPI;
using SObject = StardewValley.Object;

// bits of this are from Tractor Mod; https://github.com/Pathoschild/StardewMods/blob/68628a40f992288278b724984c0ade200e6e4296/TractorMod/Framework/BaseAttachment.cs#L132

namespace BetterJunimosForestry.Abilities
{
    public class CollectSeedsAbility : IJunimoAbility
    {
        private readonly IMonitor Monitor;
        private readonly Axe FakeAxe = new();

        internal CollectSeedsAbility(IMonitor Monitor)
        {
            this.Monitor = Monitor;
            FakeAxe.UpgradeLevel = 1;
            FakeAxe.IsEfficient = true;
        }

        public string AbilityName()
        {
            return "CollectSeeds";
        }

        public bool IsActionAvailable(GameLocation location, Vector2 pos, Guid guid)
        {
            var mode = Util.GetModeForHut(Util.GetHutFromId(guid));
            return location.terrainFeatures.ContainsKey(pos) && IsHarvestableSeed(location.terrainFeatures[pos], mode);
        }

        public bool PerformAction(GameLocation location, Vector2 pos, JunimoHarvester junimo, Guid guid)
        {
            var mode = Util.GetModeForHut(Util.GetHutFromId(guid));
            if (!location.terrainFeatures.ContainsKey(pos) || !IsHarvestableSeed(location.terrainFeatures[pos], mode))
                return false;
            UseToolOnTile(FakeAxe, pos, Game1.player, Game1.currentLocation);
            return true;
        }

        private static bool IsHarvestableSeed(TerrainFeature t, string mode)
        {
            if (t is not Tree tree) return false;
            if (tree.growthStage.Value != 0) return false;
            if (mode == Modes.Normal) return false;
            if (mode == Modes.Forest && PlantTreesAbility.IsTileInPattern(t.currentTileLocation)) return false;
            return true;
        }
        
        private static void UseToolOnTile(Tool tool, Vector2 tile, Farmer player, GameLocation location)
        {
            var (x, y) = GetToolPixelPosition(tile);
            tool.DoFunction(location, (int) x, (int) y, 0, player);
        }

        private static Vector2 GetToolPixelPosition(Vector2 tile)
        {
            return tile * Game1.tileSize + new Vector2(Game1.tileSize / 2f);
        }

        public List<int> RequiredItems()
        {
            return new();
        }


        /* older API compat */
        public bool IsActionAvailable(Farm farm, Vector2 pos, Guid guid)
        {
            return IsActionAvailable((GameLocation) farm, pos, guid);
        }

        public bool PerformAction(Farm farm, Vector2 pos, JunimoHarvester junimo, Guid guid)
        {
            return PerformAction((GameLocation) farm, pos, junimo, guid);
        }
    }
}