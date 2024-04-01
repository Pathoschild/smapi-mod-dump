/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using System.Linq;
using StardewModdingAPI;
using StardewValley;

namespace CopperStill.ModPatches {
    internal static class ModifyBundle {
        public static void Register(IModHelper helper, IMonitor monitor) {
            helper.Events.GameLoop.SaveLoaded += (s, e) => {
                var id = Game1.objectData
                    .Where(o => o.Value.Name.Contains("Brandy", System.StringComparison.OrdinalIgnoreCase))
                    .Select(o => ItemRegistry.GetMetadata(o.Key).QualifiedItemId).FirstOrDefault();
                if (id != null) {
                    var bundles = Game1.netWorldState.Value.BundleData;
                    foreach (var key in bundles.Keys.ToArray()) {
                        var val = bundles[key];
                        if (key == "Abandoned Joja Mart/36" && val == "The Missing//348 1 1 807 1 0 74 1 0 454 5 2 795 1 2 445 1 0/1/5//The Missing") {
                            bundles[key] = val.Replace("//348 1 1 ", $"//{id} 1 1 ");
                            Game1.netWorldState.Value.SetBundleData(bundles);
                            monitor.Log("Found default bundle, adjusting accordingly.", LogLevel.Info);
                            break;
                        }
                    }
                }
            };
        }
    }
}
