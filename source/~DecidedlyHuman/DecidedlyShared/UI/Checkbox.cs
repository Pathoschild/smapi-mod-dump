/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using DecidedlyShared.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace DecidedlyShared.Ui;

public class Checkbox : UiElement
{


    public Checkbox(string name, Rectangle bounds, Logger logger, DrawableType type = DrawableType.Texture, Texture2D texture = null, Rectangle? sourceRect = null, Color? color = null, bool drawShadow = false, int topEdgeSize = 16, int bottomEdgeSize = 12, int leftEdgeSize = 12, int rightEdgeSize = 16, int scale = 4) : base(name, bounds, logger, type, texture, sourceRect, color, drawShadow, topEdgeSize, bottomEdgeSize, leftEdgeSize, rightEdgeSize, scale)
    {

    }
}
