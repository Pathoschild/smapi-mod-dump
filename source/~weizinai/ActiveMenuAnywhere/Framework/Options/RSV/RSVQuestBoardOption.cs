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
using StardewModdingAPI;

namespace ActiveMenuAnywhere.Framework.Options;

internal class RSVQuestBoardOption : BaseOption
{
    private readonly IModHelper helper;

    public RSVQuestBoardOption(Rectangle sourceRect, IModHelper helper) :
        base(I18n.Option_RSVQuestBoard(), sourceRect)
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        // var questController = RSVIntegration.GetType("RidgesideVillage.Questing.QuestController");
        // object[] parameters = { Game1.currentLocation, new[] { "VillageQuestBoard" }, Game1.player, new Point() };
        // questController?.GetMethod("OpenQuestBoard", BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null, parameters);
    }
}