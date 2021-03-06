/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bpendragon/Best-of-Queen-of-Sauce
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Menus;
using SVObject = StardewValley.Object;

namespace BestOfQueenOfSauce
{
    partial class ModEntry : Mod
    {
        private Dictionary<string, int> FirstAirDate = new Dictionary<string, int>();
        private ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();
            I18n.Init(helper.Translation);

            FirstAirDate["Stir Fry"] = 7;
            FirstAirDate["Coleslaw"] = 14;
            FirstAirDate["Radish Salad"] = 21;
            FirstAirDate["Baked Fish"] = 35;
            FirstAirDate["Trout Soup"] = 70;
            FirstAirDate["Glazed Yams"] = 77;
            FirstAirDate["Artichoke Dip"] = 84;
            FirstAirDate["Plum Pudding"] = 91;
            FirstAirDate["Chocolate Cake"] = 98;
            FirstAirDate["Pumpkin Pie"] = 105;
            FirstAirDate["Cranberry Candy"] = 112;
            FirstAirDate["Complete Breakfast"] = 133;
            FirstAirDate["Lucky Lunch"] = 140;
            FirstAirDate["Carp Surprise"] = 147;
            FirstAirDate["Maple Bar"] = 154;
            FirstAirDate["Pink Cake"] = 161;
            FirstAirDate["Roasted Hazelnuts"] = 168;
            FirstAirDate["Fruit Salad"] = 175;
            FirstAirDate["Blackberry Cobbler"] = 182;
            FirstAirDate["Crab Cakes"] = 189;
            FirstAirDate["Fiddlehead Risotto"] = 196;
            FirstAirDate["Poppyseed Muffin"] = 203;
            FirstAirDate["Bruschetta"] = 217;
            FirstAirDate["Shrimp Cocktail"] = 224;

            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.DayEnding += OnDayEnding;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Helper.Content.AssetEditors.Add(new MailEditor(Config.DaysAfterAiring, Config.Price));
        }

        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            if(Game1.Date.TotalDays >= 7 + Config.DaysAfterAiring + 1)
            {
                Game1.addMailForTomorrow("BestOfQOS.Letter1");
            }
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!Context.IsWorldReady) return; //World Hasn't Loaded yet, it's definitely not the menu we want
            if (e.NewMenu == null) return; //Menu was closed
            if (!(e.NewMenu is ShopMenu)) return;
            if (!(Helper.Reflection.GetField<string>(e.NewMenu, "storeContext").GetValue() == "Saloon")) return;
            ShopMenu menu = (ShopMenu)e.NewMenu;
            //Naming is hard. This is the most recent date that a recipe can be available. 
            // Example, it's Y2,S27 (Day 167) using the default config setting of 28 days (making this variable 139) Complete Breakfast (aired day 133) would be available, but Luck Lunch (day 140) would not.
            int latestRecipeDate = Game1.Date.TotalDays - Config.DaysAfterAiring;
            foreach (var kvp in FirstAirDate.Where(x => x.Value <= latestRecipeDate && !Game1.player.cookingRecipes.Keys.Contains(x.Key)))
            {
                var tmp = new CraftingRecipe(kvp.Key, true);
                var tmp2 = new SVObject();
                tmp2.Type = "Cooking";
                tmp2.IsRecipe = true;
                tmp2.Stack = 1;
                tmp2.Name = tmp.name;
                tmp2.ParentSheetIndex = tmp.getIndexOfMenuView(); 
                menu.forSale.Add(tmp2);
                menu.itemPriceAndStock.Add(tmp2, new int[2] { Config.Price, 1 });
            }
        }
    }

    class SellingRecipe : ISalable
    {
        public SellingRecipe(SVObject item, int price)
        {
            this.item = item;
            this.price = price;
        }

        private SVObject item;
        private int price;

        public string DisplayName => item.DisplayName;

        public string Name => item.name;

        public int Stack { get => 0; set => throw new NotImplementedException(); }

        public bool actionWhenPurchased()
        {
            throw new NotImplementedException();
        }

        public int addToStack(Item stack)
        {
            throw new NotImplementedException();
        }

        public bool CanBuyItem(Farmer farmer)
        {
            throw new NotImplementedException();
        }

        public bool canStackWith(ISalable other)
        {
            throw new NotImplementedException();
        }

        public void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            throw new NotImplementedException();
        }

        public string getDescription()
        {
            throw new NotImplementedException();
        }

        public ISalable GetSalableInstance()
        {
            throw new NotImplementedException();
        }

        public bool IsInfiniteStock()
        {
            throw new NotImplementedException();
        }

        public int maximumStackSize()
        {
            throw new NotImplementedException();
        }

        public int salePrice()
        {
            return price;
        }

        public bool ShouldDrawIcon()
        {
            throw new NotImplementedException();
        }
    }
}
