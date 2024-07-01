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

internal class RSVSpecialOrderOption : BaseOption
{
    public RSVSpecialOrderOption(Rectangle sourceRect) :
        base(I18n.Option_RSVSpecialOrder(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.MasterPlayer.eventsSeen.Contains("75160207"))
        {
            var method = RSVReflection.GetRSVPrivateStaticMethod("RidgesideVillage.Questing.QuestController", "OpenSpecialOrderBoard");
            var parameters = new object[] { Game1.currentLocation, new[] { "RSVTownSO" }, Game1.player, Point.Zero };
            method.Invoke(null, parameters);
        }
        else
        {
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
        }
    }
}