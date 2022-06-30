/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ConfigurableBundleCosts;

internal class BundleManager
{
    [EventPriority(EventPriority.Low)]
    internal static void CheckBundleData()
    {
        try
        {
            // dump and reload the asset to force AssetHandler.BundleData to update
            Globals.GameContent.InvalidateCache("Data/Bundles");
            Globals.GameContent.Load<Dictionary<string, string>>("Data/Bundles");
            Game1.netWorldState?.Value?.SetBundleData(AssetHandler.BundleData);
        }
        catch (Exception ex)
        {
            Log.Error($"Exception encountered while updating bundle data: {ex}");
        }
    }
}
