using MegaStorage.Framework.Interface;
using Microsoft.Xna.Framework;
using StardewValley;

namespace MegaStorage.Framework.Models
{
    public class SuperMagicChest : CustomChest
    {
        public override int Capacity => int.MaxValue;
        public override ChestType ChestType => ChestType.SuperMagicChest;
        protected override LargeItemGrabMenu CreateItemGrabMenu() => new SuperMagicItemGrabMenu(this);
        public override Item getOne() => new SuperMagicChest(Vector2.Zero);

        public SuperMagicChest(Vector2 tileLocation) : base(MegaStorageMod.SuperMagicChestId, ModConfig.Instance.SuperMagicChest, tileLocation)
        {
            name = "Super Magic Chest";
        }
    }
}
