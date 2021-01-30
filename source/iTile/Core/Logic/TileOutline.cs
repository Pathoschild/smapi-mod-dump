/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ha1fdaew/iTile
**
*************************************************/

using iTile.Core.Logic.Action;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;

namespace iTile.Core.Logic
{
    public class TileOutline
    {
        public Texture2D texture;
        public Rectangle rect;
        public TileMode tileMode;

        public TileOutline(TileMode tileMode)
        {
            this.tileMode = tileMode;
            texture = CoreManager.Instance.assetsManager.LoadTexture("TileOutline.png");
        }

        public void Draw(object sender, RenderingHudEventArgs e)
        {
            if (!tileMode.State)
                return;

            Vector2 cursorTile = Game1.currentCursorTile;
            rect = new Rectangle(
                (int)((cursorTile.X * Game1.tileSize) - Game1.viewport.X),
                (int)((cursorTile.Y * Game1.tileSize) - Game1.viewport.Y),
                Game1.tileSize, Game1.tileSize
            );
            Game1.spriteBatch.Draw(texture, rect, GetColorBasedOnAction());
        }

        private Color GetColorBasedOnAction()
        {
            ActionManager.Action action = tileMode.CurrentAction;
            if (action == ActionManager.Action.Copy)
                return new Color(255, 209, 56);
            else if (action == ActionManager.Action.Paste)
                return new Color(56, 255, 86);
            else if (action == ActionManager.Action.Delete)
                return new Color(255, 94, 94);
            return Color.White;
        }
    }
}