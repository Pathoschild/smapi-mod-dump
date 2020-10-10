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
