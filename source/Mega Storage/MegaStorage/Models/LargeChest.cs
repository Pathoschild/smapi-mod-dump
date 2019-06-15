using MegaStorage.UI;
using StardewValley;

namespace MegaStorage.Models
{
    public class LargeChest : NiceChest
    {
        public override string ItemName => "Large Chest";
        public override string Description => "A large place to store your items.";
        public override int Capacity => 72;
        public override int ItemId => 816;
        public override ChestType ChestType => ChestType.LargeChest;
        public override string SpritePath => "Resources/MagicChest.png";
        public override string RecipeString => $"{ItemIds.Wood} 100 {ItemIds.CopperBar} 1 {ItemIds.IronBar} 1/Home/{ItemId}/true/null";
        public override string BigCraftableInfo => $"{ItemName}/0/-300/Crafting -9/{Description}/true/true/0";

        protected override LargeItemGrabMenu CreateItemGrabMenu() => new LargeItemGrabMenu(this);
        public override Item getOne() => new LargeChest();
    }
}
