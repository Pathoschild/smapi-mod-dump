using MegaStorage.UI;
using StardewValley;

namespace MegaStorage.Models
{
    public class MagicChest : CustomChest
    {
        public override int Capacity => int.MaxValue;
        public override ChestType ChestType => ChestType.MagicChest;

        protected override LargeItemGrabMenu CreateItemGrabMenu() => new MagicItemGrabMenu(this);
        public override Item getOne() => new MagicChest();

        public MagicChest() : base(ModConfig.Instance.MagicChest) { }
    }
}
