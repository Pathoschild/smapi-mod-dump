/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Annosz/UiInfoSuite2
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using UIInfoSuite2.Infrastructure;
using UIInfoSuite2.Infrastructure.Extensions;

namespace UIInfoSuite2.UIElements
{
    internal class ShowToolUpgradeStatus : IDisposable
    {
        #region Properties
        private readonly PerScreen<Rectangle?> _toolTexturePosition = new();
        private readonly PerScreen<string> _hoverText = new();
        private readonly PerScreen<Tool?> _toolBeingUpgraded = new();
        private readonly PerScreen<ClickableTextureComponent> _toolUpgradeIcon = new();

        private readonly IModHelper _helper;
        #endregion


        #region Life cycle
        public ShowToolUpgradeStatus(IModHelper helper)
        {
            _helper = helper;
        }

        public void Dispose()
        {
            ToggleOption(false);
            _toolBeingUpgraded.Value = null;
        }

        public void ToggleOption(bool showToolUpgradeStatus)
        {
            _helper.Events.Display.RenderingHud -= OnRenderingHud;
            _helper.Events.Display.RenderedHud -= OnRenderedHud;
            _helper.Events.GameLoop.DayStarted -= OnDayStarted;
            _helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;

            if (showToolUpgradeStatus)
            {
                UpdateToolInfo();
                _helper.Events.Display.RenderingHud += OnRenderingHud;
                _helper.Events.Display.RenderedHud += OnRenderedHud;
                _helper.Events.GameLoop.DayStarted += OnDayStarted;
                _helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            }
        }
        #endregion


        #region Event subscriptions
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (e.IsOneSecond && _toolBeingUpgraded.Value != Game1.player.toolBeingUpgraded.Value)
                UpdateToolInfo();
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            UpdateToolInfo();
        }

        private void OnRenderingHud(object sender, RenderingHudEventArgs e)
        {
            // Draw a 40x40 icon
            if (!Game1.eventUp && _toolBeingUpgraded.Value != null)
            {
                Point iconPosition = IconHandler.Handler.GetNewIconPosition();
                _toolUpgradeIcon.Value = new ClickableTextureComponent(
                        new Rectangle(iconPosition.X, iconPosition.Y, 40, 40),
                        Game1.toolSpriteSheet,
                        new Rectangle(),
                        2.5f);

                if (_toolTexturePosition.Value is Rectangle toolSourceRect)
                {
                    _toolUpgradeIcon.Value.sourceRect = toolSourceRect;
                    _toolUpgradeIcon.Value.draw(e.SpriteBatch);
                }
                else
                {
                    // Generic method for modded tools
                    try
                    {
                        // drawInMenu draws a 64x64 texture (16x16 texture at scale 4 = pixelZoom) if scaleSize is set to 1.
                        // It aligns position + (32, 32) with the center of the texture but we want to align position + 20, so that's an offset of -12.
                        _toolBeingUpgraded.Value.drawInMenu(e.SpriteBatch, iconPosition.ToVector2() + new Vector2(-12), 2.5f / Game1.pixelZoom);
                    }
                    catch (Exception ex)
                    {
                        ModEntry.MonitorObject.LogOnce($"An error occured while displaying the {_toolBeingUpgraded.Value.Name} tool.", LogLevel.Error);
                        ModEntry.MonitorObject.Log(ex.ToString());
                    }
                }
            }
        }

        private void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            // Show text on hover
            if (_toolBeingUpgraded.Value != null && (_toolUpgradeIcon.Value?.containsPoint(Game1.getMouseX(), Game1.getMouseY()) ?? false))
            {
                IClickableMenu.drawHoverText(
                    Game1.spriteBatch,
                    _hoverText.Value,
                    Game1.dialogueFont
                );
            }
        }
        #endregion


        #region Logic
        private void UpdateToolInfo()
        {
            Tool toolBeingUpgraded = _toolBeingUpgraded.Value = Game1.player.toolBeingUpgraded.Value;

            if (toolBeingUpgraded == null)
            {
                return;
            }

            if (toolBeingUpgraded is (Axe or Pickaxe or Hoe or WateringCan)
                || toolBeingUpgraded is GenericTool trashcan && trashcan.IndexOfMenuItemView is (>= 13 and <= 16))
            {
                // NB The previous method used Tool.UpgradeLevel, but it turns out that field is not correctly set by the game.
                //    Tools other than the Trash Cans only worked because they had special handling code.

                // Read the 16x16 source rectangle based on Tool.IndexOfMenuItemView
                _toolTexturePosition.Value = Game1.getSquareSourceRectForNonStandardTileSheet(
                    Game1.toolSpriteSheet,
                    16, 16,
                    toolBeingUpgraded.IndexOfMenuItemView);
            }
            else
            {
                _toolTexturePosition.Value = null;
            }

            if (Game1.player.daysLeftForToolUpgrade.Value > 0)
            {
                _hoverText.Value = string.Format(_helper.SafeGetString(LanguageKeys.DaysUntilToolIsUpgraded),
                    Game1.player.daysLeftForToolUpgrade.Value, toolBeingUpgraded.DisplayName);
            }
            else
            {
                _hoverText.Value = string.Format(_helper.SafeGetString(LanguageKeys.ToolIsFinishedBeingUpgraded),
                    toolBeingUpgraded.DisplayName);
            }
        }
        #endregion
    }
}
