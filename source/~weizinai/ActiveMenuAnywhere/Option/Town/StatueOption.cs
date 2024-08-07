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

internal class StatueOption : BaseOption
{
    public StatueOption(Rectangle sourceRect) :
        base(I18n.Option_Statue(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.hasRustyKey)
            this.Statue();
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }

    private void Statue()
    {
        var location = Game1.currentLocation;
        location.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatue"),
            location.createYesNoResponses(), "dogStatue");
    }
}