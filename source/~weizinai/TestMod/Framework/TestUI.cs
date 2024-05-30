/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TestMod.Framework;

public class TestUI
{
    private readonly RootElement ui = new();

    public TestUI()
    {
        var text = new Text("Hello, world!", new Vector2(100, 100));
        var button = new Button(I18n.UI_Button_Name(), new Vector2(200, 200))
        {
            OnHover = (element, _) => (element as Button)!.TextureColor = Color.White * 0.5f,
            OffHover = element => (element as Button)!.TextureColor = Color.White
        };
        ui.AddChild(text, button);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        ui.Draw(spriteBatch);
        ui.PerformHoverAction(spriteBatch);
    }

    public void Update()
    {
        ui.Update();
    }
}