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
using Common.Helpers;
using Common.Integrations.FuryCore;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewMods.BetterChests.Features;
using StardewMods.BetterChests.Interfaces.Config;
using StardewMods.BetterChests.Models.Config;
using StardewMods.BetterChests.Services;
using StardewMods.FuryCore.Services;

/// <inheritdoc />
public class BetterChests : Mod
{
    /// <summary>
    ///     Gets the unique Mod Id.
    /// </summary>
    internal static string ModUniqueId { get; private set; }

    private ConfigModel Config { get; set; }

    private FuryCoreIntegration FuryCore { get; set; }

    private ModServices Services { get; } = new();

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        BetterChests.ModUniqueId = this.ModManifest.UniqueID;
        I18n.Init(helper.Translation);
        Log.Monitor = this.Monitor;
        this.FuryCore = new(this.Helper.ModRegistry);

        // Mod Config
        IConfigData config = null;
        try
        {
            config = this.Helper.ReadConfig<ConfigData>();
        }
        catch (Exception)
        {
            // ignored
        }

        this.Config = new(config ?? new ConfigData(), this.Helper, this.Services);

        // Core Services
        this.Services.Add(
            new AssetHandler(this.Config, this.Helper),
            new CommandHandler(this.Config, this.Helper, this.Services),
            new ManagedObjects(this.Config, this.Services),
            new ModConfigMenu(this.Config, this.Helper, this.ModManifest, this.Services),
            new ModIntegrations(this.Helper, this.Services));

        // Events
        this.Helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
    }

    /// <inheritdoc />
    public override object GetApi()
    {
        return new BetterChestsApi(this.Services);
    }

    private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
    {
        this.FuryCore.API.AddFuryCoreServices(this.Services);

        // Features
        this.Services.Add(
            new AutoOrganize(this.Config, this.Helper, this.Services),
            new CarryChest(this.Config, this.Helper, this.Services),
            new CategorizeChest(this.Config, this.Helper, this.Services),
            new ChestMenuTabs(this.Config, this.Helper, this.Services),
            new CollectItems(this.Config, this.Helper, this.Services),
            new Configurator(this.Config, this.Helper, this.Services),
            new CraftFromChest(this.Config, this.Helper, this.Services),
            new CustomColorPicker(this.Config, this.Helper, this.Services),
            new FilterItems(this.Config, this.Helper, this.Services),
            new InventoryProviderForBetterCrafting(this.Config, this.Helper, this.Services),
            new MenuForShippingBin(this.Config, this.Helper, this.Services),
            new OpenHeldChest(this.Config, this.Helper, this.Services),
            new OrganizeChest(this.Config, this.Helper, this.Services),
            new ResizeChest(this.Config, this.Helper, this.Services),
            new ResizeChestMenu(this.Config, this.Helper, this.Services),
            new SearchItems(this.Config, this.Helper, this.Services),
            new SlotLock(this.Config, this.Helper, this.Services),
            new StashToChest(this.Config, this.Helper, this.Services),
            new UnloadChest(this.Config, this.Helper, this.Services));

        // Activate Features
        foreach (var feature in this.Services.FindServices<Feature>())
        {
            feature.Toggle();
        }
    }
}