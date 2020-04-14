using Microsoft.Xna.Framework;
using StardewValley;

namespace MegaStorage.Framework.Models
{
    public class LargeChest : CustomChest
    {
        public override int Capacity => 72;
        public override Item getOne() => new LargeChest(Vector2.Zero);
        public LargeChest() : this(Vector2.Zero) { }
        public LargeChest(Vector2 tileLocation)
            : base(
                ChestType.LargeChest,
                tileLocation)
        {
            name = "Large Chest";
        }
    }
}
