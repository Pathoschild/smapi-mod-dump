/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

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
                        ore.MinutesUntilReady = 7;
                        break;
                    case "gem":
                        ore = new StardewValley.Object(tile, (RNG.Next(7) + 1) * 2, "Stone", true, true, false, false); //any of the possible gem rocks
                        ore.MinutesUntilReady = 5;
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
                        ore.MinutesUntilReady = 16;
                        break;
                    case "mystic":
                        ore = new StardewValley.Object(tile, 46, "Stone", true, true, false, false); //mystic stone, a.k.a. mystic ore
                        ore.MinutesUntilReady = 12;
                        break;
                    case "radioactive":
                        ore = new StardewValley.Object(tile, 95, "Stone", true, false, false, false);
                        ore.MinutesUntilReady = 25;
                        break;
                    case "diamond":
                        ore = new StardewValley.Object(tile, 2, "Stone", true, true, false, false);
                        ore.MinutesUntilReady = 5;
                        break;
                    case "ruby":
                        ore = new StardewValley.Object(tile, 4, "Stone", true, true, false, false);
                        ore.MinutesUntilReady = 5;
                        break;
                    case "jade":
                        ore = new StardewValley.Object(tile, 6, "Stone", true, true, false, false);
                        ore.MinutesUntilReady = 5;
                        break;
                    case "amethyst":
                        ore = new StardewValley.Object(tile, 8, "Stone", true, true, false, false);
                        ore.MinutesUntilReady = 5;
                        break;
                    case "topaz":
                        ore = new StardewValley.Object(tile, 10, "Stone", true, true, false, false);
                        ore.MinutesUntilReady = 5;
                        break;
                    case "emerald":
                        ore = new StardewValley.Object(tile, 12, "Stone", true, true, false, false);
                        ore.MinutesUntilReady = 5;
                        break;
                    case "aquamarine":
                        ore = new StardewValley.Object(tile, 14, "Stone", true, true, false, false);
                        ore.MinutesUntilReady = 5;
                        break;
                    default: break;
                }

                if (ore == null)
                {
                    Utility.Monitor.Log($"The ore to be spawned (\"{oreName}\") doesn't match any known ore types. Make sure that name isn't misspelled in your config file.", LogLevel.Info);
                    return null;
                }

                Utility.Monitor.VerboseLog($"Spawning ore. Type: {oreName}. Location: {tile.X},{tile.Y} ({location.Name}).");
                location.setObject(tile, ore); //actually spawn the ore object into the world
                return ore.ParentSheetIndex;
            }
        }
    }
}