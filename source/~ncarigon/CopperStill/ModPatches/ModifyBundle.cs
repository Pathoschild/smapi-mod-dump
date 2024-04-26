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
        public static void Register() {
            if (ModEntry.Instance?.Helper is not null) {
                ModEntry.Instance.Helper.Events.GameLoop.SaveLoaded += (s, e) => UpdateBundle(Config.Instance?.ModifyDefaultBundle ?? false);
            }
        }

        public static void UpdateBundle(bool include) {
            var id = Game1.objectData
                    .Where(o => o.Value.Name.Contains("Brandy", System.StringComparison.OrdinalIgnoreCase))
                    .Select(o => ItemRegistry.GetMetadata(o.Key).QualifiedItemId).FirstOrDefault();
            var bundles = Game1.netWorldState.Value.BundleData;
            foreach (var key in bundles.Keys.ToArray()) {
                if (key == "Abandoned Joja Mart/36") {
                    var val = bundles[key];
                    if (val == "The Missing//348 1 1 807 1 0 74 1 0 454 5 2 795 1 2 445 1 0/1/5//The Missing") { // original bundle
                        if (include) {
                            ModEntry.Instance?.Monitor?.Log("Found default bundle, updating to current.", LogLevel.Info);
                            bundles[key] = val.Replace("//348 1 1 ", $"//{id} 1 1 ");
                        } else {
                            ModEntry.Instance?.Monitor?.Log("Found default bundle, nothing to do.", LogLevel.Info);
                        }
                    } else if (val == "The Missing//(O)Brandy 1 1 807 1 0 74 1 0 454 5 2 795 1 2 445 1 0/1/5//The Missing" // older item IDs
                        || val == "The Missing//(O)NCarigon.CopperStillJA_Brandy 1 1 807 1 0 74 1 0 454 5 2 795 1 2 445 1 0/1/5//The Missing"
                    ) {
                        if (include) {
                            ModEntry.Instance?.Monitor?.Log("Found legacy bundle, updating to current.", LogLevel.Info);
                            bundles[key] = val.Replace("//(O)Brandy 1 1 ", $"//{id} 1 1 ")
                                .Replace("//(O)NCarigon.CopperStillJA_Brandy 1 1 ", $"//{id} 1 1 ");
                        } else {
                            ModEntry.Instance?.Monitor?.Log("Found legacy bundle, resetting to default.", LogLevel.Info);
                            bundles[key] = val.Replace("//(O)Brandy 1 1 ", $"//348 1 1 ")
                                .Replace("//(O)NCarigon.CopperStillJA_Brandy 1 1 ", $"//348 1 1 ");
                        }
                    } else if (val == $"The Missing//{id} 1 1 807 1 0 74 1 0 454 5 2 795 1 2 445 1 0/1/5//The Missing") { // current item ID
                        if (include) {
                            ModEntry.Instance?.Monitor?.Log("Found updated bundle, nothing to do.", LogLevel.Info);
                        } else {
                            ModEntry.Instance?.Monitor?.Log("Found updated bundle, resetting to default.", LogLevel.Info);
                            bundles[key] = val.Replace($"//{id} 1 1 ", $"//348 1 1 ");
                        }
                    }
                    if (val != bundles[key]) {
                        Game1.netWorldState.Value.SetBundleData(bundles);
                    }
                    break;
                }
            }
        }
    }
}
