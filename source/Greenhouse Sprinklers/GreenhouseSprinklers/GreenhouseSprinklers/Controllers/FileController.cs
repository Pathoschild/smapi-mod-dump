/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bpendragon/GreenhouseSprinklers
**
*************************************************/

using System;
using Bpendragon.GreenhouseSprinklers.Data;
using StardewModdingAPI;
using StardewModdingAPI.Events;

using System.Linq;
using StardewValley;
using StardewValley.Buildings;

namespace Bpendragon.GreenhouseSprinklers
{
    partial class ModEntry
    {
        internal void OnLoad(object sender, SaveLoadedEventArgs e)
        {
            if (Config.ShowVisualUpgrades)
            {
                Monitor.Log("Invalidating Texture Cache at first Load");
                Helper.Content.InvalidateCache("Buildings/Greenhouse");
            }//invalidate the cache on load, forcing load of new sprite if applicable.
        }

        internal void OnSaveCompleted(object sender, SavedEventArgs e)
        {
            if (Config.ShowVisualUpgrades)
            {
                Monitor.Log("Invalidating Texture Cache after save");
                Helper.Content.InvalidateCache("Buildings/Greenhouse");
            }//invalidate the cache each night, forcing load of new sprite if applicable.
        }

    }
}
