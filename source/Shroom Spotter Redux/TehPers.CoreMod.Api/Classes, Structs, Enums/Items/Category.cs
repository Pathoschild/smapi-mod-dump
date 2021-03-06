/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using TehPers.CoreMod.Api.Drawing;

namespace TehPers.CoreMod.Api.Items {
    public readonly struct Category {
        public int Index { get; }
        public string Name { get; }
        public string DataSource { get; }

        public Category(int index, string name, string dataSource) {
            this.Index = index;
            this.Name = name;
            this.DataSource = dataSource;
        }

        public Category(int index, string name) : this(index, name, "Data/ObjectInformation") { }

        public override string ToString() {
            return string.IsNullOrEmpty(this.Name) ? this.Index.ToString() : $"{this.Name} {this.Index}";
        }

        #region Vanilla Categories
        public static Category Gem { get; } = new Category(-2, "Minerals");
        public static Category Fish { get; } = new Category(-4, "Fish");
        public static Category Egg { get; } = new Category(-5, "Basic");
        public static Category Milk { get; } = new Category(-6, "Basic");
        public static Category Cooking { get; } = new Category(-7, "Cooking");
        public static Category Crafting { get; } = new Category(-8, "Crafting");
        public static Category BigCraftable { get; } = new Category(-9, "Crafting", "Data/BigCraftablesInformation");
        public static Category Mineral { get; } = new Category(-12, "Minerals");
        public static Category Meat { get; } = new Category(-14, null);
        public static Category Metal { get; } = new Category(-15, "Basic");
        public static Category BuildingMaterial { get; } = new Category(-16, "Basic");
        public static Category SellAtPierres { get; } = new Category(-17, "Basic");
        public static Category SellAtPierresAndMarnies { get; } = new Category(-18, "Basic");
        public static Category Fertilizer { get; } = new Category(-19, "Basic");
        public static Category Trash { get; } = new Category(-20, "Basic");
        public static Category Bait { get; } = new Category(-21, "Basic");
        public static Category FishingTackle { get; } = new Category(-22, "Basic");
        public static Category SellAtFishShop { get; } = new Category(-23, "Basic");
        public static Category Furniture { get; } = new Category(-24, "Crafting");
        public static Category Ingredient { get; } = new Category(-25, null);
        public static Category ArtisanGoods { get; } = new Category(-26, "Basic");
        public static Category Syrup { get; } = new Category(-27, "Basic");
        public static Category MonsterLoot { get; } = new Category(-28, "Basic");
        public static Category Equipment { get; } = new Category(-29, null);
        public static Category Seed { get; } = new Category(-74, "Seeds");
        public static Category Vegetable { get; } = new Category(-75, "Basic");
        public static Category Fruit { get; } = new Category(-79, "Basic");
        public static Category Flower { get; } = new Category(-80, "Basic");
        public static Category Forage { get; } = new Category(-81, "Basic");
        public static Category Hat { get; } = new Category(-95, null);
        public static Category Ring { get; } = new Category(-96, null);
        public static Category Weapon { get; } = new Category(-98, null);
        public static Category Tool { get; } = new Category(-99, null);
        #endregion
    }
}