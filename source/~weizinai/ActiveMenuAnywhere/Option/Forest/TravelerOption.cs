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
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class TravelerOption : BaseOption
{
    public TravelerOption(Rectangle sourceRect) :
        base(I18n.Option_Traveler(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        var shouldTravelingMerchantVisitToday = Game1.dayOfMonth % 7 % 5 == 0;
        if (shouldTravelingMerchantVisitToday)
            Utility.TryOpenShopMenu("Traveler", null, true);
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}