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
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;
using UIInfoSuite2.Infrastructure;
using UIInfoSuite2.Infrastructure.Extensions;

namespace UIInfoSuite2.UIElements
{
    internal class ShowRobinBuildingStatusIcon : IDisposable
    {
        #region Properties

        private bool _IsBuildingInProgress;
        private Rectangle? _buildingIconSpriteLocation;
        private string _hoverText;
        private PerScreen<ClickableTextureComponent> _buildingIcon = new();
        private Texture2D _robinIconSheet;

        private readonly IModHelper _helper;
        #endregion

        #region Lifecycle
        public ShowRobinBuildingStatusIcon(IModHelper helper)
        {
            _helper = helper;
        }

        public void Dispose()
        {
            ToggleOption(false);
        }

        public void ToggleOption(bool showRobinBuildingStatus)
        {
            _helper.Events.GameLoop.DayStarted -= OnDayStarted;
            _helper.Events.Display.RenderingHud -= OnRenderingHud;
            _helper.Events.Display.RenderedHud -= OnRenderedHud;

            if (showRobinBuildingStatus)
            {
                UpdateRobinBuindingStatusData();

                _helper.Events.GameLoop.DayStarted += OnDayStarted;
                _helper.Events.Display.RenderingHud += OnRenderingHud;
                _helper.Events.Display.RenderedHud += OnRenderedHud;
            }
        }
        #endregion

        #region Event subscriptions
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            UpdateRobinBuindingStatusData();
        }

        private void OnRenderingHud(object sender, RenderingHudEventArgs e)
        {
            // Draw icon
            if (!Game1.eventUp && _IsBuildingInProgress && _buildingIconSpriteLocation.HasValue)
            {
                Point iconPosition = IconHandler.Handler.GetNewIconPosition();
                _buildingIcon.Value =
                    new ClickableTextureComponent(
                        new Rectangle(iconPosition.X, iconPosition.Y, 40, 40),
                        _robinIconSheet,
                        _buildingIconSpriteLocation.Value,
                        8 / 3f);
                _buildingIcon.Value.draw(Game1.spriteBatch);
            }
        }

        private void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            // Show text on hover
            if (_IsBuildingInProgress && (_buildingIcon.Value?.containsPoint(Game1.getMouseX(), Game1.getMouseY()) ?? false) && !String.IsNullOrEmpty(_hoverText))
            {
                IClickableMenu.drawHoverText(
                    Game1.spriteBatch,
                    _hoverText,
                    Game1.dialogueFont
                );
            }
        }
        #endregion

        #region Logic
        private void UpdateRobinBuindingStatusData()
        {
            Building buildingUnderConstruction = Game1.getFarm().getBuildingUnderConstruction();
            if (buildingUnderConstruction is null)
            {
                _IsBuildingInProgress = false;
                _hoverText = String.Empty;
            }
            else
            {
                _IsBuildingInProgress = true;
                _hoverText = String.Format(_helper.SafeGetString(LanguageKeys.RobinBuildingStatus), buildingUnderConstruction.daysOfConstructionLeft.Value > 0 ? buildingUnderConstruction.daysOfConstructionLeft.Value : buildingUnderConstruction.daysUntilUpgrade.Value);

                FindRobinSpritesheet();
            }
        }

        private void FindRobinSpritesheet()
        {
            Texture2D? foundTexture = Game1.getCharacterFromName("Robin")?.Sprite?.Texture;
            if (foundTexture != null)
            {
                _robinIconSheet = foundTexture;
            }
            else
            {
                ModEntry.MonitorObject.Log($"{this.GetType().Name}: Could not find Robin spritesheet.", LogLevel.Warn);
            }
            if (_robinIconSheet == null)
            {
                ModEntry.MonitorObject.Log($"{this.GetType().Name}: Could not find Robin spritesheet.", LogLevel.Warn);
            }

            _buildingIconSpriteLocation = new Rectangle(0, 195 + 1, 15, 15 - 1);    // 1px edits for better alignment with other icons
        }
        #endregion
    }
}
