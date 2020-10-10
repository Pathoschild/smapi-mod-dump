/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/Stardew-MegaStorage
**
*************************************************/

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
