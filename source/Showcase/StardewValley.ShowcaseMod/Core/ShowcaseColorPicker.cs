using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace Igorious.StardewValley.ShowcaseMod.Core
{
    public class ShowcaseColorPicker : DiscreteColorPicker
    {
        private Showcase Showcase { get; }

        public ShowcaseColorPicker(int xPosition, int yPosition, Showcase showcase) : base(xPosition, yPosition, 0, showcase)
        {
            Showcase = showcase;
            colorSelection = getSelectionFromColor(showcase.Color);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            Showcase.Color = getColorFromSelection(colorSelection);
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
            Showcase.Draw(b, 1, new Vector2(xPositionOnScreen + width + borderWidth / 2, yPositionOnScreen - Game1.pixelZoom * 4), Game1.pixelZoom, 0, ShowcaseDrawMode.WithoutItems);
        }
    }
}