/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using System.Reflection;
using Common.Integration;
using Microsoft.Xna.Framework;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class NinjaBoardOption : BaseOption
{
    public NinjaBoardOption(Rectangle sourceRect) :
        base(I18n.Option_NinjaBoard(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.eventsSeen.Contains("75160254"))
        {
            var questController = RSVIntegration.GetType("RidgesideVillage.Questing.QuestController");
            object[] parameters = { Game1.currentLocation, new[] { "RSVNinjaBoard" }, Game1.player, new Point() };
            questController?.GetMethod("OpenQuestBoard", BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null, parameters);
        }
        else
        {
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
        }
    }
}