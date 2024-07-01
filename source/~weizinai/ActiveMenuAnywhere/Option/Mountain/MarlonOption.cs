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

internal class MarlonOption : BaseOption
{
    public MarlonOption(Rectangle sourceRect) :
        base(I18n.Option_Marlon(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("guildMember"))
            this.Marlon();
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }

    private void Marlon()
    {
        if (Game1.player.itemsLostLastDeath.Count > 0)
        {
            var options = new List<Response>
            {
                new("Shop", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Shop")),
                new("Recovery", Game1.content.LoadString("Strings\\Locations:AdventureGuild_ItemRecovery")),
                new("Leave", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Leave"))
            };
            Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:AdventureGuild_Greeting"),
                options.ToArray(), "adventureGuild");
        }
        else
        {
            Utility.TryOpenShopMenu("AdventureShop", "Marlon");
        }
    }
}