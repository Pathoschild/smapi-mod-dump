/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Framework.Borders;
using AchtuurCore.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Framework.GUI;
internal class MenuButton : Button
{
    private static Texture2D MenuButtonTexture;
    public Texture2D Icon;
    public MenuButton()
    {
        LoadMenuButtonAsset();
        Texture = MenuButtonTexture;
        Scale = 1f;
    }

    public override void Draw(SpriteBatch sb, Vector2 position, Vector2? scale = null, Color? color = null)
    {
        Vector2 scale_real = (scale ?? new(1, 1)) * Scale;
        base.Draw(sb, position, scale_real, color);
        // draw icon

    }

    internal static void LoadMenuButtonAsset()
    {
        if (MenuButtonTexture is not null)
            return;

        GameAssetLoader assetLoader = new("LooseSprites/Cursors");
        assetLoader.AddAssetColor("MenuButton", 1, 23, tileSize: 16);
        assetLoader.ResizeAssets(64);
        MenuButtonTexture = assetLoader.GetAssetTextures()["MenuButton"];
    }
}
