/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cheesysteak/stardew-steak
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;

namespace MoreMultiplayerInfo
{
    public class PlayerEquipmentInfo : IClickableMenu
    {
        public PlayerEquipmentInfo(Farmer player, Vector2 position)
        {
            var inventory = new InventoryPage(0, 0, 0, 0);

            inventory.equipmentIcons = null;
        }
    }
}