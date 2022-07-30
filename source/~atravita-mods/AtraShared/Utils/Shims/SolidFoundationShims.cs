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
/// Shims for Solid Foundations.
/// </summary>
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Preference.")]
public static class SolidFoundationShims
{
    /// <summary>
    /// Gets whether or not the object is an SF building.
    /// </summary>
    public static Func<object, bool>? IsSFBuilding => isSFBuilding.Value;

    private static readonly Lazy<Func<object, bool>?> isSFBuilding = new(() =>
    {
        Type sFBuilding = AccessTools.TypeByName("SolidFoundations.Framework.Models.ContentPack.GenericBuilding");
        return sFBuilding?.GetTypeIs();
    });
}
