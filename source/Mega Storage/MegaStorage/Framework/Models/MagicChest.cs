using MegaStorage.Framework.Interface;
using Microsoft.Xna.Framework;
using StardewValley;

namespace MegaStorage.Framework.Models
{
    public class MagicChest : CustomChest
    {
        public override int Capacity => int.MaxValue;
        public override ChestType ChestType => ChestType.MagicChest;
        protected override LargeItemGrabMenu CreateItemGrabMenu() => new MagicItemGrabMenu(this);
        public override Item getOne() => new MagicChest(Vector2.Zero);

        public MagicChest(Vector2 tileLocation) : base(MegaStorageMod.MagicChestId, ModConfig.Instance.MagicChest, tileLocation)
        {
            name = "Magic Chest";
        }
    }
}
