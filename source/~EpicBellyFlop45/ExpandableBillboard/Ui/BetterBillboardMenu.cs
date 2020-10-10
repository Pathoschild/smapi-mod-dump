/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EpicBellyFlop45/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;

namespace ExpandableBillboard.Ui
{
    public class BetterBillboardMenu : IClickableMenu
    {
        private Texture2D BillboardBackground;

        public BetterBillboardMenu()
        {
            // load assets
            BillboardBackground = ModEntry.ModHelper.Content.Load<Texture2D>("Assets/BillboardBackground.png", ContentSource.ModFolder);

            // get the top left position for the background asset (pass it *4 as that's the scale)
            Vector2 backgroundTopLeftPosition = Utility.getTopLeftPositionForCenteringOnScreen(BillboardBackground.Width * 4, BillboardBackground.Height * 4);
            this.xPositionOnScreen = (int)backgroundTopLeftPosition.X;
            this.yPositionOnScreen = (int)backgroundTopLeftPosition.Y;
        }

        public override void draw(SpriteBatch b)
        {
            // dark background
            b.Draw(
                texture: Game1.fadeToBlackRect,
                destinationRectangle: Game1.graphics.GraphicsDevice.Viewport.Bounds,
                color: Color.Black * 0.75f
            );

            // billboard background
            b.Draw(
                texture: BillboardBackground,
                position: new Vector2(this.xPositionOnScreen, this.yPositionOnScreen),
                sourceRectangle: new Rectangle(0, 0, BillboardBackground.Width, BillboardBackground.Height),
                color: Color.White,
                rotation: 0,
                origin: Vector2.Zero,
                scale: 4,
                effects: SpriteEffects.None,
                layerDepth: 1
            );

            // cursor
            this.drawMouse(b);
        }
    }
}
