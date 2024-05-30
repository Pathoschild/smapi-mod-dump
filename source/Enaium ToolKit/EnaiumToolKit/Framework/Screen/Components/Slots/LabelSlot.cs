/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/EnaiumToolKit
**
*************************************************/

using EnaiumToolKit.Framework.Extensions;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace EnaiumToolKit.Framework.Screen.Components.Slots;

public class LabelSlot : Slot<LabelSlot>.Entry
{
    private string Title;

    public LabelSlot(string title)
    {
        Title = title;
    }

    public override void Render(SpriteBatch b, int x, int y)
    {
        Hovered = new Rectangle(x, y, Width, Height).Contains(Game1.getMouseX(), Game1.getMouseY());
        b.DrawStringCenter(Title, x, y, Width, Height);
    }
}