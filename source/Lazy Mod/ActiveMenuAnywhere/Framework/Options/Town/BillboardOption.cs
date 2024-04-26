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

namespace ActiveMenuAnywhere.Framework.Options;

public class BillboardOption : BaseOption
{
    public BillboardOption(Rectangle sourceRect) :
        base(I18n.Option_Billboard(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        var options = new List<Response>
        {
            new("Calendar", I18n.BillboardOption_Calendar()),
            new("DailyQuest", I18n.BillboardOption_DailyQuest()),
            new("Leave", I18n.BaseOption_Leave())
        };
        Game1.currentLocation.createQuestionDialogue("", options.ToArray(), AfterDialogueBehavior);
    }

    private void AfterDialogueBehavior(Farmer who, string whichAnswer)
    {
        switch (whichAnswer)
        {
            case "Calendar":
                Game1.activeClickableMenu = new Billboard();
                break;
            case "DailyQuest":
                Game1.activeClickableMenu = new Billboard(true);
                break;
            case "Leave":
                Game1.exitActiveMenu();
                Game1.player.forceCanMove();
                break;
        }
    }
}