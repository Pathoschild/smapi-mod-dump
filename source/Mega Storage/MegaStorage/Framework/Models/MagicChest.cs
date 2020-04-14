using Microsoft.Xna.Framework;
using StardewValley;

namespace MegaStorage.Framework.Models
{
    public class MagicChest : CustomChest
    {
        public override int Capacity => int.MaxValue;
        public override Item getOne() => new MagicChest(Vector2.Zero);
        public MagicChest() : this(Vector2.Zero) { }
        public MagicChest(Vector2 tileLocation)
            : base(
                ChestType.MagicChest,
                tileLocation)
        {
            name = "Magic Chest";
        }
    }
}
