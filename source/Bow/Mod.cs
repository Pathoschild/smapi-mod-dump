using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace Bow
{
    public class Mod : StardewModdingAPI.Mod
    {
        public static Mod instance;

        public override void Entry(IModHelper helper)
        {
            instance = this;

            MenuEvents.MenuChanged += menuChanged;

            BowTool.Texture = helper.Content.Load<Texture2D>("bow.png");
        }

        private void menuChanged(object sender, EventArgsClickableMenuChanged args)
        {
            var menu = args.NewMenu as ShopMenu;
            if (menu == null || menu.portraitPerson?.Name != "Marlon" )
                return;
            
            Log.trace($"Adding bow to Marlon's shop.");

            var forSale = Helper.Reflection.GetField<List<Item>>(menu, "forSale").GetValue();
            var itemPriceAndStock = Helper.Reflection.GetField<Dictionary<Item, int[]>>(menu, "itemPriceAndStock").GetValue();

            var bow = new BowTool();
            forSale.Add(bow);
            itemPriceAndStock.Add(bow, new int[] { 2500, 1 });
        }
    }
}
