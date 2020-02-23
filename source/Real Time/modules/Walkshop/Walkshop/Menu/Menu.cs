using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using EiTK.Gui;
using EiTK.Gui.Option;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Objects;
using WalkSho;
using Object = StardewValley.Object;

namespace Walkshop
{
    public class Menu : GuiMenu
    {
        private int width;
        private int height;
        
        private List<ShopData> _shopDatas;
        

        public Menu(IModHelper helper)
        {
            this._shopDatas = new List<ShopData>();
            this.width = 835;
            this.height = 595;
            Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height, 0, 0);
            this.xPositionOnScreen = (int) centeringOnScreen.X;
            this.yPositionOnScreen = (int) centeringOnScreen.Y + 32;
            init();

            List<GuiOptionsElements> optionsElementses = new List<GuiOptionsElements>();
            foreach (var VARIABLE in _shopDatas)
            {
                optionsElementses.Add(new GuiOptionButton(helper.Translation.Get("walkshop." + VARIABLE.name),helper.Translation.Get("walkshop.button"),670,
                    () =>
                    {
                        GuiHelper.openGui(VARIABLE.iClickableMenu);
                    }));
            }
            
            this.optionLists.Add(new GuiOptionList(this.xPositionOnScreen + 20,this.yPositionOnScreen + 20,10)
            {
                guiOptionsElementses = optionsElementses
            });

        }

        private void init()
        {
            _shopDatas = new List<ShopData>();
            _shopDatas.Add(new ShopData("pierre", new ShopMenu(getShopStock(true))));
            _shopDatas.Add(new ShopData("willy", new ShopMenu(Utility.getFishShopStock(Game1.player))));
            _shopDatas.Add(new ShopData("adventure", new ShopMenu(Utility.getAdventureShopStock())));
            _shopDatas.Add(new ShopData("blacksmith", new ShopMenu(Utility.getBlacksmithStock())));
            _shopDatas.Add(new ShopData("saloon", new ShopMenu(Utility.getSaloonStock())));
            _shopDatas.Add(new ShopData("hospital", new ShopMenu(Utility.getHospitalStock())));
            _shopDatas.Add(new ShopData("krobus", new ShopMenu(getKrobusStock())));
            _shopDatas.Add(new ShopData("animal", new ShopMenu(Utility.getAnimalShopStock())));
            _shopDatas.Add(new ShopData("merchant",
                new ShopMenu(
                    Utility.getTravelingMerchantStock((int) (Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed)))));
            
            _shopDatas.Add(new ShopData("qi", new ShopMenu(Utility.getQiShopStock())));
            _shopDatas.Add(new ShopData("animal", new PurchaseAnimalsMenu(Utility.getPurchaseAnimalStock())));
            _shopDatas.Add(new ShopData("carpenter", new CarpenterMenu()));
            _shopDatas.Add(new ShopData("wizard", new CarpenterMenu(true)));
            _shopDatas.Add(new ShopData("bundles", new JunimoNoteMenu(true, 1, true)));
            _shopDatas.Add(new ShopData("geode", new GeodeMenu()));
            _shopDatas.Add(new ShopData("upgrade", new ShopMenu(Utility.getBlacksmithUpgradeStock(Game1.player))));
            _shopDatas.Add(new ShopData("sewing", new TailoringMenu()));
            _shopDatas.Add(new ShopData("dye", new DyeMenu()));
            _shopDatas.Add(new ShopData("mines", new MineElevatorMenu()));
            _shopDatas.Add(new ShopData("ship", ShippingBin()));
        }

        
        public override void draw(SpriteBatch b)
        {
            
            GuiHelper.drawBox(b,this.xPositionOnScreen,
                this.yPositionOnScreen,835,595,Color.White);
            base.draw(b);
            drawMouse(b);
        }


        private List<ISalable> getShopStock(bool Pierres)
        {
            List<ISalable> objList1 = new List<ISalable>();
            if (Pierres)
            {
                if (Game1.currentSeason.Equals("spring"))
                {
                    objList1.Add((Item) new Object(Vector2.Zero, 472, int.MaxValue));
                    objList1.Add((Item) new Object(Vector2.Zero, 473, int.MaxValue));
                    objList1.Add((Item) new Object(Vector2.Zero, 474, int.MaxValue));
                    objList1.Add((Item) new Object(Vector2.Zero, 475, int.MaxValue));
                    objList1.Add((Item) new Object(Vector2.Zero, 427, int.MaxValue));
                    objList1.Add((Item) new Object(Vector2.Zero, 429, int.MaxValue));
                    objList1.Add((Item) new Object(Vector2.Zero, 477, int.MaxValue));
                    objList1.Add((Item) new Object(628, int.MaxValue, false, 1700, 0));
                    objList1.Add((Item) new Object(629, int.MaxValue, false, 1000, 0));
                    if (Game1.year > 1)
                        objList1.Add((Item) new Object(Vector2.Zero, 476, int.MaxValue));
                }

                if (Game1.currentSeason.Equals("summer"))
                {
                    objList1.Add((Item) new Object(Vector2.Zero, 480, int.MaxValue));
                    objList1.Add((Item) new Object(Vector2.Zero, 482, int.MaxValue));
                    objList1.Add((Item) new Object(Vector2.Zero, 483, int.MaxValue));
                    objList1.Add((Item) new Object(Vector2.Zero, 484, int.MaxValue));
                    objList1.Add((Item) new Object(Vector2.Zero, 479, int.MaxValue));
                    objList1.Add((Item) new Object(Vector2.Zero, 302, int.MaxValue));
                    objList1.Add((Item) new Object(Vector2.Zero, 453, int.MaxValue));
                    objList1.Add((Item) new Object(Vector2.Zero, 455, int.MaxValue));
                    objList1.Add((Item) new Object(630, int.MaxValue, false, 2000, 0));
                    objList1.Add((Item) new Object(631, int.MaxValue, false, 3000, 0));
                    if (Game1.year > 1)
                        objList1.Add((Item) new Object(Vector2.Zero, 485, int.MaxValue));
                }

                if (Game1.currentSeason.Equals("fall"))
                {
                    objList1.Add((Item) new Object(Vector2.Zero, 487, int.MaxValue));
                    objList1.Add((Item) new Object(Vector2.Zero, 488, int.MaxValue));
                    objList1.Add((Item) new Object(Vector2.Zero, 490, int.MaxValue));
                    objList1.Add((Item) new Object(Vector2.Zero, 299, int.MaxValue));
                    objList1.Add((Item) new Object(Vector2.Zero, 301, int.MaxValue));
                    objList1.Add((Item) new Object(Vector2.Zero, 492, int.MaxValue));
                    objList1.Add((Item) new Object(Vector2.Zero, 491, int.MaxValue));
                    objList1.Add((Item) new Object(Vector2.Zero, 493, int.MaxValue));
                    objList1.Add((Item) new Object(431, int.MaxValue, false, 100, 0));
                    objList1.Add((Item) new Object(Vector2.Zero, 425, int.MaxValue));
                    objList1.Add((Item) new Object(632, int.MaxValue, false, 3000, 0));
                    objList1.Add((Item) new Object(633, int.MaxValue, false, 2000, 0));
                    if (Game1.year > 1)
                        objList1.Add((Item) new Object(Vector2.Zero, 489, int.MaxValue));
                }

                objList1.Add((Item) new Object(Vector2.Zero, 297, int.MaxValue));
                objList1.Add((Item) new Object(Vector2.Zero, 245, int.MaxValue));
                objList1.Add((Item) new Object(Vector2.Zero, 246, int.MaxValue));
                objList1.Add((Item) new Object(Vector2.Zero, 423, int.MaxValue));
                Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2);
                List<ISalable> objList2 = objList1;
                Wallpaper wallpaper1 = new Wallpaper(random.Next(112), false);
                wallpaper1.Stack = int.MaxValue;
                objList2.Add((Item) wallpaper1);
                List<ISalable> objList3 = objList1;
                Wallpaper wallpaper2 = new Wallpaper(random.Next(40), true);
                wallpaper2.Stack = int.MaxValue;
                objList3.Add((Item) wallpaper2);
                List<ISalable> objList4 = objList1;
                Clothing clothing = new Clothing(1000 + random.Next(128));
                clothing.Stack = int.MaxValue;
                clothing.Price = 1000;
                objList4.Add((Item) clothing);
                if (Game1.player.achievements.Contains(38))
                    objList1.Add((Item) new Object(Vector2.Zero, 458, int.MaxValue));
            }
            else
            {
                if (Game1.currentSeason.Equals("spring"))
                    objList1.Add((Item) new Object(Vector2.Zero, 478, int.MaxValue));
                if (Game1.currentSeason.Equals("summer"))
                {
                    objList1.Add((Item) new Object(Vector2.Zero, 486, int.MaxValue));
                    objList1.Add((Item) new Object(Vector2.Zero, 481, int.MaxValue));
                }

                if (Game1.currentSeason.Equals("fall"))
                {
                    objList1.Add((Item) new Object(Vector2.Zero, 493, int.MaxValue));
                    objList1.Add((Item) new Object(Vector2.Zero, 494, int.MaxValue));
                }

                objList1.Add((Item) new Object(Vector2.Zero, 88, int.MaxValue));
                objList1.Add((Item) new Object(Vector2.Zero, 90, int.MaxValue));
            }

            return objList1;
        }

        public List<ISalable> getKrobusStock()
        {
            return new List<ISalable>
            {
                new StardewValley.Object(305, int.MaxValue, false, 2500, 0),
                new StardewValley.Object(434, 1, false, 10000, 0)
            };
        }
        
        private ItemGrabMenu ShippingBin()
        {
            MethodInfo method = typeof(Farm).GetMethod("shipItem", BindingFlags.Instance | BindingFlags.NonPublic);
            ItemGrabMenu.behaviorOnItemSelect behaviorOnItemSelectFunction = (ItemGrabMenu.behaviorOnItemSelect)Delegate.CreateDelegate(typeof(ItemGrabMenu.behaviorOnItemSelect), Game1.getFarm(), method);
            ItemGrabMenu itemGrabMenu = new ItemGrabMenu(null, true, false, new InventoryMenu.highlightThisItem(Utility.highlightShippableObjects), behaviorOnItemSelectFunction, "", null, true, true, false, true, false, 0, null, -1, null);
            itemGrabMenu.initializeUpperRightCloseButton();
            int num = 0;
            itemGrabMenu.setBackgroundTransparency(num != 0);
            int num2 = 1;
            itemGrabMenu.setDestroyItemOnClick(num2 != 0);
            itemGrabMenu.initializeShippingBin();
            return itemGrabMenu;
        }
    }
}