/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/Stardew-MegaStorage
**
*************************************************/

namespace MegaStorage.Framework.Models
{
    public class DeserializedChest
    {
        public string Name { get; set; }
        public long PlayerId { get; set; }
        public string LocationName { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public int InventoryIndex { get; set; }
        public ChestType ChestType { get; set; }
    }
}
