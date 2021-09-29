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
    using Common.UI;
    using HarmonyLib;
    using Models;
    using Services;
    using StardewModdingAPI;
    using StardewModdingAPI.Events;
    using StardewModdingAPI.Utilities;
    using StardewValley;
    using StardewValley.Menus;
    using StardewValley.Objects;

    /// <inheritdoc />
    internal class CategorizeChestFeature : BaseFeature
    {
        private readonly IModHelper _helper;
        private readonly ModConfigService _modConfigService;
        private readonly ItemGrabMenuChangedService _itemGrabMenuChangedService;
        private readonly RenderedActiveMenuService _renderedActiveMenuService;
        private readonly PerScreen<int> _screenId = new();
        private readonly PerScreen<ItemGrabMenu?> _returnMenu = new();
        private readonly PerScreen<Chest> _chest = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="CategorizeChestFeature"/> class.
        /// </summary>
        /// <param name="modHelper">Provides simplified APIs for writing mods.</param>
        /// <param name="modConfigService">Service to handle read/write to ModConfig.</param>
        /// <param name="itemGrabMenuChangedService">Service to handle creation/invocation of ItemGrabMenuChanged event.</param>
        /// <param name="renderedActiveMenuService">Service to handle creation/invocation of RenderedActiveMenu event.</param>
        internal CategorizeChestFeature(
            IModHelper modHelper,
            ModConfigService modConfigService,
            ItemGrabMenuChangedService itemGrabMenuChangedService,
            RenderedActiveMenuService renderedActiveMenuService)
            : base("CategorizeChest")
        {
            this._helper = modHelper;
            this._modConfigService = modConfigService;
            this._itemGrabMenuChangedService = itemGrabMenuChangedService;
            this._renderedActiveMenuService = renderedActiveMenuService;
        }

        /// <inheritdoc/>
        public override void Activate(IModEvents modEvents, Harmony harmony)
        {
            this._itemGrabMenuChangedService.AddHandler(this.OnItemGrabMenuChanged);
        }

        /// <inheritdoc/>
        public override void Deactivate(IModEvents modEvents, Harmony harmony)
        {
            this._itemGrabMenuChangedService.RemoveHandler(this.OnItemGrabMenuChanged);
        }

        private void OnItemGrabMenuChanged(object sender, ItemGrabMenuEventArgs e)
        {
            if (e.ItemGrabMenu is null || e.Chest is null/* || !this.IsEnabledForItem(e.Chest)*/)
            {
                this._renderedActiveMenuService.RemoveHandler(this.OnRenderedActiveMenu);
                this._helper.Events.Input.ButtonPressed -= this.OnButtonPressed;
                return;
            }

            this._renderedActiveMenuService.RemoveHandler(this.OnRenderedActiveMenu);
            this._helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            this._screenId.Value = Context.ScreenId;
            this._returnMenu.Value = e.ItemGrabMenu;
            this._chest.Value = e.Chest;
        }

        private void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            // Draw config button
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.F9)
            {
                if (!this._chest.Value.modData.TryGetValue($"{XSPlus.ModPrefix}/FilterItems", out var filterItems))
                {
                    filterItems = string.Empty;
                }

                Game1.activeClickableMenu = new ItemSelectionMenu(
                    this._helper,
                    this._modConfigService.ModConfig.SearchTagSymbol,
                    this.ReturnToMenu,
                    filterItems,
                    value => this._chest.Value.modData[$"{XSPlus.ModPrefix}/FilterItems"] = value);
            }
        }

        private void ReturnToMenu()
        {
            Game1.activeClickableMenu = this._returnMenu.Value;
        }
    }
}