using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using Newtonsoft.Json;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Finds and returns the matching TMXLoader buildable's "ID" for a given location name.</summary>
            /// <param name="locationName">The GameLocation.Name to be checked.</param>
            /// <returns>The non-unique "ID" value for the matching TMXLoader buildable type. Null if none was found.</returns>
            public static string GetTMXBuildableName(string locationName)
            {
                string tmxPrefix = "BuildableIndoors-"; //the prefix currently used by TMXLoader in buildable indoor location names
                
                if (!locationName.StartsWith(tmxPrefix)) //if the location name doesn't start with the TMX prefix
                {
                    return null; //return null without checking TMX's data
                }

                if (Type.GetType("TMXLoader.TMXLoaderMod, TMXLoader") is Type tmx) //if TMXLoader can be accessed
                {
                    if (tmx.GetField("buildablesBuild", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) is IList tmxSaveBuildables) //if tmx's SaveBuildables list can be accessed
                    {
                        foreach (object sb in tmxSaveBuildables) //for each saved buildable in TMXLoader
                        {
                            if (sb.GetType() is Type sbType && sbType.GetProperty("UniqueId").GetValue(sb) is string UniqueId && sbType.GetProperty("Id").GetValue(sb) is string Id) //if this buildable's UniqueID and ID can be accessed
                            {
                                string locationUniqueId = locationName.Substring(tmxPrefix.Length); //get the location's unique ID by removing the TMX prefix
                                if (locationUniqueId == UniqueId) //if the location's unique ID matches this saved buildable
                                {
                                    return Id; //return this saved buildable's non-unique ID
                                }
                            }
                        }
                    }
                }

                return null; //no matching name was detected, so return null
            }
        }
    }
}