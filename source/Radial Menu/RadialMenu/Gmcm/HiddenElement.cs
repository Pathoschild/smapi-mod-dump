/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/focustense/StardewRadialMenu
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using SpaceShared.UI;

namespace RadialMenu.Gmcm;

internal class HiddenElement : Element
{
    internal Element OriginalElement => originalElement;

    public override int Width => 0;

    public override int Height => 0;

    private readonly Element originalElement;

    public HiddenElement(Element originalElement)
    {
        this.originalElement = originalElement;
        LocalPosition = originalElement.LocalPosition;
    }

    public override void Draw(SpriteBatch b)
    {
    }

    public override void Update(bool isOffScreen = false)
    {
    }
}
