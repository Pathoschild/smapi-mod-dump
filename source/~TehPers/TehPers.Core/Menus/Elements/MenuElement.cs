using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using TehPers.Core.Helpers.Static;
using TehPers.Core.Menus.BoxModel;

namespace TehPers.Core.Menus.Elements {
    public class MenuElement : Element {
        public virtual bool StopKeyPropagation { get; set; } = false;

        public MenuElement() {
            this.Padding = new OuterSize(Game1.tileSize);
        }

        protected virtual void DrawBackground(SpriteBatch batch, Rectangle2I parentBounds) {
            batch.DrawMenuBox(this.Bounds.ToAbsolute(parentBounds).ToRectangle(), this.GetGlobalDepth(0));
        }

        public virtual void DrawCursor(SpriteBatch batch) {
            if (Game1.options.hardwareCursor)
                return;
            batch.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.SnappyMenus ? 44 : 0, 16, 16), Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 0.1f);
        }

        protected override void OnDraw(SpriteBatch batch, Rectangle2I parentBounds) {
            this.DrawBackground(batch, parentBounds);
        }

        protected override bool OnKeyPressed(Keys key) => this.StopKeyPropagation;
    }
}
