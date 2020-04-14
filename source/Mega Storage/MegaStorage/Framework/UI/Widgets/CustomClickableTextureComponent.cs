using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;

namespace MegaStorage.Framework.UI.Widgets
{
    internal class CustomClickableTextureComponent : ClickableTextureComponent
    {
        // Custom actions for widgets
        public Action<SpriteBatch, CustomClickableTextureComponent> DrawAction;
        public Action<CustomClickableTextureComponent> LeftClickAction;
        public Action<CustomClickableTextureComponent> RightClickAction;
        public Action<int, CustomClickableTextureComponent> ScrollAction;
        public Action<int, int, CustomClickableTextureComponent> HoverAction;

        protected CustomInventoryMenu ParentMenu;
        protected Vector2 Offset;

        public CustomClickableTextureComponent(
            string name,
            CustomInventoryMenu parentMenu,
            Vector2 offset,
            Texture2D texture,
            Rectangle sourceRect,
            string hoverText = "",
            int width = Game1.tileSize,
            int height = Game1.tileSize,
            float scale = Game1.pixelZoom)
            : base(name, new Rectangle(parentMenu.xPositionOnScreen + (int)offset.X, parentMenu.yPositionOnScreen + (int)offset.Y, width, height), "", hoverText, texture, sourceRect, scale)
        {
            ParentMenu = parentMenu;
            Offset = offset;
        }

        public void GameWindowSizeChanged()
        {
            bounds.X = ParentMenu.xPositionOnScreen + (int)Offset.X;
            bounds.Y = ParentMenu.yPositionOnScreen + (int)Offset.Y;
        }
    }
}
