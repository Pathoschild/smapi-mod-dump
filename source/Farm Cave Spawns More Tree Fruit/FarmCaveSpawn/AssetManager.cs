/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/FarmCaveSpawn
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace FarmCaveSpawn;

internal class AssetManager : IAssetLoader
{
    public readonly string denylistLocation = PathUtilities.NormalizeAssetName("Mods/atravita_FarmCaveSpawn_denylist");
    public readonly string additionalLocationsLocation = PathUtilities.NormalizeAssetName("Mods/atravita_FarmCaveSpawn_additionalLocations");

    public bool CanLoad<T>(IAssetInfo asset)
    {
        return asset.AssetNameEquals(denylistLocation) || asset.AssetNameEquals(additionalLocationsLocation);
    }

    /// <summary>
    /// Load initial blank denylist for other mods to edit later,
    /// Load initial additional areas list with SVE areas included
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="asset"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public T Load<T>(IAssetInfo asset)
    {
        if (asset.AssetNameEquals(denylistLocation))
        {
            return (T)(object)new Dictionary<string, string>
            {
            };
        }
        else if (asset.AssetNameEquals(additionalLocationsLocation))
        {
            return (T)(object)new Dictionary<string, string>
            {
                ["FlashShifter.SVECode"] = "Custom_MinecartCave, Custom_DeepCave",
            };
        }
        throw new InvalidOperationException($"Should not have tried to load '{asset.AssetName}'.");
    }
}
