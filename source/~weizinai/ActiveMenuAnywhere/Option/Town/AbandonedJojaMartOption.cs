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

internal class AbandonedJojaMartOption : BaseOption
{
    public AbandonedJojaMartOption(Rectangle sourceRect) :
        base(I18n.Option_AbandonedJojaMart(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.MasterPlayer.mailReceived.Contains("abandonedJojaMartAccessible"))
        {
            var abandonedJojaMart = Game1.RequireLocation<AbandonedJojaMart>("AbandonedJojaMart");
            abandonedJojaMart.checkBundle();
        }
        else
        {
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
        }
    }
}