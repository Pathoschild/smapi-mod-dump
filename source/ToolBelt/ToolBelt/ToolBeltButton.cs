/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/yuri0r/toolbelt
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;

namespace ToolBelt
{
    public class ToolBeltButton : ClickableTextureComponent
    {
        private Tool tool;
        private int inventoryIndex;
        private float initScale;
        private float selectedScale;

        public ToolBeltButton(Rectangle bounds, Texture2D texture, Rectangle sourceRect, float scale, int inventoryIndex, Tool tool, bool drawShadow = false)
            : base(bounds, texture, sourceRect, scale, drawShadow = false)
        {
            this.tool = tool;
            this.inventoryIndex = inventoryIndex;
            initScale = scale;
            selectedScale = scale * 1.2f;
        }


        public int getIndex()
        {
            return inventoryIndex;
        }

        public String toolName()
        {
            return tool.DisplayName;
        }

        public void select()
        {
            scale = selectedScale;
        }
        public void deSelect()
        {
            scale = initScale;
        }

    }
}
