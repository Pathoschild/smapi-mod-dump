using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using System;
using Object = StardewValley.Object;

namespace MegaStorage.Framework.Interface
{
    public class MiscCategory : ChestCategory
    {
        public MiscCategory(int index, string name, Vector2 spritePos, int[] categoryIds, int x, int y) : base(index, name, spritePos, categoryIds, x, y)
        {
        }

        protected override bool BelongsToCategory(Item i)
        {
            if (i is null || string.IsNullOrWhiteSpace(i.getCategoryName()))
            {
                return true;
            }

            if (i is Object obj && !(obj.Type is null) && obj.Type.Equals("Arch", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            switch (i)
            {
                case Tool _:
                case Boots _:
                case Ring _:
                case Furniture _:
                    return true;
                default:
                    return base.BelongsToCategory(i);
            }
        }
    }
}
