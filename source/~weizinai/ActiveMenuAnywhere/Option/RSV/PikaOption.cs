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

internal class PikaOption : BaseOption
{
    public PikaOption(Rectangle sourceRect) :
        base(I18n.Option_Pika(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        var options = new List<Response>
        {
            new("Shop", I18n.PikaOption_Shop()),
            new("RecipeShop", I18n.PikaOption_RecipeShop()),
            new("Leave", I18n.BaseOption_Leave())
        };
        Game1.currentLocation.createQuestionDialogue("", options.ToArray(), this.AfterDialogueBehavior);
    }

    private void AfterDialogueBehavior(Farmer who, string whichAnswer)
    {
        switch (whichAnswer)
        {
            case "Shop":
                Utility.TryOpenShopMenu("RSVPikaShop", "Pika");
                break;
            case "RecipeShop":
                Utility.TryOpenShopMenu("RSVPikaRecipes", "Pika");
                break;
            case "Leave":
                Game1.exitActiveMenu();
                Game1.player.forceCanMove();
                break;
        }
    }
}