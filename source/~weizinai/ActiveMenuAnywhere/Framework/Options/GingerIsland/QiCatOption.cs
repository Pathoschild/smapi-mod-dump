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
using StardewValley.Locations;

namespace ActiveMenuAnywhere.Framework.Options;

internal class QiCatOption : BaseOption
{
    private readonly IModHelper helper;

    public QiCatOption(Rectangle sourceRect, IModHelper helper) :
        base(I18n.Option_QiCat(), sourceRect)
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        var isQiWalnutRoomDoorUnlocked = IslandWest.IsQiWalnutRoomDoorUnlocked(out _);
        if (isQiWalnutRoomDoorUnlocked)
            helper.Reflection.GetMethod(new GameLocation(), "ShowQiCat").Invoke();
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}