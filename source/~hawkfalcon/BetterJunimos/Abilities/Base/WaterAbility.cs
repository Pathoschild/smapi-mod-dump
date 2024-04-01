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
using BetterJunimos.Utils;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using StardewModdingAPI;

namespace BetterJunimos.Abilities {
    public class WaterAbility : IJunimoAbility {
        public string AbilityName() {
            return "Water";
        }
        
        public bool IsActionAvailable(GameLocation location, Vector2 pos, Guid guid) {
            if (!location.terrainFeatures.ContainsKey(pos)) return false;
            if (location.terrainFeatures[pos] is not HoeDirt hd) return false;
            if (hd.state.Value == HoeDirt.watered) return false;
            if (hd.crop == null) return false;
            return true;
        }

        public bool PerformAction(GameLocation location, Vector2 pos, JunimoHarvester junimo, Guid guid) {
            if (!location.terrainFeatures.ContainsKey(pos) || location.terrainFeatures[pos] is not HoeDirt hd) {
                return false;
            }

            hd.state.Value = HoeDirt.watered;
            if (!Utility.isOnScreen(Utility.Vector2ToPoint(pos), 64, location)) return true;
            var multiplayer = Util.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
            multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(13,
                new Vector2(pos.X * 64f, pos.Y * 64f), Color.White, 10, Game1.random.NextDouble() < 0.5, 70f, 0, 64,
                (float) ((pos.Y * 64.0 + 32.0) / 10000.0 - 0.00999999977648258)));

            return true;
        }

        public List<string> RequiredItems() {
            return new List<string>();
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