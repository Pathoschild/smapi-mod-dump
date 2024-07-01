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

internal class CommunityCenterOption : BaseOption
{
    private readonly List<string> keys;
    private readonly List<string> texts;

    public CommunityCenterOption(Rectangle sourceRect) :
        base(I18n.Option_CommunityCenter(), sourceRect)
    {
        this.keys = new List<string> { "Pantry", "CraftsRoom", "FishTank", "BoilerRoom", "Vault", "Bulletin" };
        this.texts = new List<string>
        {
            Game1.content.LoadString("Strings\\Locations:CommunityCenter_AreaName_Pantry"),
            Game1.content.LoadString("Strings\\Locations:CommunityCenter_AreaName_CraftsRoom"),
            Game1.content.LoadString("Strings\\Locations:CommunityCenter_AreaName_FishTank"),
            Game1.content.LoadString("Strings\\Locations:CommunityCenter_AreaName_BoilerRoom"),
            Game1.content.LoadString("Strings\\Locations:CommunityCenter_AreaName_Vault"),
            Game1.content.LoadString("Strings\\Locations:CommunityCenter_AreaName_BulletinBoard")
        };
    }

    public override void ReceiveLeftClick()
    {
        if (!Game1.player.mailReceived.Contains("JojaMember") && Game1.player.mailReceived.Contains("canReadJunimoText"))
            this.CheckBundle();
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }

    private void CheckBundle()
    {
        var communityCenter = Game1.RequireLocation<CommunityCenter>("CommunityCenter");
        var options = new List<Response>();
        for (var i = 0; i < 6; i++)
            if (communityCenter.shouldNoteAppearInArea(i))
                options.Add(new Response(this.keys[i], this.texts[i]));

        options.Add(new Response("Leave", I18n.BaseOption_Leave()));

        Game1.currentLocation.createQuestionDialogue("", options.ToArray(), this.AfterDialogueBehavior);
    }

    private void AfterDialogueBehavior(Farmer who, string whichAnswer)
    {
        if (whichAnswer == "Leave")
        {
            Game1.exitActiveMenu();
            Game1.player.forceCanMove();
        }
        else
        {
            var communityCenter = Game1.RequireLocation<CommunityCenter>("CommunityCenter");
            communityCenter.checkBundle(this.keys.IndexOf(whichAnswer));
        }
    }
}