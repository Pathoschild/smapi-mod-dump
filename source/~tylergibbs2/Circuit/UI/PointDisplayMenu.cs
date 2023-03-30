/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace Circuit.UI
{
    public class PointDisplayMenu : IClickableMenu
    {
        public int Points { get; private set; } = 0;

        private bool IsAddingPoints { get; set; } = false;

        private int AddingPointsDropdownFrame { get; set; } = 0;

        private MoneyDial Dial { get; } = new(3);

        public PointDisplayMenu(int startingAmount) : base()
        {
            Points = startingAmount;
            Dial.currentValue = startingAmount;

            CalculatePositions();
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);

            CalculatePositions();
        }

        private void CalculatePositions()
        {
            width = 106;
            height = 64;

            xPositionOnScreen = Game1.uiViewport.Width - width - 16;
            yPositionOnScreen = DayTimeMoneyBox.height + 32;
        }

        public void AddPoints(int amount)
        {
            IsAddingPoints = true;
            AddingPointsDropdownFrame = 0;
            Points += amount;
        }

        public override void update(GameTime time)
        {
            base.update(time);
        }

        private void DrawPointsBox(SpriteBatch b)
        {
            drawTextureBox(
                b,
                Game1.menuTexture,
                new Rectangle(0, 256, 60, 60),
                xPositionOnScreen,
                yPositionOnScreen,
                width,
                height,
                Color.White,
                drawShadow: false
            );
        }

        private void DrawPoints(SpriteBatch b)
        {
            Dial.draw(b, new(xPositionOnScreen + 18, yPositionOnScreen + 18), Points);
        }

        private void DrawPointsDropdown(SpriteBatch b)
        {
            if (!IsAddingPoints)
                return;
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);

            DrawPointsBox(b);
            DrawPoints(b);

            DrawPointsDropdown(b);
            drawMouse(b);
        }
    }
}
