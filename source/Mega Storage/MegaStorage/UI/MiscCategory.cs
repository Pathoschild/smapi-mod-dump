using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;

namespace MegaStorage.UI
{
    public class MiscCategory : ChestCategory
    {
        public MiscCategory(int index, string name, Vector2 spritePos, int[] categoryIds, int x, int y) : base(index, name, spritePos, categoryIds, x, y)
        {
        }

        protected override bool BelongsToCategory(Item i)
        {
            if (i.getCategoryName() == "" || i is Object obj && obj.Type == "Arch")
                return true;
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
