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
using EnaiumToolKit.Framework.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace EnaiumToolKit.Framework.Screen.Elements;

public class Button : BaseButton
{
    public Button(string title, string? description = null) : base(title, description)
    {
    }

    public override void Render(SpriteBatch b, int x, int y)
    {
        b.DrawButtonTexture(x, y, Width, Height, Hovered ? Color.Wheat : Color.White);
        b.DrawStringCenter(Title!, x, y, Width, Height);
        base.Render(b, x, y);
    }
}