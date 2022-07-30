/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Features;

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewMods.BetterChests.Helpers;
using StardewMods.Common.Enums;
using StardewMods.Common.Integrations.BetterChests;
using StardewMods.CommonHarmony.Enums;
using StardewMods.CommonHarmony.Helpers;
using StardewMods.CommonHarmony.Models;
using StardewValley;
using StardewValley.Objects;

/// <summary>
///     Debris such as mined or farmed items can be collected into a Chest in the farmer's inventory.
/// </summary>
internal class CollectItems : IFeature
{
    private const string Id = "furyx639.BetterChests/CollectItems";

    private readonly PerScreen<List<IStorageObject>?> _cachedEligible = new();

    private CollectItems(IModHelper helper)
    {
        this.Helper = helper;
        HarmonyHelper.AddPatches(
            CollectItems.Id,
            new SavedPatch[]
            {
                new(
                    AccessTools.Method(typeof(Debris), nameof(Debris.collect)),
                    typeof(CollectItems),
                    nameof(CollectItems.Debris_collect_transpiler),
                    PatchType.Transpiler),
            });
    }

    private static IEnumerable<IStorageObject> Eligible
    {
        get
        {
            foreach (var item in Game1.player.Items.Take(12))
            {
                if (StorageHelper.TryGetOne(item, out var storage) && storage.CollectItems != FeatureOption.Disabled)
                {
                    yield return storage;
                }
            }
        }
    }

    private static CollectItems? Instance { get; set; }

    private List<IStorageObject>? CachedEligible
    {
        get => this._cachedEligible.Value;
        set => this._cachedEligible.Value = value;
    }

    private IModHelper Helper { get; }

    private bool IsActivated { get; set; }

    /// <summary>
    ///     Initializes <see cref="CollectItems" />.
    /// </summary>
    /// <param name="helper">SMAPI helper for events, input, and content.</param>
    /// <returns>Returns an instance of the <see cref="CollectItems" /> class.</returns>
    public static CollectItems Init(IModHelper helper)
    {
        return CollectItems.Instance ??= new(helper);
    }

    /// <inheritdoc />
    public void Activate()
    {
        if (!this.IsActivated)
        {
            this.IsActivated = true;
            HarmonyHelper.ApplyPatches(CollectItems.Id);
            this.Helper.Events.Player.InventoryChanged += this.OnInventoryChanged;
        }
    }

    /// <inheritdoc />
    public void Deactivate()
    {
        if (this.IsActivated)
        {
            this.IsActivated = false;
            HarmonyHelper.UnapplyPatches(CollectItems.Id);
            this.Helper.Events.Player.InventoryChanged -= this.OnInventoryChanged;
        }
    }

    private static bool AddItemToInventoryBool(Farmer farmer, Item? item, bool makeActiveObject)
    {
        if (item is null)
        {
            return true;
        }

        CollectItems.Instance!.CachedEligible ??= CollectItems.Eligible.ToList();

        if (!CollectItems.Instance.CachedEligible.Any())
        {
            return farmer.addItemToInventoryBool(item, makeActiveObject);
        }

        foreach (var storage in CollectItems.Instance.CachedEligible)
        {
            item.resetState();
            storage.ClearNulls();
            item = storage.StashItem(item, storage.StashToChestStacks != FeatureOption.Disabled);

            if (item is null)
            {
                break;
            }
        }

        return item is null || farmer.addItemToInventoryBool(item, makeActiveObject);
    }

    private static IEnumerable<CodeInstruction> Debris_collect_transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Callvirt && instruction.operand.Equals(AccessTools.Method(typeof(Farmer), nameof(Farmer.addItemToInventoryBool))))
            {
                yield return new(OpCodes.Call, AccessTools.Method(typeof(CollectItems), nameof(CollectItems.AddItemToInventoryBool)));
            }
            else
            {
                yield return instruction;
            }
        }
    }

    private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
    {
        if (e.IsLocalPlayer && (e.Added.OfType<Chest>().Any() || e.Removed.OfType<Chest>().Any()))
        {
            this.CachedEligible = null;
        }
    }
}