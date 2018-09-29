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