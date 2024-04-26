/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.Services.Features;

using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewMods.BetterChests.Framework.Interfaces;
using StardewMods.BetterChests.Framework.Models.Containers;
using StardewMods.BetterChests.Framework.Services.Factory;
using StardewMods.Common.Interfaces;
using StardewMods.Common.Services.Integrations.BetterChests.Enums;
using StardewMods.Common.Services.Integrations.BetterChests.Interfaces;
using StardewMods.Common.Services.Integrations.FauxCore;
using StardewMods.Common.Services.Integrations.ToolbarIcons;
using StardewValley.Locations;
using StardewValley.Menus;

/// <summary>Stash items into placed chests and chests in the farmer's inventory.</summary>
internal sealed class StashToChest : BaseFeature<StashToChest>
{
    private readonly AssetHandler assetHandler;
    private readonly ContainerFactory containerFactory;
    private readonly ContainerHandler containerHandler;
    private readonly IInputHelper inputHelper;
    private readonly MenuManager menuManager;
    private readonly ToolbarIconsIntegration toolbarIconsIntegration;

    /// <summary>Initializes a new instance of the <see cref="StashToChest" /> class.</summary>
    /// <param name="assetHandler">Dependency used for handling assets.</param>
    /// <param name="containerFactory">Dependency used for accessing containers.</param>
    /// <param name="containerHandler">Dependency used for handling operations between containers.</param>
    /// <param name="eventManager">Dependency used for managing events.</param>
    /// <param name="inputHelper">Dependency used for checking and changing input state.</param>
    /// <param name="menuManager">Dependency used for managing the current menu.</param>
    /// <param name="log">Dependency used for logging debug information to the console.</param>
    /// <param name="manifest">Dependency for accessing mod manifest.</param>
    /// <param name="modConfig">Dependency used for accessing config data.</param>
    /// <param name="toolbarIconsIntegration">Dependency for Toolbar Icons integration.</param>
    public StashToChest(
        AssetHandler assetHandler,
        ContainerFactory containerFactory,
        ContainerHandler containerHandler,
        IEventManager eventManager,
        IInputHelper inputHelper,
        MenuManager menuManager,
        ILog log,
        IManifest manifest,
        IModConfig modConfig,
        ToolbarIconsIntegration toolbarIconsIntegration)
        : base(eventManager, log, manifest, modConfig)
    {
        this.assetHandler = assetHandler;
        this.containerFactory = containerFactory;
        this.containerHandler = containerHandler;
        this.inputHelper = inputHelper;
        this.menuManager = menuManager;
        this.toolbarIconsIntegration = toolbarIconsIntegration;
    }

    /// <inheritdoc />
    public override bool ShouldBeActive => this.Config.DefaultOptions.StashToChest != RangeOption.Disabled;

    /// <inheritdoc />
    protected override void Activate()
    {
        // Events
        this.Events.Subscribe<ButtonsChangedEventArgs>(this.OnButtonsChanged);
        this.Events.Subscribe<ButtonPressedEventArgs>(this.OnButtonPressed);
        this.Events.Subscribe<RenderingActiveMenuEventArgs>(this.OnRenderingActiveMenu);

        // Integrations
        if (!this.toolbarIconsIntegration.IsLoaded)
        {
            return;
        }

        this.toolbarIconsIntegration.Api.AddToolbarIcon(
            this.Id,
            this.assetHandler.Icons.Name.BaseName,
            new Rectangle(16, 0, 16, 16),
            I18n.Button_StashToChest_Name());

        this.toolbarIconsIntegration.Api.Subscribe(this.OnIconPressed);
    }

    /// <inheritdoc />
    protected override void Deactivate()
    {
        // Events
        this.Events.Unsubscribe<ButtonsChangedEventArgs>(this.OnButtonsChanged);
        this.Events.Unsubscribe<ButtonPressedEventArgs>(this.OnButtonPressed);
        this.Events.Unsubscribe<RenderingActiveMenuEventArgs>(this.OnRenderingActiveMenu);

        // Integrations
        if (!this.toolbarIconsIntegration.IsLoaded)
        {
            return;
        }

        this.toolbarIconsIntegration.Api.RemoveToolbarIcon(this.Id);
        this.toolbarIconsIntegration.Api.Unsubscribe(this.OnIconPressed);
    }

    private void OnButtonPressed(ButtonPressedEventArgs e)
    {
        if (e.Button is not SButton.MouseLeft
            || this.menuManager.CurrentMenu is not ItemGrabMenu itemGrabMenu
            || itemGrabMenu.fillStacksButton is null
            || this.menuManager.Bottom.Container is null
            || this.menuManager.Top.Container is null
            || !this.containerFactory.TryGetOne(out var container)
            || container.Options.StashToChest is RangeOption.Disabled or RangeOption.Default)
        {
            return;
        }

        var (mouseX, mouseY) = Game1.getMousePosition(true);
        if (!itemGrabMenu.fillStacksButton.containsPoint(mouseX, mouseY))
        {
            return;
        }

        this.inputHelper.Suppress(e.Button);
        Game1.playSound("Ship");

        // Stash to existing stacks only
        if (!this.Config.Controls.TransferItems.IsDown()
            || this.menuManager.Top.Container.Options.StashToChest is RangeOption.Disabled or RangeOption.Default)
        {
            this.containerHandler.Transfer(
                this.menuManager.Bottom.Container,
                this.menuManager.Top.Container,
                out _,
                existingOnly: true);

            return;
        }

        // Stash using categorization rules
        var (from, to) = this.Config.Controls.TransferItemsReverse.IsDown()
            ? (this.menuManager.Top.Container, this.menuManager.Bottom.Container)
            : (this.menuManager.Bottom.Container, this.menuManager.Top.Container);

        var force = to is FarmerContainer;
        if (!this.containerHandler.Transfer(from, to, out var amounts, force))
        {
            return;
        }

        foreach (var (name, amount) in amounts)
        {
            if (amount > 0)
            {
                this.Log.Trace(
                    "{0}: {{ Item: {1}, Quantity: {2}, From: {3}, To: {4} }}",
                    this.Id,
                    name,
                    amount,
                    from,
                    to);
            }
        }
    }

    private void OnButtonsChanged(ButtonsChangedEventArgs e)
    {
        if (!this.Config.Controls.StashItems.JustPressed())
        {
            return;
        }

        // Stash to All
        if (Context.IsPlayerFree)
        {
            this.inputHelper.SuppressActiveKeybinds(this.Config.Controls.StashItems);
            this.StashIntoAll();
            return;
        }

        if (!this.containerFactory.TryGetOne(out var container)
            || container.Options.StashToChest is RangeOption.Disabled or RangeOption.Default)
        {
            return;
        }

        // Stash to Current
        this.inputHelper.SuppressActiveKeybinds(this.Config.Controls.StashItems);
        this.StashIntoContainer(container);
        Game1.playSound("Ship");
    }

    private void OnRenderingActiveMenu(RenderingActiveMenuEventArgs obj)
    {
        if (this.menuManager.CurrentMenu is not ItemGrabMenu itemGrabMenu
            || itemGrabMenu.fillStacksButton is null
            || !this.containerFactory.TryGetOne(out var container)
            || container.Options.StashToChest is RangeOption.Disabled or RangeOption.Default)
        {
            return;
        }

        var (mouseX, mouseY) = Game1.getMousePosition(true);
        if (!this.Config.Controls.TransferItems.IsDown()
            || !itemGrabMenu.fillStacksButton.containsPoint(mouseX, mouseY))
        {
            itemGrabMenu.fillStacksButton.texture = Game1.mouseCursors;
            itemGrabMenu.fillStacksButton.sourceRect = new Rectangle(103, 469, 16, 16);
            itemGrabMenu.fillStacksButton.hoverText = Game1.content.LoadString("Strings\\UI:ItemGrab_FillStacks");
            return;
        }

        itemGrabMenu.fillStacksButton.texture = this.assetHandler.Icons.Value;

        itemGrabMenu.fillStacksButton.sourceRect = this.Config.Controls.TransferItemsReverse.IsDown()
            ? new Rectangle(96, 0, 16, 16)
            : new Rectangle(80, 0, 16, 16);

        itemGrabMenu.fillStacksButton.hoverText = this.Config.Controls.TransferItemsReverse.IsDown()
            ? I18n.Button_TransferDown_Name()
            : I18n.Button_TransferUp_Name();
    }

    private void OnIconPressed(IIconPressedEventArgs e)
    {
        if (e.Id == this.Id)
        {
            this.StashIntoAll();
        }
    }

    private void StashIntoAll()
    {
        if (!this.containerFactory.TryGetOne(Game1.player, out var containerFrom))
        {
            return;
        }

        var containerGroups =
            this
                .containerFactory.GetAll(Predicate)
                .GroupBy(container => container.Options.StashToChestPriority)
                .ToDictionary(group => group.Key, group => group.ToList());

        if (!containerGroups.Any())
        {
            Game1.showRedMessage(I18n.Alert_StashToChest_NoEligible());
            return;
        }

        var topPriority = containerGroups.Keys.Max();
        var bottomPriority = containerGroups.Keys.Min();
        var stashedAny = false;

        for (var priority = topPriority; priority >= bottomPriority; --priority)
        {
            if (!containerGroups.TryGetValue(priority, out var containersTo))
            {
                continue;
            }

            foreach (var containerTo in containersTo)
            {
                if (!this.containerHandler.Transfer(containerFrom, containerTo, out var amounts))
                {
                    continue;
                }

                stashedAny = true;
                foreach (var (name, amount) in amounts)
                {
                    if (amount <= 0)
                    {
                        continue;
                    }

                    this.Log.Trace(
                        "{0}: {{ Item: {1}, Quantity: {2}, From: {3}, To: {4} }}",
                        this.Id,
                        name,
                        amount,
                        containerFrom,
                        containerTo);
                }
            }
        }

        if (!stashedAny)
        {
            this.Log.Alert(I18n.Alert_StashToChest_NoEligible());
            return;
        }

        Game1.playSound("Ship");
        return;

        bool Predicate(IStorageContainer container) =>
            container is not FarmerContainer
            && container.Options.StashToChest is not (RangeOption.Disabled or RangeOption.Default)
            && !this.Config.StashToChestDisableLocations.Contains(Game1.player.currentLocation.Name)
            && !(this.Config.StashToChestDisableLocations.Contains("UndergroundMine")
                && Game1.player.currentLocation is MineShaft mineShaft
                && mineShaft.Name.StartsWith("UndergroundMine", StringComparison.OrdinalIgnoreCase))
            && container.Options.StashToChest.WithinRange(
                container.Options.StashToChestDistance,
                container.Location,
                container.TileLocation);
    }

    private void StashIntoContainer(IStorageContainer containerTo)
    {
        if (!this.containerFactory.TryGetOne(Game1.player, out var containerFrom))
        {
            return;
        }

        if (!this.containerHandler.Transfer(containerFrom, containerTo, out var amounts))
        {
            return;
        }

        foreach (var (name, amount) in amounts)
        {
            if (amount > 0)
            {
                this.Log.Trace(
                    "{0}: {{ Item: {1}, Quantity: {2}, From: {3}, To: {4} }}",
                    this.Id,
                    name,
                    amount,
                    containerFrom,
                    containerTo);
            }
        }
    }
}