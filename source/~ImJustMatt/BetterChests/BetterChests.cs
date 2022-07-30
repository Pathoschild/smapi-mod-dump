/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewMods.BetterChests.Features;
using StardewMods.BetterChests.Helpers;
using StardewMods.Common.Enums;
using StardewMods.Common.Helpers;
using StardewMods.Common.Integrations.BetterChests;

/// <inheritdoc />
public class BetterChests : Mod
{
    private ModConfig? Config { get; set; }

    private Dictionary<string, (IFeature Feature, Func<bool> Condition)> Features { get; } = new();

    private Dictionary<Func<object, bool>, IStorageData> StorageTypes { get; } = new();

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        Log.Monitor = this.Monitor;
        LocationHelper.Multiplayer = this.Helper.Multiplayer;
        I18n.Init(helper.Translation);
        this.Config = ConfigHelper.Init(this.Helper, this.ModManifest, this.Features);
        IntegrationHelper.Init(this.Helper, this.Config);
        StorageHelper.Init(this.Config, this.StorageTypes);
        ThemeHelper.Init(this.Helper, "furyx639.BetterChests/Icons", "furyx639.BetterChests/Tabs/Texture");

        if (this.Helper.ModRegistry.IsLoaded("furyx639.FuryCore"))
        {
            Log.Alert("Remove FuryCore, it is no longer needed by this mod!");
        }

        // Events
        this.Helper.Events.Content.AssetRequested += BetterChests.OnAssetRequested;
        this.Helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

        // Features
        this.Features.Add(nameof(AutoOrganize), new(AutoOrganize.Init(this.Helper), () => this.Config.DefaultChest.AutoOrganize != FeatureOption.Disabled));
        this.Features.Add(nameof(BetterColorPicker), new(BetterColorPicker.Init(this.Helper, this.Config), () => this.Config.DefaultChest.CustomColorPicker != FeatureOption.Disabled));
        this.Features.Add(nameof(BetterItemGrabMenu), new(BetterItemGrabMenu.Init(this.Helper, this.Config), () => true));
        this.Features.Add(nameof(BetterShippingBin), new(BetterShippingBin.Init(this.Helper), () => this.Config.BetterShippingBin));
        this.Features.Add(nameof(CarryChest), new(CarryChest.Init(this.Helper, this.Config), () => this.Config.DefaultChest.CarryChest != FeatureOption.Disabled));
        this.Features.Add(nameof(LabelChest), new(LabelChest.Init(this.Helper), () => this.Config.LabelChest));
        this.Features.Add(nameof(ChestFinder), new(ChestFinder.Init(this.Helper, this.Config), () => this.Config.ChestFinder));
        this.Features.Add(nameof(ChestMenuTabs), new(ChestMenuTabs.Init(this.Helper, this.Config), () => this.Config.DefaultChest.ChestMenuTabs != FeatureOption.Disabled));
        this.Features.Add(nameof(CollectItems), new(CollectItems.Init(this.Helper), () => this.Config.DefaultChest.CollectItems != FeatureOption.Disabled));
        this.Features.Add(nameof(Configurator), new(Configurator.Init(this.Helper, this.Config, this.ModManifest), () => this.Config.Configurator && IntegrationHelper.GMCM.IsLoaded));
        this.Features.Add(nameof(CraftFromChest), new(CraftFromChest.Init(this.Helper, this.Config), () => this.Config.DefaultChest.CraftFromChest != FeatureOptionRange.Disabled));
        this.Features.Add(nameof(FilterItems), new(FilterItems.Init(this.Helper), () => this.Config.DefaultChest.FilterItems != FeatureOption.Disabled));
        this.Features.Add(nameof(OpenHeldChest), new(OpenHeldChest.Init(this.Helper), () => this.Config.DefaultChest.OpenHeldChest != FeatureOption.Disabled));
        this.Features.Add(nameof(OrganizeChest), new(OrganizeChest.Init(this.Helper), () => this.Config.DefaultChest.OrganizeChest != FeatureOption.Disabled));
        this.Features.Add(nameof(ResizeChest), new(ResizeChest.Init(), () => this.Config.DefaultChest.ResizeChest != FeatureOption.Disabled));
        this.Features.Add(nameof(ResizeChestMenu), new(ResizeChestMenu.Init(this.Helper), () => this.Config.DefaultChest.ResizeChestMenu != FeatureOption.Disabled));
        this.Features.Add(nameof(SearchItems), new(SearchItems.Init(this.Helper, this.Config), () => this.Config.DefaultChest.SearchItems != FeatureOption.Disabled));
        this.Features.Add(nameof(SlotLock), new(SlotLock.Init(this.Helper, this.Config), () => this.Config.SlotLock));
        this.Features.Add(nameof(StashToChest), new(StashToChest.Init(this.Helper, this.Config), () => this.Config.DefaultChest.StashToChest != FeatureOptionRange.Disabled));
        this.Features.Add(nameof(UnloadChest), new(UnloadChest.Init(this.Helper), () => this.Config.DefaultChest.UnloadChest != FeatureOption.Disabled));
    }

    /// <inheritdoc />
    public override object GetApi()
    {
        return new BetterChestsApi(this.StorageTypes);
    }

    private static void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (e.Name.IsEquivalentTo("furyx639.BetterChests/Icons"))
        {
            e.LoadFromModFile<Texture2D>("assets/icons.png", AssetLoadPriority.Exclusive);
            return;
        }

        if (e.Name.IsEquivalentTo("furyx639.BetterChests/Tabs/Texture"))
        {
            e.LoadFromModFile<Texture2D>("assets/tabs.png", AssetLoadPriority.Exclusive);
        }
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        foreach (var (featureName, (feature, condition)) in this.Features)
        {
            if (IntegrationHelper.TestConflicts(featureName, out var mods))
            {
                var modList = string.Join(", ", mods.OfType<IModInfo>().Select(mod => mod.Manifest.Name));
                Log.Warn(string.Format(I18n.Warn_Incompatibility_Disabled(), $"BetterChests.{featureName}", modList));
            }
            else if (condition())
            {
                feature.Activate();
            }
        }
    }
}