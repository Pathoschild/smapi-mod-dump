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
using StardewValley.Menus;

namespace ActiveMenuAnywhere.Framework.Options;

public class QiSpecialOrderOption : BaseOption
{
    public QiSpecialOrderOption(Rectangle sourceRect) :
        base(I18n.Option_QiSpecialOrder(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        var isQiWalnutRoomDoorUnlocked = IslandWest.IsQiWalnutRoomDoorUnlocked(out _);
        if (isQiWalnutRoomDoorUnlocked)
            Game1.activeClickableMenu = new SpecialOrdersBoard("Qi");
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}