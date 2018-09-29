using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace WaitAroundSMAPI
{
    internal class MenuButton : ClickableTextureComponent
    {
        public Action<MenuButton> Callback { set; get; }
        public int RelativeX { set; get; }
        public int RelativeY { set; get; }
        public Vector2 ParentMenuFactor { set; get; }

        public MenuButton(string name, int relativeX, int relativeY, int spriteWidth, int spriteHeight, Vector2 parentMenuFactor, Rectangle parentMenu, Texture2D spritesheet, int spritePosition, Action<MenuButton> callback)
            : base(
                name: name,
                label: null,
                hoverText: null,
                bounds: new Rectangle(0, 0, spriteWidth, spriteHeight),
                texture: spritesheet,
                sourceRect: Game1.getSourceRectForStandardTileSheet(spritesheet, spritePosition, spriteWidth, spriteHeight),
                scale: 1
            )
        {
            this.RelativeX = relativeX;
            this.RelativeY = relativeY;
            this.ParentMenuFactor = parentMenuFactor;
            this.Callback = callback;

            this.SetAbsolutePosition(parentMenu);
        }

        public void Draw(SpriteBatch b, Rectangle parentMenu)
        {
            this.SetAbsolutePosition(parentMenu);
            base.draw(b);
        }

        private void SetAbsolutePosition(Rectangle parentMenu)
        {
            int x = parentMenu.X + this.RelativeX + (int)Math.Floor(parentMenu.Width * this.ParentMenuFactor.X);
            if (this.RelativeX + (int)Math.Floor(parentMenu.Width * this.ParentMenuFactor.X) < 0)
                x = parentMenu.X + parentMenu.Width + this.RelativeX + (int)Math.Floor(parentMenu.Width * this.ParentMenuFactor.X);

            int y = parentMenu.Y + this.RelativeY + (int)Math.Floor(parentMenu.Height * this.ParentMenuFactor.Y);
            if (this.RelativeY + (int)Math.Floor(parentMenu.Height * this.ParentMenuFactor.Y) < 0)
                y = parentMenu.Y + parentMenu.Height + this.RelativeY + (int)Math.Floor(parentMenu.Height * this.ParentMenuFactor.Y);

            this.bounds = new Rectangle(x, y, this.bounds.Width, this.bounds.Height);
        }

    }
}
