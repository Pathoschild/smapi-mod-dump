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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SARDIS.CustomObjects;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SARDIS;

public class ModEntry : Mod
{
    private Warp sardisWarp;
    private static Texture2D sardisExterior;

    public static Texture2D SardisExterior
    {
        get => sardisExterior;
    }

    public override void Entry(IModHelper helper)
    {
        helper.Events.Input.ButtonPressed += this.InputOnButtonPressed;
        sardisExterior = helper.ModContent.Load<Texture2D>("assets/SARDISExteriorTexture.png");
    }

    private void InputOnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (e.IsDown(SButton.F5))
        {
        }

        if (e.IsDown(SButton.F6))
        {
            Game1.player.addItemToInventory(new SardisObject());
        }

        if (e.IsDown(SButton.OemSemicolon))
        {
        }

        if (e.IsDown(SButton.OemOpenBrackets))
        {
        }

        if (e.IsDown(SButton.OemComma))
        {
        }
    }

    private void MoveBuilding()
    {
    }
}
