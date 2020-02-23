using MegaStorage.Framework.Interface;
using Microsoft.Xna.Framework;
using StardewValley;

namespace MegaStorage.Framework.Models
{
    public class LargeChest : CustomChest
    {
        public override int Capacity => 72;
        public override ChestType ChestType => ChestType.LargeChest;
        protected override LargeItemGrabMenu CreateItemGrabMenu() => new LargeItemGrabMenu(this);
        public override Item getOne() => new LargeChest(Vector2.Zero);

        public LargeChest(Vector2 tileLocation) : base(MegaStorageMod.LargeChestId, ModConfig.Instance.LargeChest, tileLocation)
        {
            name = "Large Chest";
        }
    }
}
