/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System.Collections.Generic;
using xTile.Layers;

namespace DynamicMapTiles
{
    public interface IDynamicMapTilesApi
    {
        public bool TriggerActions(List<Layer> layers, Farmer farmer, Point tilePos, List<string> suffixes);
    }
    public class DynamicMapTilesApi : IDynamicMapTilesApi
    {
        public bool TriggerActions(List<Layer> layers, Farmer farmer, Point tilePos, List<string> suffixes)
        {
            return ModEntry.TriggerActions(layers, farmer, tilePos, suffixes);
        }
    }
}