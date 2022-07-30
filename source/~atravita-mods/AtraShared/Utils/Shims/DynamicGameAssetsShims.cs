/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit.Reflection;
using HarmonyLib;

namespace AtraShared.Utils.Shims;

/// <summary>
/// Shims for DGA.
/// </summary>
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Preference.")]
public class DynamicGameAssetsShims
{
    /// <summary>
    /// Gets whether or not something is a DGA giant crop.
    /// </summary>
    public static Func<object, bool>? IsDGAGiantCrop => isDGAGiantCrop.Value;

    private static Lazy<Func<object, bool>?> isDGAGiantCrop = new(() =>
    {
        var type = AccessTools.TypeByName("DynamicGameAssets.Game.CustomGiantCrop");
        return type?.GetTypeIs();
    });
}
