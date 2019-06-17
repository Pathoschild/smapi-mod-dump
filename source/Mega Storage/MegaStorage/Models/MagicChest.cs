using MegaStorage.UI;
using StardewValley;

namespace MegaStorage.Models
{
    public class MagicChest : NiceChest
    {
        public override string ItemName => "Magic Chest";
        public override string Description => "A magical place to store your items.";
        public override int Capacity => int.MaxValue;
        public override int ItemId => 824;
        public override ChestType ChestType => ChestType.MagicChest;
        public override string SpritePath => "Resources/MagicChest.png";
        public override string RecipeString => $"{Config.Instance.MagicChestRecipe}/Home/{ItemId}/true/null";
        public override string BigCraftableInfo => $"{ItemName}/0/-300/Crafting -9/{Description}/true/true/0";

        protected override LargeItemGrabMenu CreateItemGrabMenu() => new MagicItemGrabMenu(this);
        public override Item getOne() => new MagicChest();
    }
}
