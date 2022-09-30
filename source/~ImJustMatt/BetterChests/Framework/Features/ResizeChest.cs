/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.Features;

using HarmonyLib;
using StardewMods.Common.Enums;
using StardewMods.CommonHarmony.Enums;
using StardewMods.CommonHarmony.Helpers;
using StardewMods.CommonHarmony.Models;
using StardewValley.Objects;

/// <summary>
///     Expand the capacity of chests and add scrolling to access extra items.
/// </summary>
internal sealed class ResizeChest : IFeature
{
    private const string Id = "furyx639.BetterChests/ResizeChest";

#nullable disable
    private static IFeature Instance;
#nullable enable

    private bool _isActivated;

    private ResizeChest()
    {
        HarmonyHelper.AddPatches(
            ResizeChest.Id,
            new SavedPatch[]
            {
                new(
                    AccessTools.Method(typeof(Chest), nameof(Chest.GetActualCapacity)),
                    typeof(ResizeChest),
                    nameof(ResizeChest.Chest_GetActualCapacity_postfix),
                    PatchType.Postfix),
            });
    }

    /// <summary>
    ///     Initializes <see cref="ResizeChest" />.
    /// </summary>
    /// <returns>Returns an instance of the <see cref="ResizeChest" /> class.</returns>
    public static IFeature Init()
    {
        return ResizeChest.Instance ??= new ResizeChest();
    }

    /// <inheritdoc />
    public void Activate()
    {
        if (this._isActivated)
        {
            return;
        }

        this._isActivated = true;
        HarmonyHelper.ApplyPatches(ResizeChest.Id);
    }

    /// <inheritdoc />
    public void Deactivate()
    {
        if (!this._isActivated)
        {
            return;
        }

        this._isActivated = false;
        HarmonyHelper.UnapplyPatches(ResizeChest.Id);
    }

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Harmony")]
    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Harmony")]
    [SuppressMessage("StyleCop", "SA1313", Justification = "Harmony")]
    private static void Chest_GetActualCapacity_postfix(Chest __instance, ref int __result)
    {
        if (!Storages.TryGetOne(__instance, out var storage)
         || storage.ResizeChest is not FeatureOption.Enabled
         || storage.ResizeChestCapacity == 0)
        {
            return;
        }

        __result = storage.ActualCapacity;
    }
}