using StardewValley.Menus;

namespace Walkshop
{
    public class ShopData
    {

        public string name { get; }
        public IClickableMenu iClickableMenu { get; }

        public ShopData(string name, IClickableMenu iClickableMenu)
        {
            this.name = name;
            this.iClickableMenu = iClickableMenu;
        }
    }
}