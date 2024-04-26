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

namespace ActiveMenuAnywhere.Framework.Options;

public class LolaOption : BaseOption
{
    public LolaOption(Rectangle sourceRect) :
        base(I18n.Option_Lola(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.eventsSeen.Contains("75160093"))
            Utility.TryOpenShopMenu("RSVLolaShop", "Lola");
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}