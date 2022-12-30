/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.Features;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewMods.BetterChests.Framework.Models;
using StardewMods.BetterChests.Framework.StorageObjects;
using StardewMods.Common.Enums;
using StardewValley.Objects;

/// <summary>
///     Debris such as mined or farmed items can be collected into a Chest in the farmer's inventory.
/// </summary>
internal sealed class CollectItems : Feature
{
    private const string Id = "furyx639.BetterChests/CollectItems";

    private static readonly MethodBase DebrisCollect = AccessTools.Method(typeof(Debris), nameof(Debris.collect));

#nullable disable
    private static CollectItems Instance;
#nullable enable

    private readonly PerScreen<List<StorageNode>> _eligible = new(() => new());
    private readonly Harmony _harmony;
    private readonly IModHelper _helper;

    private CollectItems(IModHelper helper)
    {
        this._helper = helper;
        this._harmony = new(CollectItems.Id);
    }

    private static List<StorageNode> Eligible => CollectItems.Instance._eligible.Value;

    /// <summary>
    ///     Initializes <see cref="CollectItems" />.
    /// </summary>
    /// <param name="helper">SMAPI helper for events, input, and content.</param>
    /// <returns>Returns an instance of the <see cref="CollectItems" /> class.</returns>
    public static Feature Init(IModHelper helper)
    {
        return CollectItems.Instance ??= new(helper);
    }

    /// <inheritdoc />
    protected override void Activate()
    {
        // Events
        Configurator.StorageEdited += CollectItems.OnStorageEdited;
        this._helper.Events.GameLoop.SaveLoaded += CollectItems.OnSaveLoaded;
        this._helper.Events.Player.InventoryChanged += CollectItems.OnInventoryChanged;

        // Patches
        this._harmony.Patch(
            CollectItems.DebrisCollect,
            transpiler: new(typeof(CollectItems), nameof(CollectItems.Debris_collect_transpiler)));
    }

    /// <inheritdoc />
    protected override void Deactivate()
    {
        // Events
        Configurator.StorageEdited -= CollectItems.OnStorageEdited;
        this._helper.Events.GameLoop.SaveLoaded -= CollectItems.OnSaveLoaded;
        this._helper.Events.Player.InventoryChanged -= CollectItems.OnInventoryChanged;

        // Patches
        this._harmony.Unpatch(
            CollectItems.DebrisCollect,
            AccessTools.Method(typeof(CollectItems), nameof(CollectItems.Debris_collect_transpiler)));
    }

    private static bool AddItemToInventoryBool(Farmer farmer, Item? item, bool makeActiveObject)
    {
        if (item is null)
        {
            return true;
        }

        if (!CollectItems.Eligible.Any())
        {
            return farmer.addItemToInventoryBool(item, makeActiveObject);
        }

        foreach (var storage in CollectItems.Eligible)
        {
            if (storage is not { Data: Storage storageObject })
            {
                continue;
            }

            item.resetState();
            storageObject.ClearNulls();
            item = storage.StashItem(item, storage.StashToChestStacks is FeatureOption.Enabled);

            if (item is null)
            {
                break;
            }
        }

        return item is null || farmer.addItemToInventoryBool(item, makeActiveObject);
    }

    private static IEnumerable<CodeInstruction> Debris_collect_transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return instructions.MethodReplacer(
            AccessTools.Method(typeof(Farmer), nameof(Farmer.addItemToInventoryBool)),
            AccessTools.Method(typeof(CollectItems), nameof(CollectItems.AddItemToInventoryBool)));
    }

    private static void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
    {
        if (e.IsLocalPlayer && (e.Added.OfType<Chest>().Any() || e.Removed.OfType<Chest>().Any()))
        {
            CollectItems.RefreshEligible();
        }
    }

    private static void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        CollectItems.RefreshEligible();
    }

    private static void OnStorageEdited(object? sender, StorageNode storage)
    {
        CollectItems.RefreshEligible();
    }

    private static void RefreshEligible()
    {
        CollectItems.Eligible.Clear();
        foreach (var storage in Storages.FromPlayer(Game1.player, limit: 12))
        {
            if (storage is not { CollectItems: FeatureOption.Enabled })
            {
                continue;
            }

            CollectItems.Eligible.Add(storage);
        }
    }
}