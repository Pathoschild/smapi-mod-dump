/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class DyeOption : BaseOption
{
    public DyeOption(Rectangle sourceRect) :
        base(I18n.Option_Dye(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.eventsSeen.Contains("992559"))
            Game1.activeClickableMenu = new DyeMenu();
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}