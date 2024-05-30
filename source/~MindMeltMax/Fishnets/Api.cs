/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Fishnets.Data;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewValley;
using System.Xml.Linq;

namespace Fishnets
{
    public interface IApi
    {
        /// <summary>
        /// Remove a fish from being able to be caught by a fish net
        /// </summary>
        /// <param name="name">The name of the fish to exclude</param>
        /// <returns>True if the fish was excluded, false if it wasn't</returns>
        bool AddExclusion(string name);

        /// <summary>
        /// Remove a fish from the list of exclusions, allowing it to be caught by the fish net
        /// </summary>
        /// <param name="name">The name of the fish to remove</param>
        /// <returns>True if the fish was removed from the exclusions, false if it was not found</returns>
        bool RemoveExclusion(string name);

        /// <summary>
        /// Get the <see cref="Item.ItemId"/> of the fish net object
        /// </summary>
        string GetId();

        /// <summary>
        /// Try to obtain some useful information about a fish net object in a specified location at a specified tile
        /// </summary>
        /// <param name="location">The location where the object should be</param>
        /// <param name="tile">The tile at which the object should be</param>
        /// <param name="data">A collection of data retrieved if the object was found</param>
        /// <returns>true if the object was found and data was collected, otherwise false</returns>
        bool TryGetFishNetInfoAt(GameLocation location, Vector2 tile, out Dictionary<string, string> data);

        /// <summary>
        /// Add a QualifiedItemId for a custom sea weed / algae variant, to be used when <see cref="Config.LessWeeds"/> is toggled on
        /// </summary>
        /// <param name="id">The QualifiedItemId of the sea weed / algae</param>
        /// <returns>True if the id was succesfully added, false if it wasn't</returns>
        bool AddWeedsId(string id);

        /// <summary>
        /// Remove a QualifiedItemId for any sea weed / algae variant
        /// </summary>
        /// <param name="id">The QualifiedItemId of the sea weed / algae</param>
        /// <returns>True if the id was succesfully removed, false if it wasn't</returns>
        bool RemoveWeedsId(string id);

        /// <summary>
        /// Add a QualifiedItemId for a custom jelly variant, to be used when <see cref="Config.LessJelly"/> is toggled on
        /// </summary>
        /// <param name="id">The QualifiedItemId of the jelly</param>
        /// <returns>True if the id was succesfully added, false if it wasn't</returns>
        bool AddJellyId(string id);

        /// <summary>
        /// Remove a QualifiedItemId for any jelly variant
        /// </summary>
        /// <param name="id">The QualifiedItemId of the jelly</param>
        /// <returns>True if the id was succesfully removed, false if it wasn't</returns>
        bool RemoveJellyId(string id);
    }

    public class Api : IApi
    {
        public bool AddExclusion(string name)
        {
            if (Statics.ExcludedFish.Contains(name))
            {
                ModEntry.IMonitor.Log($"{name} has already been excluded from the fishnet drop list");
                return false;
            }
            ModEntry.IMonitor.Log($"Excluded {name} from fishnet drop list");
            Statics.ExcludedFish.Add(name);
            ModEntry.IHelper.Data.WriteJsonFile($"assets/excludedfish.json", Statics.ExcludedFish);
            return true;
        }

        public bool RemoveExclusion(string name)
        {
            if (!Statics.ExcludedFish.Contains(name))
            {
                ModEntry.IMonitor.Log($"{name} is not excluded from the fishnet drop list");
                return false;
            }
            ModEntry.IMonitor.Log($"Removed exclusion for {name} from fishnet drop list");
            Statics.ExcludedFish.Remove(name);
            ModEntry.IHelper.Data.WriteJsonFile($"assets/excludedfish.json", Statics.ExcludedFish);
            return true;
        }

        public string GetId() => ModEntry.ObjectInfo.Id;

        public bool TryGetFishNetInfoAt(GameLocation location, Vector2 tile, out Dictionary<string, string> data)
        {
            data = [];
            if (location.Objects.TryGetValue(tile, out Object obj) && obj.ItemId == ModEntry.ObjectInfo.Id && Statics.TryParseModData(obj, out var baitData))
            {
                data = new()
                {
                    { "heldObject", obj.heldObject.Value.QualifiedItemId },
                    { "heldObjectStack", obj.heldObject.Value.Stack.ToString() },
                    { "heldObjectQuality", obj.heldObject.Value.Quality.ToString() },
                    { "bait", baitData.Key },
                    { "baitQuality", baitData.Value.ToString() },
                    { "owner", obj.owner.Value.ToString() }
                };
                return true;
            }
            ModEntry.IMonitor.Log($"Unable to find fish net object in {location.Name} at tile {tile}");
            return false;
        }

        public bool AddWeedsId(string id)
        {
            if (Statics.WeedsIds.Contains(id))
            {
                ModEntry.IMonitor.Log($"{id} has already been added to the weeds list");
                return false;
            }
            ModEntry.IMonitor.Log($"Added {id} to the weeds list");
            Statics.WeedsIds.Add(id);
            return true;
        }

        public bool RemoveWeedsId(string id)
        {
            if (Statics.WeedsIds.Contains(id))
            {
                ModEntry.IMonitor.Log($"{id} was not in the weeds list");
                return false;
            }
            ModEntry.IMonitor.Log($"Removed {id} from the weeds list");
            Statics.WeedsIds.Add(id);
            return true;
        }

        public bool AddJellyId(string id)
        {
            if (Statics.JellyIds.Contains(id))
            {
                ModEntry.IMonitor.Log($"{id} has already been added to the jellies list");
                return false;
            }
            ModEntry.IMonitor.Log($"Added {id} to the jellies list");
            Statics.JellyIds.Add(id);
            return true;
        }

        public bool RemoveJellyId(string id)
        {
            if (!Statics.JellyIds.Contains(id))
            {
                ModEntry.IMonitor.Log($"{id} was not in the jellies list");
                return false;
            }
            ModEntry.IMonitor.Log($"Removed {id} from the jellies list");
            Statics.JellyIds.Add(id);
            return true;
        }
    }
}
