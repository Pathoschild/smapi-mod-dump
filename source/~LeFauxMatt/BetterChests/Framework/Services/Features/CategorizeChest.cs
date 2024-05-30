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

using StardewModdingAPI.Utilities;
using StardewMods.BetterChests.Framework.Interfaces;
using StardewMods.BetterChests.Framework.Models.Events;
using StardewMods.Common.Helpers;
using StardewMods.Common.Interfaces;
using StardewMods.Common.Models;
using StardewMods.Common.Services.Integrations.BetterChests;
using StardewMods.Common.Services.Integrations.FauxCore;

/// <summary>Restricts what items can be added into a chest.</summary>
internal sealed class CategorizeChest : BaseFeature<CategorizeChest>
{
    private readonly PerScreen<List<Item>> cachedItems = new(() => []);
    private readonly IExpressionHandler expressionHandler;
    private readonly MenuHandler menuHandler;

    /// <summary>Initializes a new instance of the <see cref="CategorizeChest" /> class.</summary>
    /// <param name="eventManager">Dependency used for managing events.</param>
    /// <param name="expressionHandler">Dependency used for parsing expressions.</param>
    /// <param name="menuHandler">Dependency used for managing the current menu.</param>
    /// <param name="modConfig">Dependency used for accessing config data.</param>
    public CategorizeChest(
        IEventManager eventManager,
        IExpressionHandler expressionHandler,
        MenuHandler menuHandler,
        IModConfig modConfig)
        : base(eventManager, modConfig)
    {
        this.expressionHandler = expressionHandler;
        this.menuHandler = menuHandler;
    }

    /// <inheritdoc />
    public override bool ShouldBeActive => this.Config.DefaultOptions.CategorizeChest != FeatureOption.Disabled;

    /// <inheritdoc />
    protected override void Activate()
    {
        // Events
        this.Events.Subscribe<ItemHighlightingEventArgs>(this.OnItemHighlighting);
        this.Events.Subscribe<ItemsDisplayingEventArgs>(this.OnItemsDisplaying);
        this.Events.Subscribe<ItemTransferringEventArgs>(this.OnItemTransferring);
        this.Events.Subscribe<SearchChangedEventArgs>(this.OnSearchChanged);
    }

    /// <inheritdoc />
    protected override void Deactivate()
    {
        // Events
        this.Events.Unsubscribe<ItemHighlightingEventArgs>(this.OnItemHighlighting);
        this.Events.Unsubscribe<ItemsDisplayingEventArgs>(this.OnItemsDisplaying);
        this.Events.Unsubscribe<ItemTransferringEventArgs>(this.OnItemTransferring);
        this.Events.Unsubscribe<SearchChangedEventArgs>(this.OnSearchChanged);
    }

    private bool CanAcceptItem(IStorageContainer container, Item item, out bool accepted)
    {
        accepted = false;
        var includeStacks = container.CategorizeChestIncludeStacks == FeatureOption.Enabled;
        var hasStacks = container.Items.ContainsId(item.QualifiedItemId);
        if (includeStacks && hasStacks)
        {
            accepted = true;
            return true;
        }

        // Cannot handle if there is no search term
        if (string.IsNullOrWhiteSpace(container.CategorizeChestSearchTerm))
        {
            return false;
        }

        // Cannot handle if search term is invalid
        if (!this.expressionHandler.TryParseExpression(container.CategorizeChestSearchTerm, out var searchExpression))
        {
            return false;
        }

        // Check if item matches search expressions
        accepted = searchExpression.Equals(item);
        return true;
    }

    private void OnItemHighlighting(ItemHighlightingEventArgs e)
    {
        var top = this.menuHandler.Top.Container;
        if (e.Container == this.menuHandler.Bottom.Container
            && top is
            {
                CategorizeChest: FeatureOption.Enabled,
                CategorizeChestBlockItems: FeatureOption.Enabled,
            })
        {
            if (this.CanAcceptItem(top, e.Item, out var accepted) && !accepted)
            {
                e.UnHighlight();
            }

            return;
        }

        // Unhighlight items not actually in container
        if (e.Container == top && !e.Container.Items.Contains(e.Item))
        {
            e.UnHighlight();
        }
    }

    [Priority(int.MinValue + 1)]
    private void OnItemsDisplaying(ItemsDisplayingEventArgs e)
    {
        // Append searched items to the end of the list
        if (e.Container == this.menuHandler.Top.Container
            && e.Container.CategorizeChest is FeatureOption.Enabled
            && this.cachedItems.Value.Any())
        {
            e.Edit(items => items.Concat(this.cachedItems.Value.Except(e.Container.Items)));
        }
    }

    [Priority(int.MinValue)]
    private void OnItemTransferring(ItemTransferringEventArgs e)
    {
        // Only test if categorize is enabled
        if (e.Into.CategorizeChest != FeatureOption.Enabled || !this.CanAcceptItem(e.Into, e.Item, out var accepted))
        {
            return;
        }

        if (accepted)
        {
            e.AllowTransfer();
        }
        else if (e.Into.CategorizeChestBlockItems == FeatureOption.Enabled)
        {
            e.PreventTransfer();
        }
    }

    private void OnSearchChanged(SearchChangedEventArgs e)
    {
        if (e.SearchExpression is null || string.IsNullOrWhiteSpace(e.SearchTerm))
        {
            this.cachedItems.Value = [];
            return;
        }

        this.cachedItems.Value = [..ItemRepository.GetItems(e.SearchExpression.Equals)];
    }
}