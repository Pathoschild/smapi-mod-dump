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
using StardewModdingAPI;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class RSVSpecialOrderOption : BaseOption
{
    private readonly IModHelper helper;

    public RSVSpecialOrderOption(Rectangle sourceRect, IModHelper helper) :
        base(I18n.Option_RSVSpecialOrder(), sourceRect)
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.MasterPlayer.eventsSeen.Contains("75160207"))
        {
            var questController = RSVIntegration.GetType("RidgesideVillage.Questing.QuestController");
            object[] parameters = { Game1.currentLocation, new[] { "RSVTownSO" }, Game1.player, Point.Zero };
            questController?.GetMethod("OpenSOBoard", BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null, parameters);
        }
        else
        {
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
        }
    }
}