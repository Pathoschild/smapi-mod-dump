using MegaStorage.UI;
using StardewValley;

namespace MegaStorage.Models
{
    public class LargeChest : CustomChest
    {
        public override int Capacity => 72;
        public override ChestType ChestType => ChestType.LargeChest;

        public override LargeItemGrabMenu CreateItemGrabMenu() => new LargeItemGrabMenu(this);
        public override Item getOne() => new LargeChest();

        public LargeChest() : base(ModConfig.Instance.LargeChest) { }
    }
}
