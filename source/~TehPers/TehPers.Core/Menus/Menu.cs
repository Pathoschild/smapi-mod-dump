using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using TehPers.Core.Enums;
using TehPers.Core.Menus.BoxModel;
using TehPers.Core.Menus.Elements;

namespace TehPers.Core.Menus {

    public class Menu : IClickableMenu {
        public MenuElement MainElement { get; } = new MenuElement();

        public Menu(int x, int y, int width, int height) : base(x, y, width, height) {
            this.MainElement.Bounds = new BoxRectangle(BoxVector.Zero, BoxVector.Fill);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true) {
            if (!this.MainElement.Click(new Vector2I(x, y), MouseButtons.LEFT, this.GetBounds()))
                base.receiveLeftClick(x, y, playSound);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true) {
            this.MainElement.Click(new Vector2I(x, y), MouseButtons.RIGHT, this.GetBounds());
        }

        public override void receiveScrollWheelAction(int direction) {
            this.MainElement.Scroll(new Vector2I(Game1.getMouseX(), Game1.getMouseY()), direction, this.GetBounds());
        }

        public override void receiveKeyPress(Keys key) {
            Element focused = this.MainElement.GetFocusedElement() ?? this.MainElement;
            if (!focused.PressKey(key))
                base.receiveKeyPress(key);
        }

        public override void leftClickHeld(int x, int y) {
            if (!this.MainElement.Drag(new Vector2I(x, y), this.GetBounds()))
                base.leftClickHeld(x, y);
        }

        public void EnterText(string text) {
            Element focused = this.MainElement.GetFocusedElement() ?? this.MainElement;
            focused.EnterText(text);
        }

        public override void performHoverAction(int x, int y) {
            //this._hoverText = "";
            //this._hoverTitle = "";
        }

        protected virtual void DrawHoverText(SpriteBatch batch) {
            //IClickableMenu.drawHoverText(b, this._hoverText, Game1.smallFont, 0, 0, -1, this._hoverTitle.Length > 0 ? this._hoverTitle : null);
        }

        public override void draw(SpriteBatch batch) {
            if (this.MainElement == null)
                return;

            using (SpriteBatch menuBatch = new SpriteBatch(Game1.graphics.GraphicsDevice)) {
                menuBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
                this.MainElement.Draw(batch, this.GetBounds());
                this.MainElement.DrawCursor(batch);
                menuBatch.End();
            }
        }

        private Rectangle2I GetBounds() {
            return new Rectangle2I(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
        }
    }
}