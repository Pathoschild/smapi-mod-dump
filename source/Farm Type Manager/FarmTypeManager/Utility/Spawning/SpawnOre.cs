using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Generates ore and places it on the specified map and tile.</summary>
            /// <param name="oreName">A string representing the name of the ore type to be spawned, e.g. "stone"</param>
            /// <param name="location">The GameLocation where the ore should be spawned.</param>
            /// <param name="tile">The x/y coordinates of the tile where the ore should be spawned.</param>
            /// <returns>The spawned ore's parentSheetIndex. Null if spawn failed.</returns>
            public static int? SpawnOre(string oreName, GameLocation location, Vector2 tile)
            {
                StardewValley.Object ore = null; //ore object, to be spawned into the world later
                switch (oreName.ToLower()) //avoid any casing issues in method calls by making this lower-case
                {
                    case "stone":
                        ore = new StardewValley.Object(tile, 668 + (RNG.Next(2) * 2), 1); //either of the two random stones spawned in the vanilla hilltop quarry
                        ore.MinutesUntilReady = 2; //durability, i.e. number of hits with basic pickaxe required to break the ore (each pickaxe level being +1 damage)
                        break;
                    case "geode":
                        ore = new StardewValley.Object(tile, 75, 1); //"regular" geode rock, as spawned on vanilla hilltop quarries 
                        ore.MinutesUntilReady = 3;
                        break;
                    case "frozengeode":
                        ore = new StardewValley.Object(tile, 76, 1); //frozen geode rock
                        ore.MinutesUntilReady = 5;
                        break;
                    case "magmageode":
                        ore = new StardewValley.Object(tile, 77, 1); //magma geode rock
                        ore.MinutesUntilReady = 8; //TODO: replace this guess w/ actual vanilla durability
                        break;
                    case "gem":
                        ore = new StardewValley.Object(tile, (RNG.Next(7) + 1) * 2, "Stone", true, false, false, false); //any of the possible gem rocks
                        ore.MinutesUntilReady = 5; //based on "gemstone" durability, but applies to every type for simplicity's sake
                        break;
                    case "copper":
                        ore = new StardewValley.Object(tile, 751, 1); //copper ore
                        ore.MinutesUntilReady = 3;
                        break;
                    case "iron":
                        ore = new StardewValley.Object(tile, 290, 1); //iron ore
                        ore.MinutesUntilReady = 4;
                        break;
                    case "gold":
                        ore = new StardewValley.Object(tile, 764, 1); //gold ore
                        ore.MinutesUntilReady = 8;
                        break;
                    case "iridium":
                        ore = new StardewValley.Object(tile, 765, 1); //iridium ore
                        ore.MinutesUntilReady = 16; //TODO: confirm this is still the case (it's based on SDV 1.11 code)
                        break;
                    case "mystic":
                        ore = new StardewValley.Object(tile, 46, "Stone", true, false, false, false); //mystic ore, i.e. high-end cavern rock with iridium + gold
                        ore.MinutesUntilReady = 16; //TODO: replace this guess w/ actual vanilla durability
                        break;
                    default: break;
                }

                if (ore != null)
                {
                    Utility.Monitor.Log($"Spawning ore. Type: {oreName}. Location: {tile.X},{tile.Y} ({location.Name}).", LogLevel.Trace);
                    location.setObject(tile, ore); //actually spawn the ore object into the world
                    return ore.ParentSheetIndex;
                }
                else
                {
                    Utility.Monitor.Log($"The ore to be spawned (\"{oreName}\") doesn't match any known ore types. Make sure that name isn't misspelled in your player config file.", LogLevel.Info);
                    return null;
                }
            }
        }
    }
}