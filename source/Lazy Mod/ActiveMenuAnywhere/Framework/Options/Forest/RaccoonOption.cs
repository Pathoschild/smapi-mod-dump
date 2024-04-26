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
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;

namespace ActiveMenuAnywhere.Framework.Options;

public class RaccoonOption : BaseOption
{
    private readonly IModHelper helper;

    public RaccoonOption(Rectangle sourceRect, IModHelper helper) :
        base(I18n.Option_Raccoon(), sourceRect)
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.MasterPlayer.mailReceived.Contains("raccoonMovedIn"))
        {
            var options = new List<Response>();
            var day = Game1.netWorldState.Value.Date.TotalDays - Game1.netWorldState.Value.DaysPlayedWhenLastRaccoonBundleWasFinished;
            if (day >= 7)
                options.Add(new Response("RaccoonBundle", "RaccoonBundle"));
            var mrsRaccoon = Game1.RequireLocation<Forest>("Forest").getCharacterFromName("MrsRaccoon");
            if (mrsRaccoon != null)
                options.Add(new Response("MrsRaccoonShop", "MrsRaccoonShop"));
            options.Add(new Response("Leave", "Leave"));
            Game1.currentLocation.createQuestionDialogue("", options.ToArray(), AfterDialogueBehavior);
        }
        else
        {
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
        }
    }

    private void AfterDialogueBehavior(Farmer who, string whichAnswer)
    {
        switch (whichAnswer)
        {
            case "RaccoonBundle":
                helper.Reflection.GetMethod(new Raccoon(), "_activateMrRaccoon").Invoke();
                break;
            case "MrsRaccoonShop":
                Utility.TryOpenShopMenu("Raccoon", "Raccoon");
                break;
            case "Leave":
                Game1.exitActiveMenu();
                Game1.player.forceCanMove();
                break;
        }
    }
}