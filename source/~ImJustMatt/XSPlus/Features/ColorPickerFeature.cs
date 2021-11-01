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
    using System.Diagnostics.CodeAnalysis;
    using Common.Services;
    using Common.UI;
    using CommonHarmony.Enums;
    using CommonHarmony.Models;
    using CommonHarmony.Services;
    using HarmonyLib;
    using Microsoft.Xna.Framework.Graphics;
    using StardewModdingAPI;
    using StardewModdingAPI.Events;
    using StardewModdingAPI.Utilities;
    using StardewValley;
    using StardewValley.Menus;
    using StardewValley.Objects;

    /// <inheritdoc />
    internal class ColorPickerFeature : BaseFeature
    {
        private const int Width = 58;
        private const int Height = 558;
        private static ColorPickerFeature Instance;
        private readonly PerScreen<Chest> _fakeChest = new();
        private readonly PerScreen<HSLSlider> _hslSlider = new();
        private readonly PerScreen<ItemGrabMenuEventArgs> _menu = new();
        private HarmonyService _harmony;
        private ItemGrabMenuChangedService _itemGrabMenuChanged;
        private ItemGrabMenuSideButtonsService _itemGrabMenuSideButtons;
        private RenderedActiveMenuService _renderedActiveMenu;

        private ColorPickerFeature(ServiceManager serviceManager)
            : base("ColorPicker", serviceManager)
        {
            ColorPickerFeature.Instance ??= this;

            // Dependencies
            this.AddDependency<ItemGrabMenuChangedService>(service => this._itemGrabMenuChanged = service as ItemGrabMenuChangedService);
            this.AddDependency<ItemGrabMenuSideButtonsService>(service => this._itemGrabMenuSideButtons = service as ItemGrabMenuSideButtonsService);
            this.AddDependency<RenderedActiveMenuService>(service => this._renderedActiveMenu = service as RenderedActiveMenuService);
            this.AddDependency<HarmonyService>(
                service =>
                {
                    // Init
                    this._harmony = service as HarmonyService;

                    // Patches
                    this._harmony?.AddPatch(
                        this.ServiceName,
                        AccessTools.Method(typeof(ItemGrabMenu), nameof(ItemGrabMenu.setSourceItem)),
                        typeof(ColorPickerFeature),
                        nameof(ColorPickerFeature.ItemGrabMenu_setSourceItem_postfix),
                        PatchType.Postfix);
                });
        }

        /// <inheritdoc />
        public override void Activate()
        {
            // Events
            this._itemGrabMenuChanged.AddHandler(this.OnItemGrabMenuChanged);
            this._itemGrabMenuSideButtons.AddHandler(ColorPickerFeature.OnSideButtonPressed);
            this._renderedActiveMenu.AddHandler(this.OnRenderedActiveMenu);
            this.Helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            this.Helper.Events.Input.ButtonReleased += this.OnButtonReleased;
            this.Helper.Events.Input.CursorMoved += this.OnCursorMoved;
            this.Helper.Events.Input.MouseWheelScrolled += this.OnMouseWheelScrolled;

            // Patches
            this._harmony.ApplyPatches(this.ServiceName);
        }

        /// <inheritdoc />
        public override void Deactivate()
        {
            // Events
            this._itemGrabMenuChanged.RemoveHandler(this.OnItemGrabMenuChanged);
            this._itemGrabMenuSideButtons.RemoveHandler(ColorPickerFeature.OnSideButtonPressed);
            this._renderedActiveMenu.RemoveHandler(this.OnRenderedActiveMenu);
            this.Helper.Events.Input.ButtonPressed -= this.OnButtonPressed;
            this.Helper.Events.Input.ButtonReleased -= this.OnButtonReleased;
            this.Helper.Events.Input.CursorMoved -= this.OnCursorMoved;
            this.Helper.Events.Input.MouseWheelScrolled -= this.OnMouseWheelScrolled;

            // Patches
            this._harmony.UnapplyPatches(this.ServiceName);
        }

        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Naming is determined by Harmony.")]
        private static void ItemGrabMenu_setSourceItem_postfix(ItemGrabMenu __instance)
        {
            if (__instance.context is not Chest chest || !ColorPickerFeature.Instance.IsEnabledForItem(chest))
            {
                return;
            }

            __instance.chestColorPicker = null;
            __instance.discreteColorPickerCC = null;
        }

        private void OnItemGrabMenuChanged(object sender, ItemGrabMenuEventArgs e)
        {
            if (e.ItemGrabMenu is null || e.Chest is null || !this.IsEnabledForItem(e.Chest))
            {
                this._menu.Value = null;
                return;
            }

            if (e.IsNew)
            {
                // Remove vanilla color picker
                e.ItemGrabMenu.chestColorPicker = null;
                e.ItemGrabMenu.discreteColorPickerCC = null;
            }

            this._fakeChest.Value = new(true, e.Chest.ParentSheetIndex)
            {
                Name = e.Chest.Name,
                lidFrameCount =
                {
                    Value = e.Chest.lidFrameCount.Value,
                },
                playerChoiceColor =
                {
                    Value = e.Chest.playerChoiceColor.Value,
                },
            };

            foreach (var modData in e.Chest.modData)
            {
                this._fakeChest.Value.modData.CopyFrom(modData);
            }

            this._fakeChest.Value.resetLidFrame();

            this._hslSlider.Value ??= new(this.Helper.Content.Load<Texture2D>);
            this._hslSlider.Value.Area = new(e.ItemGrabMenu.xPositionOnScreen + e.ItemGrabMenu.width + 96 + IClickableMenu.borderWidth / 2, e.ItemGrabMenu.yPositionOnScreen - 56 + IClickableMenu.borderWidth / 2, ColorPickerFeature.Width, ColorPickerFeature.Height);
            this._hslSlider.Value.CurrentColor = e.Chest.playerChoiceColor.Value;

            this._menu.Value = e;
        }

        private static bool OnSideButtonPressed(SideButtonPressedEventArgs e)
        {
            if (e.Type != ButtonType.ColorPickerToggleButton)
            {
                return false;
            }

            // Override color picker
            Game1.player.showChestColorPicker = !Game1.player.showChestColorPicker;
            return true;
        }

        private void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            if (this._menu.Value is null || this._menu.Value.ScreenId != Context.ScreenId || !Game1.player.showChestColorPicker)
            {
                return;
            }

            var x = this._hslSlider.Value.Area.Left;
            var y = this._hslSlider.Value.Area.Top - IClickableMenu.borderWidth / 2 - Game1.tileSize;
            this._fakeChest.Value.draw(e.SpriteBatch, x, y, 1f, true);
            this._hslSlider.Value.Draw(e.SpriteBatch);
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (this._menu.Value is null || this._menu.Value.ScreenId != Context.ScreenId || !Game1.player.showChestColorPicker)
            {
                return;
            }

            if (this._hslSlider.Value.LeftClick())
            {
                Game1.playSound("coin");
                this._fakeChest.Value.playerChoiceColor.Value = this._hslSlider.Value.CurrentColor;
            }
        }

        private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (this._menu.Value is null || this._menu.Value.ScreenId != Context.ScreenId || !Game1.player.showChestColorPicker || e.Button != SButton.MouseLeft)
            {
                return;
            }

            if (this._hslSlider.Value.LeftReleased())
            {
                this._fakeChest.Value.playerChoiceColor.Value = this._hslSlider.Value.CurrentColor;
                this._menu.Value.Chest.playerChoiceColor.Value = this._fakeChest.Value.playerChoiceColor.Value;
            }
        }

        private void OnCursorMoved(object sender, CursorMovedEventArgs e)
        {
            if (this._menu.Value is null || this._menu.Value.ScreenId != Context.ScreenId || !Game1.player.showChestColorPicker)
            {
                return;
            }

            if (this._hslSlider.Value.OnHover())
            {
                this._fakeChest.Value.playerChoiceColor.Value = this._hslSlider.Value.CurrentColor;
            }
        }

        private void OnMouseWheelScrolled(object sender, MouseWheelScrolledEventArgs e)
        {
            if (this._menu.Value is null || this._menu.Value.ScreenId != Context.ScreenId || !Game1.player.showChestColorPicker)
            {
                return;
            }

            if (this._hslSlider.Value.OnScroll(e.Delta))
            {
                this._fakeChest.Value.playerChoiceColor.Value = this._hslSlider.Value.CurrentColor;
                this._menu.Value.Chest.playerChoiceColor.Value = this._fakeChest.Value.playerChoiceColor.Value;
            }
        }
    }
}