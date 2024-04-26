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

public class MagicBoatOption: BaseOption
{
    public MagicBoatOption(Rectangle sourceRect) : base("MagicBoat", sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Utility.IsPassiveFestivalDay("NightMarket"))
             Utility.TryOpenShopMenu("Festival_NightMarket_MagicBoat_Day"+Utility.GetDayOfPassiveFestival("NightMarket"), null, false);
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}