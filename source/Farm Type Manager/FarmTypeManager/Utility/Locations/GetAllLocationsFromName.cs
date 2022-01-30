/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Creates a list of all game location names, including building interiors, matching the provided name.</summary>
            /// <param name="name">The name of the location(s) to be listed. Case-insensitive.</param>
            /// <returns>A list of all locations with a <see cref="GameLocation.NameOrUniqueName"/> matching the provided name.</returns>
            public static List<string> GetAllLocationsFromName(string name)
            {
                //NOTE: Do not "preload" mine levels; they will instantiate and spawn things, advance elevator progress, etc. This might also apply to volcano levels.

                if (name.StartsWith("UndergroundMine", StringComparison.OrdinalIgnoreCase) //if the name is a mine level
                    || name.StartsWith("VolcanoDungeon", StringComparison.OrdinalIgnoreCase) //if the name is a volcano level
                    || (Game1.getLocationFromName(name) != null)) //OR if the name is a typical, easily retrieved location 
                {
                    return new List<string>() { name }; //return a list containing the name
                }

                List<string> locations = new List<string>(); //create a blank list

                foreach (BuildableGameLocation buildable in Game1.locations.OfType<BuildableGameLocation>()) //for each buildable location in the game
                {
                    foreach (Building building in buildable.buildings.Where(building => building.indoors.Value != null)) //for each building with an interior location ("indoors")
                    {
                        if (building.indoors.Value.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) //if the location's name matches the provided name
                        {
                            locations.Add(building.indoors.Value.NameOrUniqueName); //add the location to the list
                        }
                    }
                }

                if (locations.Count == 0) //if locations is still empty
                {
                    //check for TMXLoader buildable locations
                    if (Type.GetType("TMXLoader.TMXLoaderMod, TMXLoader") is Type tmx) //if TMXLoader can be accessed
                    {
                        if (tmx.GetField("buildablesBuild", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) is IList tmxSaveBuildables) //if tmx's SaveBuildables list can be accessed
                        {
                            foreach (object sb in tmxSaveBuildables) //for each saved buildable in TMXLoader
                            {
                                if (sb.GetType() is Type sbType && sbType.GetProperty("UniqueId").GetValue(sb) is string UniqueId && sbType.GetProperty("Id").GetValue(sb) is string Id) //if this buildable's UniqueID and ID can be accessed
                                {
                                    string mapName = "BuildableIndoors-" + UniqueId; //construct the GameLocation.Name used for this buildable's interior location
                                    if (name == Id && Game1.getLocationFromName(mapName) is GameLocation indoors) //if the provided name equals this buildable's ID AND the interior location exists
                                    {
                                        locations.Add(mapName); //add this location to the list
                                    }
                                }
                            }
                        }
                    }
                }

                return locations;
            }
        }
    }
}