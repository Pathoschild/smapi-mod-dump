/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Brandon22Adams/ToolPouch
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ToolPouch
{
    public class ToolPouchButton : ClickableTextureComponent
    {
        private Item tool;
        private int inventoryIndex;
        private float initScale;
        private float selectedScale;
        private static int assetSize = 70;
        private int assetOffset = 3;
        private float depth = 0.86f;
        IModHelper helper;


        public ToolPouchButton(int inventoryIndex, Item tool, IModHelper helper, bool drawShadow = false)
            : base(new Rectangle(0, 0, assetSize, assetSize), null, new Rectangle(0, 0, assetSize, assetSize), 1f, drawShadow)
        {

            scale = 1f;
            this.tool = tool;
            this.inventoryIndex = inventoryIndex;
            this.helper = helper;
            initScale = scale;
            selectedScale = scale * 1.2f;
            this.deSelect();
        }


        public int getIndex()
        {
            return inventoryIndex;
        }

        public String toolName()
        {
            if(tool == null)
            {
                return "Empty";
            }
            return tool.DisplayName;
        }

        public void draw(SpriteBatch b, float transparancy, bool useBackdrop)
        {
            if (useBackdrop) base.draw(b, Color.White * transparancy, depth);
            Vector2 vector = getVector2();
            vector.X += assetOffset;
            vector.Y += assetOffset;
            if(tool != null)
            {
                tool.drawInMenu(b, vector, useBackdrop ? scale * 0.9f : scale, transparancy, depth);
            }
        }

        public void select()
        {
            texture = helper.ModContent.Load<Texture2D>("assets\\selected.png");
            scale = selectedScale;
        }
        public void deSelect()
        {
            texture = helper.ModContent.Load<Texture2D>("assets\\unselected.png");
            scale = initScale;
        }

    }
}
