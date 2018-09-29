using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;

namespace MoreMultiplayerInfo
{
    public class PlayerHealthInfo : IClickableMenu
    {
        private readonly Rectangle _healthBarPosition;
        private readonly ProgressBar _healthProgress;
        private readonly ProgressBar _energyProgress;
        private Rectangle _energyBarPosition;

        public PlayerHealthInfo(int playerHealth, int playerMaxHealth, int playerEnergy, int playerMaxEnergy, Rectangle position)
        {
            xPositionOnScreen = Convert.ToInt32(position.X);
            yPositionOnScreen = Convert.ToInt32(position.Y);

            _healthBarPosition = position;

            _energyBarPosition = new Rectangle(position.X + 15 * Game1.pixelZoom, position.Y, position.Width, position.Height);

            _healthProgress = new ProgressBar(playerHealth, playerMaxHealth, new Rectangle(_healthBarPosition.X + 3 * Game1.pixelZoom, _healthBarPosition.Y + 13 * Game1.pixelZoom, 6 * Game1.pixelZoom, 41 * Game1.pixelZoom), Color.LimeGreen, true);

            _energyProgress = new ProgressBar(playerEnergy, playerMaxEnergy, new Rectangle(_energyBarPosition.X + 3 * Game1.pixelZoom, _energyBarPosition.Y + 13 * Game1.pixelZoom, 6 * Game1.pixelZoom, 41 * Game1.pixelZoom), Color.Lime, true);
        }

        public override void draw(SpriteBatch b)
        {
            var healthBarDims = new Rectangle(268, 408, 12, 56); 
            var energyBarDims = new Rectangle(256, 408, 12, 56);

            var healthBg = new ClickableTextureComponent("", _healthBarPosition, "", "", Game1.mouseCursors, healthBarDims, Game1.pixelZoom, false);
            var energyBg = new ClickableTextureComponent("", _energyBarPosition, "", "", Game1.mouseCursors, energyBarDims, Game1.pixelZoom, false);

            healthBg.draw(b);
            energyBg.draw(b);

            _healthProgress.draw(b);
            _energyProgress.draw(b);

            base.draw(b);
        }
    }
}