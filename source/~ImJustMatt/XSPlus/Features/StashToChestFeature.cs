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
    using System.Linq;
    using Common.Helpers.ItemMatcher;
    using HarmonyLib;
    using Services;
    using StardewModdingAPI;
    using StardewModdingAPI.Events;
    using StardewModdingAPI.Utilities;
    using StardewValley;
    using StardewValley.Objects;

    /// <inheritdoc />
    internal class StashToChestFeature : FeatureWithParam<string>
    {
        private readonly IInputHelper _inputHelper;
        private readonly ModConfigService _modConfigService;
        private readonly PerScreen<List<Chest>?> _cachedEnabledChests = new();
        private readonly PerScreen<IList<Chest>?> _cachedPlayerChests = new();
        private readonly PerScreen<IList<Chest>?> _cachedGameChests = new();

        /// <summary>Initializes a new instance of the <see cref="StashToChestFeature"/> class.</summary>
        /// <param name="inputHelper">API for changing state of input.</param>
        /// <param name="modConfigService">Service to handle read/write to ModConfig.</param>
        public StashToChestFeature(IInputHelper inputHelper, ModConfigService modConfigService)
            : base("StashToChest")
        {
            this._inputHelper = inputHelper;
            this._modConfigService = modConfigService;
        }

        private List<Chest> EnabledChests
        {
            get
            {
                this._cachedPlayerChests.Value ??= Game1.player.Items.OfType<Chest>().Where(this.IsEnabledForItem).ToList();
                this._cachedGameChests.Value ??= XSPlus.AccessibleChests.Where(this.IsEnabledForItem).ToList();
                return this._cachedEnabledChests.Value ??= this._cachedPlayerChests.Value.Union(this._cachedGameChests.Value).ToList();
            }
        }

        /// <inheritdoc/>
        public override void Activate(IModEvents modEvents, Harmony harmony)
        {
            // Events
            modEvents.Player.InventoryChanged += this.OnInventoryChanged;
            modEvents.Player.Warped += this.OnWarped;
            modEvents.Input.ButtonsChanged += this.OnButtonsChanged;
        }

        /// <inheritdoc/>
        public override void Deactivate(IModEvents modEvents, Harmony harmony)
        {
            // Events
            modEvents.Player.InventoryChanged -= this.OnInventoryChanged;
            modEvents.Player.Warped -= this.OnWarped;
            modEvents.Input.ButtonsChanged -= this.OnButtonsChanged;
        }

        /// <inheritdoc/>
        [SuppressMessage("ReSharper", "HeapView.BoxingAllocation", Justification = "Required for enumerating this collection.")]
        protected internal override bool IsEnabledForItem(Item item)
        {
            if (!base.IsEnabledForItem(item) || item is not Chest chest || !chest.playerChest.Value || !this.TryGetValueForItem(item, out string range))
            {
                return false;
            }

            return range switch
            {
                "Inventory" => Game1.player.Items.IndexOf(item) != -1,
                "Location" => Game1.currentLocation.Objects.Values.Contains(item),
                "World" => true,
                _ => false,
            };
        }

        /// <inheritdoc/>
        protected override bool TryGetValueForItem(Item item, out string param)
        {
            if (base.TryGetValueForItem(item, out param))
            {
                return true;
            }

            param = this._modConfigService.ModConfig.StashingRange;
            return !string.IsNullOrWhiteSpace(param);
        }

        private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            if (!e.IsLocalPlayer)
            {
                return;
            }

            this._cachedPlayerChests.Value = null;
            this._cachedEnabledChests.Value = null;
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            this._cachedGameChests.Value = null;
            this._cachedEnabledChests.Value = null;
        }

        /// <summary>Stash inventory items into all supported chests.</summary>
        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsPlayerFree || !this._modConfigService.ModConfig.StashItems.JustPressed() || !this.EnabledChests.Any())
            {
                return;
            }

            for (int i = Game1.player.Items.Count - 1; i >= 0; i--)
            {
                Item item = Game1.player.Items[i];
                if (item is not null)
                {
                    Game1.player.Items[i] = this.TryAddItem(item);
                }
            }

            Game1.playSound("Ship");
            this._inputHelper.SuppressActiveKeybinds(this._modConfigService.ModConfig.StashItems);
        }

        private Item? TryAddItem(Item item)
        {
            var itemMatcher = new ItemMatcher(this._modConfigService.ModConfig.SearchTagSymbol);
            uint stack = (uint)item.Stack;
            foreach (Chest chest in this.EnabledChests)
            {
                bool allowList = FilterItemsFeature.Instance.IsEnabledForItem(chest);

                if (chest.modData.TryGetValue($"{XSPlus.ModPrefix}/FilterItems", out string filterItems))
                {
                    itemMatcher.SetSearch(filterItems);

                    // Skip chest if per-chest filter does not match
                    if (!itemMatcher.Matches(item))
                    {
                        continue;
                    }
                }
                else if (!allowList)
                {
                    // Skip chest if no built-in filter and no per-chest filter
                    continue;
                }

                // Attempt to add item into chest
                Item tmp = chest.addItem(item);
                if (tmp is null || tmp.Stack <= 0)
                {
                    return null;
                }

                if (tmp.Stack != stack)
                {
                    item.Stack = tmp.Stack;
                }
            }

            return item;
        }
    }
}