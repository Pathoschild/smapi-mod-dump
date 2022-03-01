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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Common.Helpers;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewMods.BetterChests.Enums;
using StardewMods.BetterChests.Interfaces.Config;
using StardewMods.FuryCore.Enums;
using StardewMods.FuryCore.Interfaces;
using StardewMods.FuryCore.Models;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;

/// <inheritdoc />
internal class OpenHeldChest : Feature
{
    private readonly Lazy<IHarmonyHelper> _harmony;

    /// <summary>
    ///     Initializes a new instance of the <see cref="OpenHeldChest" /> class.
    /// </summary>
    /// <param name="config">Data for player configured mod options.</param>
    /// <param name="helper">SMAPI helper for events, input, and content.</param>
    /// <param name="services">Provides access to internal and external services.</param>
    public OpenHeldChest(IConfigModel config, IModHelper helper, IModServices services)
        : base(config, helper, services)
    {
        this._harmony = services.Lazy<IHarmonyHelper>(
            harmony =>
            {
                harmony.AddPatches(
                    this.Id,
                    new SavedPatch[]
                    {
                        new(
                            AccessTools.Method(typeof(Chest), nameof(Chest.addItem)),
                            typeof(OpenHeldChest),
                            nameof(OpenHeldChest.Chest_addItem_prefix),
                            PatchType.Prefix),
                    });
            });
    }

    private IHarmonyHelper Harmony
    {
        get => this._harmony.Value;
    }

    /// <inheritdoc />
    protected override void Activate()
    {
        this.Harmony.ApplyPatches(this.Id);
        this.Helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        if (Context.IsMainPlayer)
        {
            this.Helper.Events.GameLoop.UpdateTicked += OpenHeldChest.OnUpdateTicked;
        }
    }

    /// <inheritdoc />
    protected override void Deactivate()
    {
        this.Harmony.UnapplyPatches(this.Id);
        this.Helper.Events.Input.ButtonPressed -= this.OnButtonPressed;
        if (Context.IsMainPlayer)
        {
            this.Helper.Events.GameLoop.UpdateTicked -= OpenHeldChest.OnUpdateTicked;
        }
    }

    /// <summary>Prevent adding chest into itself.</summary>
    [HarmonyPriority(Priority.High)]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Naming is determined by Harmony.")]
    [SuppressMessage("StyleCop", "SA1313", Justification = "Naming is determined by Harmony.")]
    private static bool Chest_addItem_prefix(Chest __instance, ref Item __result, Item item)
    {
        if (!ReferenceEquals(__instance, item))
        {
            return true;
        }

        __result = item;
        return false;
    }

    private static void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        if (!Context.IsPlayerFree)
        {
            return;
        }

        foreach (var player in Game1.getOnlineFarmers())
        {
            foreach (var item in player.Items.Take(12).OfType<Object>())
            {
                item.updateWhenCurrentLocation(Game1.currentGameTime, player.currentLocation);
            }
        }
    }

    /// <summary>Open inventory for currently held chest.</summary>
    private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsPlayerFree || !e.Button.IsActionButton() || Game1.player.CurrentItem is not Object obj)
        {
            return;
        }

        if (!this.ManagedObjects.TryGetManagedStorage(Game1.player.CurrentItem, out var managedStorage) || managedStorage.OpenHeldChest == FeatureOption.Disabled)
        {
            return;
        }

        Log.Trace($"Opening ItemGrabMenu for Held Chest ${managedStorage.QualifiedItemId}.");
        if (Context.IsMainPlayer)
        {
            obj.checkForAction(Game1.player);
        }
        else if (managedStorage.Context is Chest chest)
        {
            Game1.player.currentLocation.localSound("openChest");
            chest.ShowMenu();
        }

        this.Helper.Input.Suppress(e.Button);
    }
}