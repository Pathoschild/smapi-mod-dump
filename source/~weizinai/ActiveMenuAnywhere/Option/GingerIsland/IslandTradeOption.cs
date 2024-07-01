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
using StardewValley.Locations;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class IslandTradeOption : BaseOption
{
    public IslandTradeOption(Rectangle sourceRect) :
        base(I18n.Option_IslandTrade(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.RequireLocation<IslandNorth>("IslandNorth").traderActivated.Value)
            Utility.TryOpenShopMenu("IslandTrade", null, true);
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}