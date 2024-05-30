/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/TeleportNpc
**
*************************************************/

using EnaiumToolKit.Framework.Screen;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Monsters;
using TeleportNpc.Framework.Screen.Elements;

namespace TeleportNpc.Framework.Gui;

public class TeleportNpcScreen : ScreenGui
{
    public TeleportNpcScreen()
    {
        foreach (var npc in Utility.getAllCharacters().Where(it => it is not Monster).OrderByDescending(it =>
                 {
                     if (!it.CanReceiveGifts()) return it is Horse or Child or Pet ? int.MaxValue : -1;
                     return Game1.player.friendshipData.TryGetValue(it.Name, out var data) ? data.Points : 0;
                 }))
        {
            AddElement(new NpcButton(
                $"{ModEntry.GetInstance().Helper.Translation.Get("button.teleport")}{npc.displayName}",
                $"{ModEntry.GetInstance().Helper.Translation.Get("button.teleport")}{npc.displayName}", npc)
            {
                OnLeftClicked = () =>
                {
                    Game1.exitActiveMenu();
                    Game1.warpFarmer(npc.currentLocation.Name, npc.TilePoint.X, npc.TilePoint.Y,
                        Game1.player.getFacingDirection());
                }
            });
        }
    }
}