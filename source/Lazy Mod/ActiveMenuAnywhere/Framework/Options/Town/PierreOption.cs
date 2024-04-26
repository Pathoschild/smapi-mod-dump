/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using StardewValley;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace ActiveMenuAnywhere.Framework.Options;

public class PierreOption : BaseOption
{
    public PierreOption(Rectangle sourceRect) :
        base(I18n.Option_Pierre(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        var options = new List<Response>
        {
            new("SeedShop", I18n.PierreOption_SeedShop()),
            new("BuyBackpack", I18n.PierreOption_BuyBackpack()),
            new("Leave", I18n.BaseOption_Leave())
        };
        Game1.currentLocation.createQuestionDialogue("", options.ToArray(), AfterQuestionBehavior);
    }

    private void AfterQuestionBehavior(Farmer who, string whichAnswer)
    {
        switch (whichAnswer)
        {
            case "SeedShop":
                Utility.TryOpenShopMenu("SeedShop", "Pierre");
                break;
            case "BuyBackpack":
                Game1.currentLocation.performAction("BuyBackpack", Game1.player, new Location());
                break;
            case "Leave":
                Game1.exitActiveMenu();
                Game1.player.forceCanMove();
                break;
        }
    }
}