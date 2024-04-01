/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

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
                StardewValley.Object ore = null; //the ore object to spawn
                switch (oreName.ToLower()) //avoid any casing issues in method calls by making this lower-case
                {
                    case "stone":
                        ore = new Object((668 + RNG.Next(2) * 2).ToString(), 1); //either of the two random stones spawned in the vanilla hilltop quarry
                        break;
                    case "geode":
                        ore = new Object("75", 1);
                        break;
                    case "frozengeode":
                        ore = new Object("76", 1);
                        break;
                    case "magmageode":
                        ore = new Object("77", 1);
                        break;
                    case "omnigeode":
                        ore = new Object("819", 1);
                        break;
                    case "gem":
                        ore = new Object(((RNG.Next(7) + 1) * 2).ToString(), 1); //any of the specific gem nodes (NOT the gem node with ID 44, which was considered excessively high-reward)
                        break;
                    case "copper":
                        ore = new Object("751", 1);
                        break;
                    case "iron":
                        ore = new Object("290", 1);
                        break;
                    case "gold":
                        ore = new Object("764", 1);
                        break;
                    case "iridium":
                        ore = new Object("765", 1);
                        break;
                    case "mystic":
                        ore = new Object("46", 1);
                        break;
                    case "radioactive":
                        ore = new Object("95", 1);
                        break;
                    case "diamond":
                        ore = new Object("2", 1);
                        break;
                    case "ruby":
                        ore = new Object("4", 1);
                        break;
                    case "jade":
                        ore = new Object("6", 1);
                        break;
                    case "amethyst":
                        ore = new Object("8", 1);
                        break;
                    case "topaz":
                        ore = new Object("10", 1);
                        break;
                    case "emerald":
                        ore = new Object("12", 1);
                        break;
                    case "aquamarine":
                        ore = new Object("14", 1);
                        break;
                    case "mussel":
                        ore = new Object("25", 1);
                        break;
                    case "fossil":
                        ore = new Object((816 + RNG.Next(2)).ToString(), 1); //either of the two random fossil nodes
                        break;
                    case "clay":
                        ore = new Object("818", 1);
                        break;
                    case "cindershard":
                        ore = new Object((843 + RNG.Next(2)).ToString(), 1);
                        break;
                    case "coal":
                        ore = new Object("BasicCoalNode" + RNG.Next(2).ToString(), 1);
                        break;
                    default: break;
                }

                if (ore == null)
                {
                    Utility.Monitor.Log($"The ore to be spawned (\"{oreName}\") doesn't match any known ore types. Make sure that name isn't misspelled in your config file.", LogLevel.Info);
                    return null;
                }

                int? durability = GetDefaultDurability(ore.ItemId); //try to get this ore type's default durability
                if (durability.HasValue) //if a default exists
                    ore.MinutesUntilReady = durability.Value; //use it

                Utility.Monitor.VerboseLog($"Spawning ore. Type: {oreName}. Location: {tile.X},{tile.Y} ({location.Name}).");
                location.objects.TryAdd(tile, ore); //actually spawn the ore object into the world
                return ore.ParentSheetIndex;
            }
        }
    }
}