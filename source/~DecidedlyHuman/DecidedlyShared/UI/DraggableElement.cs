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

namespace DecidedlyShared.Ui;

public class DraggableElement : UiElement
{
    private bool currentlyBeingDragged;
    private UiElement dragArea;

    public DraggableElement(string name, Rectangle bounds, Logger logger, DrawableType type = DrawableType.Texture,Texture2D? texture = null, Rectangle? sourceRect = null,
        Color? color = null)
        : base(name, bounds, logger, type, texture, sourceRect, color)
    {
    }
}
