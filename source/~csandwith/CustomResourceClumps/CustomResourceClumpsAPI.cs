/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Linq;

namespace CustomResourceClumps
{
    public class CustomResourceClumpsAPI
    {
        public ResourceClump GetCustomClump(string id, Vector2 tile)
        {
            var clump = ModEntry.customClumps.Find(n => n.id == id);
            if (clump == null)
                return null;
            return new ResourceClump(clump.index, clump.tileWidth, clump.tileHeight, tile);
        }
        public bool TryPlaceClump(GameLocation location, string id, Vector2 tile)
        {
            if (location == null)
                return false;
            var clump = ModEntry.customClumps.Find(n => n.id == id);
            if (clump == null)
                return false;
            location.resourceClumps.Add(new ResourceClump(clump.index, clump.tileWidth, clump.tileHeight, tile));
            return true;
        }
        public List<object> GetCustomClumpData()
        {
            return new List<object>(ModEntry.customClumps);
        }
        public List<string> GetCustomClumpIDs()
        {
            return ModEntry.customClumps.Select(n => n.id).ToList();
        }
    }
}