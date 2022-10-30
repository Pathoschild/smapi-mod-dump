/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace DecidedlyShared.Ui;

public abstract class VBoxUiElement : UiElement
{
    private List<UiElement> childElements;
    internal bool resizeToFitElements;
    internal int maximumHeight;

    public virtual void Draw(SpriteBatch sb)
    {
        base.Draw(sb);

        foreach (UiElement child in this.childElements)
        {

        }
    }

}
