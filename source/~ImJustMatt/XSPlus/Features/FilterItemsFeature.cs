/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace XSPlus.Features
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Common.Helpers.ItemMatcher;
    using Common.Services;
    using CommonHarmony;
    using CommonHarmony.Models;
    using CommonHarmony.Services;
    using HarmonyLib;
    using StardewModdingAPI;
    using StardewModdingAPI.Utilities;
    using StardewValley;
    using StardewValley.Objects;

    /// <inheritdoc cref="FeatureWithParam{TParam}" />
    internal class FilterItemsFeature : FeatureWithParam<Dictionary<string, bool>>
    {
        private static FilterItemsFeature Instance;
        private readonly ItemMatcher _addItemMatcher = new(string.Empty, true);
        private readonly PerScreen<ItemMatcher> _itemMatcher = new(() => new(string.Empty, true));
        private readonly PerScreen<ItemGrabMenuEventArgs> _menu = new();
        private HarmonyService _harmony;
        private HighlightItemsService _highlightItems;
        private ItemGrabMenuChangedService _itemGrabMenuChanged;

        private FilterItemsFeature(ServiceManager serviceManager)
            : base("FilterItems", serviceManager)
        {
            FilterItemsFeature.Instance ??= this;

            // Dependencies
            this.AddDependency<ItemGrabMenuChangedService>(service => this._itemGrabMenuChanged = service as ItemGrabMenuChangedService);
            this.AddDependency<HighlightItemsService>(service => this._highlightItems = service as HighlightItemsService);
            this.AddDependency<HarmonyService>(
                service =>
                {
                    // Init
                    this._harmony = service as HarmonyService;

                    // Patches
                    this._harmony?.AddPatch(
                        this.ServiceName,
                        AccessTools.Method(typeof(Chest), nameof(Chest.addItem)),
                        typeof(FilterItemsFeature),
                        nameof(FilterItemsFeature.Chest_addItem_prefix));

                    if (this.Helper.ModRegistry.IsLoaded("Pathochild.Automate"))
                    {
                        this._harmony?.AddPatch(
                            this.ServiceName,
                            new AssemblyPatch("Automate").Method("Pathoschild.Stardew.Automate.Framework.Storage.ChestContainer", "Store"),
                            typeof(FilterItemsFeature),
                            nameof(FilterItemsFeature.Automate_Store_prefix));
                    }
                });
        }

        /// <inheritdoc />
        public override void Activate()
        {
            // Events
            this._highlightItems.AddHandler(this.HighlightMethod);
            this._itemGrabMenuChanged.AddHandler(this.OnItemGrabMenuChangedEvent);

            // Patches
            this._harmony.ApplyPatches(this.ServiceName);
        }

        /// <inheritdoc />
        public override void Deactivate()
        {
            // Events
            this._itemGrabMenuChanged.RemoveHandler(this.OnItemGrabMenuChangedEvent);
            this._highlightItems.RemoveHandler(this.HighlightMethod);

            // Patches
            this._harmony.UnapplyPatches(this.ServiceName);
        }

        /// <inheritdoc />
        internal override bool IsEnabledForItem(Item item)
        {
            return base.IsEnabledForItem(item) || item is Chest chest && chest.playerChest.Value && chest.modData.TryGetValue($"{XSPlus.ModPrefix}/FilterItems", out var filterItems) && !string.IsNullOrWhiteSpace(filterItems);
        }

        public bool HasFilterItems(Chest chest)
        {
            return this.IsEnabledForItem(chest);
        }

        public bool Matches(Chest chest, Item item, ItemMatcher itemMatcher = null)
        {
            if (!this.IsEnabledForItem(chest))
            {
                return true;
            }

            itemMatcher ??= this._addItemMatcher;

            // Mod configured filter
            if (this.TryGetValueForItem(chest, out var modFilterItems))
            {
                itemMatcher.SetSearch(modFilterItems);
            }
            else
            {
                itemMatcher.SetSearch(string.Empty);
            }

            // Player configured filter
            itemMatcher.AddSearch(chest.GetFilterItems());

            return itemMatcher.Matches(item);
        }

        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Naming is determined by Harmony.")]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Type is determined by Harmony.")]
        [HarmonyPriority(Priority.High)]
        private static bool Chest_addItem_prefix(Chest __instance, ref Item __result, Item item)
        {
            if (FilterItemsFeature.Instance.Matches(__instance, item))
            {
                return true;
            }

            __result = item;
            return false;
        }

        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Naming is determined by Harmony.")]
        private static bool Automate_Store_prefix(Chest ___Chest, object stack)
        {
            return FilterItemsFeature.Instance.Matches(
                ___Chest,
                FilterItemsFeature.Instance.Helper.Reflection.GetProperty<Item>(stack, "Sample").GetValue());
        }

        private void OnItemGrabMenuChangedEvent(object sender, ItemGrabMenuEventArgs e)
        {
            if (e.ItemGrabMenu is null || e.Chest is null || !this.IsEnabledForItem(e.Chest))
            {
                this._menu.Value = null;
                return;
            }

            this._menu.Value = e;

            // Mod configured filter
            if (this.TryGetValueForItem(e.Chest, out var modFilterItems))
            {
                this._itemMatcher.Value.SetSearch(modFilterItems);
            }
            else
            {
                this._itemMatcher.Value.SetSearch(string.Empty);
            }

            // Player configured filter
            this._itemMatcher.Value.AddSearch(e.Chest.GetFilterItems());
        }

        private bool HighlightMethod(Item item)
        {
            if (this._menu.Value is null || this._menu.Value.ScreenId != Context.ScreenId || !this.IsEnabledForItem(this._menu.Value.Chest))
            {
                return true;
            }

            return this._itemMatcher.Value.Matches(item);
        }
    }
}