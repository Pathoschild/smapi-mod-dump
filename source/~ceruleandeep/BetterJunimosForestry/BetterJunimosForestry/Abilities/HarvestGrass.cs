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
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using BetterJunimos.Abilities;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace BetterJunimosForestry.Abilities {
    public class HarvestGrassAbility : IJunimoAbility {
        public string AbilityName() {
            return "HarvestGrass";
        }

        public bool IsActionAvailable(GameLocation farm, Vector2 pos, Guid guid)
        {
            if (!ModEntry.Config.HarvestGrassEnabled) return false;
            return farm.terrainFeatures.ContainsKey(pos) && farm.terrainFeatures[pos] is Grass;
        }

        public bool PerformAction(GameLocation farm, Vector2 pos, JunimoHarvester junimo, Guid id) {
            if (!ModEntry.Config.HarvestGrassEnabled) return false;
            if (farm.terrainFeatures.ContainsKey(pos) && farm.terrainFeatures[pos] is Grass g) {
                return TryHarvestGrass(g, farm, pos);
            }
            return false;
        }

        // https://github.com/Pathoschild/StardewMods/blob/68628a40f992288278b724984c0ade200e6e4296/TractorMod/Framework/BaseAttachment.cs#L373

        /// <summary>Try to harvest tall grass.</summary>
        /// <param name="grass">The grass to harvest.</param>
        /// <param name="location">The location being harvested.</param>
        /// <param name="tile">The tile being harvested.</param>
        /// <returns>Returns whether it was harvested.</returns>
        protected bool TryHarvestGrass(Grass grass, GameLocation location, Vector2 tile) {
            if (grass == null) return false;

            // remove grass
            location.terrainFeatures.Remove(tile);

            // collect hay
            if (Game1.random.NextDouble() < 0.5) {
                Game1.getFarm().tryToAddHay(1);
            }

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