using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace PelicanFiber.Framework
{
    internal class PelicanFiberMenu : IClickableMenu
    {
        /*********
        ** Properties
        *********/
        private static readonly int MenuHeight = 1216;
        private static readonly int MenuWidth = 1354;
        private readonly List<ClickableTextureComponent> LinksToVisit = new List<ClickableTextureComponent>();
        private readonly bool Unfiltered;
        private readonly float Scale;

        private readonly ItemUtils ItemUtils;
        private readonly Action ReopenMainMenu;
        private readonly bool GiveAchievements;
        private readonly Func<long> GetNewId;


        /*********
        ** Public methods
        *********/
        public PelicanFiberMenu(Texture2D websites, ItemUtils itemUtils, bool giveAchievements, Func<long> getNewId, Action reopenMainMenu, float scale = 1.0f, bool unfiltered = true)
          : base(Game1.viewport.Width / 2 - (int)(PelicanFiberMenu.MenuWidth * scale) / 2 - IClickableMenu.borderWidth * 2,
                Game1.viewport.Height / 2 - (int)(PelicanFiberMenu.MenuHeight * scale) / 2 - IClickableMenu.borderWidth * 2,
                (int)(PelicanFiberMenu.MenuWidth * scale) + IClickableMenu.borderWidth * 2,
                (int)(PelicanFiberMenu.MenuHeight * scale) + IClickableMenu.borderWidth, true)
        {
            this.ItemUtils = itemUtils;
            this.GiveAchievements = giveAchievements;
            this.GetNewId = getNewId;
            this.ReopenMainMenu = reopenMainMenu;

            this.height += Game1.tileSize;
            this.Scale = scale;
            this.Unfiltered = unfiltered;


            ClickableTextureComponent c1 = new ClickableTextureComponent(new Rectangle((int)(this.xPositionOnScreen + 55 * scale), (int)(this.yPositionOnScreen + 185 * scale), (int)(256f * scale), (int)(128f * scale)), websites, new Rectangle(0, 0, 256, 128), scale);
            ClickableTextureComponent c1_1 = new ClickableTextureComponent(new Rectangle((int)(this.xPositionOnScreen + 55 * scale), (int)(this.yPositionOnScreen + 313 * scale), (int)(256f * scale), (int)(128f * scale)), websites, new Rectangle(0, 128, 256, 128), scale);
            ClickableTextureComponent c2 = new ClickableTextureComponent(new Rectangle((int)(this.xPositionOnScreen + 321 * scale), (int)(this.yPositionOnScreen + 185 * scale), (int)(256f * scale), (int)(128f * scale)), websites, new Rectangle(257, 0, 256, 128), scale);
            ClickableTextureComponent c2_1 = new ClickableTextureComponent(new Rectangle((int)(this.xPositionOnScreen + 321 * scale), (int)(this.yPositionOnScreen + 313 * scale), (int)(256f * scale), (int)(128f * scale)), websites, new Rectangle(257, 128, 256, 128), scale);
            ClickableTextureComponent c3 = new ClickableTextureComponent(new Rectangle((int)(this.xPositionOnScreen + 587 * scale), (int)(this.yPositionOnScreen + 185 * scale), (int)(256f * scale), (int)(256f * scale)), websites, new Rectangle(513, 0, 256, 256), scale);
            ClickableTextureComponent c4 = new ClickableTextureComponent(new Rectangle((int)(this.xPositionOnScreen + 853 * scale), (int)(this.yPositionOnScreen + 185 * scale), (int)(256f * scale), (int)(128f * scale)), websites, new Rectangle(769, 0, 256, 128), scale);
            ClickableTextureComponent c4_1 = new ClickableTextureComponent(new Rectangle((int)(this.xPositionOnScreen + 853 * scale), (int)(this.yPositionOnScreen + 313 * scale), (int)(256f * scale), (int)(128f * scale)), websites, new Rectangle(769, 128, 256, 128), scale);
            ClickableTextureComponent c17 = new ClickableTextureComponent(new Rectangle((int)(this.xPositionOnScreen + 1119 * scale), (int)(this.yPositionOnScreen + 185 * scale), (int)(256f * scale), (int)(256f * scale)), websites, new Rectangle(1025, 0, 256, 256), scale);

            ClickableTextureComponent c5 = new ClickableTextureComponent(new Rectangle((int)(this.xPositionOnScreen + 55 * scale), (int)(this.yPositionOnScreen + 451 * scale), (int)(256f * scale), (int)(256f * scale)), websites, new Rectangle(0, 257, 256, 256), scale);
            ClickableTextureComponent c6 = new ClickableTextureComponent(new Rectangle((int)(this.xPositionOnScreen + 321 * scale), (int)(this.yPositionOnScreen + 451 * scale), (int)(256f * scale), (int)(256f * scale)), websites, new Rectangle(257, 257, 256, 256), scale);
            ClickableTextureComponent c7 = new ClickableTextureComponent(new Rectangle((int)(this.xPositionOnScreen + 587 * scale), (int)(this.yPositionOnScreen + 451 * scale), (int)(256f * scale), (int)(256f * scale)), websites, new Rectangle(513, 257, 256, 256), scale);
            ClickableTextureComponent c8 = new ClickableTextureComponent(new Rectangle((int)(this.xPositionOnScreen + 853 * scale), (int)(this.yPositionOnScreen + 451 * scale), (int)(256f * scale), (int)(256f * scale)), websites, new Rectangle(769, 257, 256, 256), scale);
            ClickableTextureComponent c18 = new ClickableTextureComponent(new Rectangle((int)(this.xPositionOnScreen + 1119 * scale), (int)(this.yPositionOnScreen + 451 * scale), (int)(256f * scale), (int)(256f * scale)), websites, new Rectangle(1025, 257, 256, 256), scale);

            ClickableTextureComponent c9 = new ClickableTextureComponent(new Rectangle((int)(this.xPositionOnScreen + 55 * scale), (int)(this.yPositionOnScreen + 717 * scale), (int)(256f * scale), (int)(256f * scale)), websites, new Rectangle(0, 513, 256, 256), scale);
            ClickableTextureComponent c10 = new ClickableTextureComponent(new Rectangle((int)(this.xPositionOnScreen + 321 * scale), (int)(this.yPositionOnScreen + 717 * scale), (int)(256f * scale), (int)(256f * scale)), websites, new Rectangle(257, 513, 256, 256), scale);
            ClickableTextureComponent c11 = new ClickableTextureComponent(new Rectangle((int)(this.xPositionOnScreen + 587 * scale), (int)(this.yPositionOnScreen + 717 * scale), (int)(256f * scale), (int)(256f * scale)), websites, new Rectangle(513, 513, 256, 256), scale);
            ClickableTextureComponent c12 = new ClickableTextureComponent(new Rectangle((int)(this.xPositionOnScreen + 853 * scale), (int)(this.yPositionOnScreen + 717 * scale), (int)(256f * scale), (int)(256f * scale)), websites, new Rectangle(769, 513, 256, 256), scale);
            ClickableTextureComponent c19 = new ClickableTextureComponent(new Rectangle((int)(this.xPositionOnScreen + 1119 * scale), (int)(this.yPositionOnScreen + 717 * scale), (int)(256f * scale), (int)(256f * scale)), websites, new Rectangle(1025, 513, 256, 256), scale);

            ClickableTextureComponent c13 = new ClickableTextureComponent(new Rectangle((int)(this.xPositionOnScreen + 55 * scale), (int)(this.yPositionOnScreen + 983 * scale), (int)(256f * scale), (int)(256f * scale)), websites, new Rectangle(0, 769, 256, 256), scale);
            ClickableTextureComponent c14 = new ClickableTextureComponent(new Rectangle((int)(this.xPositionOnScreen + 321 * scale), (int)(this.yPositionOnScreen + 983 * scale), (int)(256f * scale), (int)(256f * scale)), websites, new Rectangle(257, 769, 256, 256), scale);
            ClickableTextureComponent c15 = new ClickableTextureComponent(new Rectangle((int)(this.xPositionOnScreen + 587 * scale), (int)(this.yPositionOnScreen + 983 * scale), (int)(256f * scale), (int)(256f * scale)), websites, new Rectangle(513, 769, 256, 256), scale);
            ClickableTextureComponent c16 = new ClickableTextureComponent(new Rectangle((int)(this.xPositionOnScreen + 853 * scale), (int)(this.yPositionOnScreen + 983 * scale), (int)(256f * scale), (int)(256f * scale)), websites, new Rectangle(769, 769, 256, 256), scale);
            ClickableTextureComponent c20 = new ClickableTextureComponent(new Rectangle((int)(this.xPositionOnScreen + 1119 * scale), (int)(this.yPositionOnScreen + 983 * scale), (int)(256f * scale), (int)(256f * scale)), websites, new Rectangle(1025, 769, 256, 256), scale);

            this.upperRightCloseButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 9 * Game1.pixelZoom, this.yPositionOnScreen - Game1.pixelZoom * 2, 12 * Game1.pixelZoom, 12 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), Game1.pixelZoom);

            c1.name = "blacksmith_tools";
            c1_1.name = "blacksmith";
            c2.name = "animals";
            c2_1.name = "animal_supplies";
            c3.name = "produce";
            c4.name = "carpentry_build";
            c4_1.name = "carpentry";
            c5.name = "fish";
            c6.name = "dining";
            c7.name = "imports";
            c8.name = "adventure";
            c9.name = "wizard";
            c10.name = "hats";
            c11.name = "hospital";
            c12.name = "krobus";
            c13.name = "dwarf";
            c14.name = "qi";
            c15.name = "sandy";
            c16.name = "joja";
            c17.name = "sauce";
            c18.name = "bundle";
            c19.name = "artifact";
            c20.name = "leah";

            this.LinksToVisit.Add(c1);
            this.LinksToVisit.Add(c1_1);
            this.LinksToVisit.Add(c2);
            this.LinksToVisit.Add(c2_1);
            this.LinksToVisit.Add(c3);
            this.LinksToVisit.Add(c4);
            this.LinksToVisit.Add(c4_1);
            this.LinksToVisit.Add(c5);
            this.LinksToVisit.Add(c6);
            this.LinksToVisit.Add(c7);
            this.LinksToVisit.Add(c8);
            this.LinksToVisit.Add(c9);
            this.LinksToVisit.Add(c10);
            this.LinksToVisit.Add(c11);
            this.LinksToVisit.Add(c12);
            this.LinksToVisit.Add(c13);
            this.LinksToVisit.Add(c14);
            this.LinksToVisit.Add(c15);
            this.LinksToVisit.Add(c16);
            this.LinksToVisit.Add(c17);
            this.LinksToVisit.Add(c18);
            this.LinksToVisit.Add(c19);
            this.LinksToVisit.Add(c20);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {

        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);

            foreach (ClickableTextureComponent textureComponent in this.LinksToVisit)
            {
                if (textureComponent.containsPoint(x, y))
                {
                    switch (textureComponent.name)
                    {
                        case "blacksmith":
                            this.exitThisMenu();
                            Game1.activeClickableMenu = new ShopMenu2(this.ReopenMainMenu, this.ItemUtils, this.GiveAchievements, this.ItemUtils.GetBlacksmithStock(this.Unfiltered), 0, null, "Blacksmith");
                            break;
                        case "blacksmith_tools":
                            this.exitThisMenu();
                            Game1.activeClickableMenu = new ShopMenu2(this.ReopenMainMenu, this.ItemUtils, this.GiveAchievements, Utility.getBlacksmithUpgradeStock(Game1.player));
                            break;
                        case "animals":
                            this.exitThisMenu();
                            Game1.activeClickableMenu = new ShopMenu2(this.ReopenMainMenu, this.ItemUtils, this.GiveAchievements, Utility.getAnimalShopStock(), 0, null, "AnimalShop");
                            break;
                        case "animal_supplies":
                            this.exitThisMenu();
                            if (Game1.currentLocation is AnimalHouse)
                                Game1.activeClickableMenu = new MailOrderPigMenu(this.ItemUtils.GetPurchaseAnimalStock(), this.ItemUtils, this.ReopenMainMenu, this.GetNewId);
                            else
                                Game1.activeClickableMenu = new BuyAnimalMenu(Utility.getPurchaseAnimalStock(), this.ReopenMainMenu, this.GetNewId);
                            break;
                        case "produce":
                            this.exitThisMenu();
                            //Game1.activeClickableMenu = new ShopMenu2(Utility.getShopStock(true), 0, null, "SeedShop");
                            Game1.activeClickableMenu = new ShopMenu2(this.ReopenMainMenu, this.ItemUtils, this.GiveAchievements, this.ItemUtils.GetShopStock(true, this.Unfiltered), 0, null, "SeedShop");
                            break;
                        case "carpentry":
                            this.exitThisMenu();
                            Game1.activeClickableMenu = new ShopMenu2(this.ReopenMainMenu, this.ItemUtils, this.GiveAchievements, this.ItemUtils.GetCarpenterStock(this.Unfiltered), 0, null, "ScienceHouse");
                            break;
                        case "carpentry_build":
                            this.exitThisMenu();
                            Game1.activeClickableMenu = new ConstructionMenu(false, this.ReopenMainMenu);
                            break;
                        case "fish":
                            this.exitThisMenu();
                            Game1.activeClickableMenu = new ShopMenu2(this.ReopenMainMenu, this.ItemUtils, this.GiveAchievements, this.ItemUtils.GetFishShopStock(Game1.player, this.Unfiltered), 0, null, "FishShop");
                            break;
                        case "dining":
                            this.exitThisMenu();
                            Game1.activeClickableMenu = new ShopMenu2(this.ReopenMainMenu, this.ItemUtils, this.GiveAchievements, this.ItemUtils.GetSaloonStock(this.Unfiltered));
                            break;
                        case "imports":
                            this.exitThisMenu();
                            {
                                Forest forest = (Forest)Game1.getLocationFromName("Forest");
                                Game1.activeClickableMenu = new ShopMenu2(this.ReopenMainMenu, this.ItemUtils, this.GiveAchievements, Utility.getTravelingMerchantStock(forest.stockSeed.Value));
                            }
                            break;
                        case "adventure":
                            this.exitThisMenu();
                            Game1.activeClickableMenu = new ShopMenu2(this.ReopenMainMenu, this.ItemUtils, this.GiveAchievements, this.GetAdventureShopStock(), 0, null, "AdventureGuild");
                            break;
                        case "hats":
                            this.exitThisMenu();
                            Game1.activeClickableMenu = new ShopMenu2(this.ReopenMainMenu, this.ItemUtils, this.GiveAchievements, Utility.getHatStock());
                            break;
                        case "hospital":
                            this.exitThisMenu();
                            Game1.activeClickableMenu = new ShopMenu2(this.ReopenMainMenu, this.ItemUtils, this.GiveAchievements, Utility.getHospitalStock());
                            break;
                        case "wizard":
                            this.exitThisMenu();
                            Game1.activeClickableMenu = new ConstructionMenu(true, this.ReopenMainMenu);
                            break;
                        case "dwarf":
                            this.exitThisMenu();
                            Game1.activeClickableMenu = new ShopMenu2(this.ReopenMainMenu, this.ItemUtils, this.GiveAchievements, Utility.getDwarfShopStock());
                            break;
                        case "krobus":
                            this.exitThisMenu();
                            Game1.activeClickableMenu = new ShopMenu2(this.ReopenMainMenu, this.ItemUtils, this.GiveAchievements, (Game1.getLocationFromName("Sewer") as Sewer).getShadowShopStock(), 0, "Krobus");
                            break;
                        case "qi":
                            this.exitThisMenu();
                            Game1.activeClickableMenu = new ShopMenu2(this.ReopenMainMenu, this.ItemUtils, this.GiveAchievements, Utility.getQiShopStock());
                            break;
                        case "joja":
                            this.exitThisMenu();
                            Game1.activeClickableMenu = new ShopMenu2(this.ReopenMainMenu, this.ItemUtils, this.GiveAchievements, Utility.getJojaStock());
                            break;
                        case "sandy":
                            this.exitThisMenu();
                            Game1.activeClickableMenu = new ShopMenu2(this.ReopenMainMenu, this.ItemUtils, this.GiveAchievements, this.ItemUtils.GetShopStock(false, this.Unfiltered));
                            break;
                        case "sauce":
                            this.exitThisMenu();
                            Game1.activeClickableMenu = new ShopMenu2(this.ReopenMainMenu, this.ItemUtils, this.GiveAchievements, this.ItemUtils.GetRecipesStock(this.Unfiltered), 0, null, "Recipe");
                            break;
                        case "bundle":
                            this.exitThisMenu();
                            Game1.activeClickableMenu = new ShopMenu2(this.ReopenMainMenu, this.ItemUtils, this.GiveAchievements, this.ItemUtils.GetJunimoStock(), 0, null, "Junimo");
                            //ItemUtils.finishAllBundles();
                            //Game1.showRedMessage("Error 404: Not found. www.thejunimoconspiracy.com");
                            break;
                        case "artifact":
                            this.exitThisMenu();
                            Game1.activeClickableMenu = new ShopMenu2(this.ReopenMainMenu, this.ItemUtils, this.GiveAchievements, this.ItemUtils.GetMineralsAndArtifactsStock(this.Unfiltered), 0, null, "Artifact");
                            break;
                        case "leah":
                            this.exitThisMenu();
                            Game1.activeClickableMenu = new ShopMenu2(this.ReopenMainMenu, this.ItemUtils, this.GiveAchievements, this.ItemUtils.GetLeahShopStock(this.Unfiltered), 0, "Leah", "LeahCottage");
                            break;
                    }
                }
            }
        }

        public override void performHoverAction(int x, int y)
        {
            this.upperRightCloseButton.tryHover(x, y, 0.5f);

            foreach (ClickableTextureComponent textureComponent in this.LinksToVisit)
            {
                textureComponent.scale = textureComponent.containsPoint(x, y)
                    ? Math.Min(textureComponent.scale + 0.05f, textureComponent.baseScale - 0.05f)
                    : Math.Max(textureComponent.baseScale, textureComponent.scale - 0.025f);
            }
        }

        public override void draw(SpriteBatch b)
        {
            if (!Game1.dialogueUp && !Game1.globalFade)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

                if (this.Scale > .9f)
                    SpriteText.drawStringWithScrollCenteredAt(b, "PelicanFiber 3.0 (a subsidiary of JojaNet, Inc.)", Game1.viewport.Width / 2, (int)(this.yPositionOnScreen * this.Scale));
                else
                    SpriteText.drawStringWithScrollCenteredAt(b, "PelicanFiber 3.0 (a subsidiary of JojaNet, Inc.)", Game1.viewport.Width / 2, (int)(this.yPositionOnScreen * this.Scale) + 25);

                Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);

                if (this.Scale < 1.0f)
                    SpriteText.drawStringHorizontallyCenteredAt(b, "Click a Link Below to Shop Online", Game1.viewport.Width / 2, (int)(this.yPositionOnScreen + 92 * this.Scale) + 35);
                else
                    SpriteText.drawStringHorizontallyCenteredAt(b, "Click a Link Below to Shop Online", Game1.viewport.Width / 2, (int)(this.yPositionOnScreen + 92 * this.Scale));

                Game1.dayTimeMoneyBox.drawMoneyBox(b);
                foreach (ClickableTextureComponent textureComponent in this.LinksToVisit)
                    textureComponent.draw(b);

                SpriteText.drawStringHorizontallyCenteredAt(b, "Blacksmith", (int)(this.xPositionOnScreen + 182 * this.Scale), (int)(this.yPositionOnScreen + 381 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Animals", (int)(this.xPositionOnScreen + 448 * this.Scale), (int)(this.yPositionOnScreen + 381 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Produce", (int)(this.xPositionOnScreen + 714 * this.Scale), (int)(this.yPositionOnScreen + 381 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Carpentry", (int)(this.xPositionOnScreen + 980 * this.Scale), (int)(this.yPositionOnScreen + 381 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Recipes", (int)(this.xPositionOnScreen + 1246 * this.Scale), (int)(this.yPositionOnScreen + 381 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 1);

                SpriteText.drawStringHorizontallyCenteredAt(b, "Upgrades", (int)(this.xPositionOnScreen + 182 * this.Scale), (int)(this.yPositionOnScreen + 211 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Supplies", (int)(this.xPositionOnScreen + 448 * this.Scale), (int)(this.yPositionOnScreen + 211 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                //SpriteText.drawStringHorizontallyCenteredAt(b, "Produce", (int)(this.xPositionOnScreen + 714 * scale), (int)(this.yPositionOnScreen + 381 * scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Buildings", (int)(this.xPositionOnScreen + 980 * this.Scale), (int)(this.yPositionOnScreen + 211 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 1);

                SpriteText.drawStringHorizontallyCenteredAt(b, "Fish", (int)(this.xPositionOnScreen + 182 * this.Scale), (int)(this.yPositionOnScreen + 643 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Dining", (int)(this.xPositionOnScreen + 448 * this.Scale), (int)(this.yPositionOnScreen + 643 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Imports", (int)(this.xPositionOnScreen + 714 * this.Scale), (int)(this.yPositionOnScreen + 643 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Adventure", (int)(this.xPositionOnScreen + 980 * this.Scale), (int)(this.yPositionOnScreen + 643 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Bundles", (int)(this.xPositionOnScreen + 1246 * this.Scale), (int)(this.yPositionOnScreen + 643 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 1);


                SpriteText.drawStringHorizontallyCenteredAt(b, "Wizard", (int)(this.xPositionOnScreen + 182 * this.Scale), (int)(this.yPositionOnScreen + 905 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Hats", (int)(this.xPositionOnScreen + 448 * this.Scale), (int)(this.yPositionOnScreen + 905 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Hospital", (int)(this.xPositionOnScreen + 714 * this.Scale), (int)(this.yPositionOnScreen + 905 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Krobus", (int)(this.xPositionOnScreen + 980 * this.Scale), (int)(this.yPositionOnScreen + 905 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Artifacts", (int)(this.xPositionOnScreen + 1246 * this.Scale), (int)(this.yPositionOnScreen + 905 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 1);

                SpriteText.drawStringHorizontallyCenteredAt(b, "Dwarf", (int)(this.xPositionOnScreen + 182 * this.Scale), (int)(this.yPositionOnScreen + 1167 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Qi", (int)(this.xPositionOnScreen + 448 * this.Scale), (int)(this.yPositionOnScreen + 1167 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Oaisis", (int)(this.xPositionOnScreen + 714 * this.Scale), (int)(this.yPositionOnScreen + 1167 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Joja", (int)(this.xPositionOnScreen + 980 * this.Scale), (int)(this.yPositionOnScreen + 1167 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 1);

                SpriteText.drawStringHorizontallyCenteredAt(b, "Foraged", (int)(this.xPositionOnScreen + 1246 * this.Scale), (int)(this.yPositionOnScreen + 997 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "by", (int)(this.xPositionOnScreen + 1246 * this.Scale), (int)(this.yPositionOnScreen + 1082 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Leah", (int)(this.xPositionOnScreen + 1246 * this.Scale), (int)(this.yPositionOnScreen + 1167 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 1);



                SpriteText.drawStringHorizontallyCenteredAt(b, "Blacksmith", (int)(this.xPositionOnScreen + 180 * this.Scale), (int)(this.yPositionOnScreen + 379 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Animals", (int)(this.xPositionOnScreen + 446 * this.Scale), (int)(this.yPositionOnScreen + 379 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Produce", (int)(this.xPositionOnScreen + 712 * this.Scale), (int)(this.yPositionOnScreen + 379 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Carpentry", (int)(this.xPositionOnScreen + 978 * this.Scale), (int)(this.yPositionOnScreen + 379 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Recipes", (int)(this.xPositionOnScreen + 1244 * this.Scale), (int)(this.yPositionOnScreen + 379 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 4);


                SpriteText.drawStringHorizontallyCenteredAt(b, "Upgrades", (int)(this.xPositionOnScreen + 180 * this.Scale), (int)(this.yPositionOnScreen + 209 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Supplies", (int)(this.xPositionOnScreen + 446 * this.Scale), (int)(this.yPositionOnScreen + 209 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                //SpriteText.drawStringHorizontallyCenteredAt(b, "Produce", (int)(this.xPositionOnScreen + 712 * scale), (int)(this.yPositionOnScreen + 379 * scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Buildings", (int)(this.xPositionOnScreen + 978 * this.Scale), (int)(this.yPositionOnScreen + 209 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 4);

                SpriteText.drawStringHorizontallyCenteredAt(b, "Fish", (int)(this.xPositionOnScreen + 180 * this.Scale), (int)(this.yPositionOnScreen + 641 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Dining", (int)(this.xPositionOnScreen + 446 * this.Scale), (int)(this.yPositionOnScreen + 641 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Imports", (int)(this.xPositionOnScreen + 712 * this.Scale), (int)(this.yPositionOnScreen + 641 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Adventure", (int)(this.xPositionOnScreen + 978 * this.Scale), (int)(this.yPositionOnScreen + 641 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Bundles", (int)(this.xPositionOnScreen + 1244 * this.Scale), (int)(this.yPositionOnScreen + 641 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 4);


                SpriteText.drawStringHorizontallyCenteredAt(b, "Wizard", (int)(this.xPositionOnScreen + 180 * this.Scale), (int)(this.yPositionOnScreen + 903 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Hats", (int)(this.xPositionOnScreen + 446 * this.Scale), (int)(this.yPositionOnScreen + 903 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Hospital", (int)(this.xPositionOnScreen + 712 * this.Scale), (int)(this.yPositionOnScreen + 903 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Krobus", (int)(this.xPositionOnScreen + 978 * this.Scale), (int)(this.yPositionOnScreen + 903 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Artifacts", (int)(this.xPositionOnScreen + 1244 * this.Scale), (int)(this.yPositionOnScreen + 903 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 4);

                SpriteText.drawStringHorizontallyCenteredAt(b, "Dwarf", (int)(this.xPositionOnScreen + 180 * this.Scale), (int)(this.yPositionOnScreen + 1165 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Qi", (int)(this.xPositionOnScreen + 448 * this.Scale), (int)(this.yPositionOnScreen + 1165 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Oaisis", (int)(this.xPositionOnScreen + 714 * this.Scale), (int)(this.yPositionOnScreen + 1165 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Joja", (int)(this.xPositionOnScreen + 980 * this.Scale), (int)(this.yPositionOnScreen + 1165 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 4);

                SpriteText.drawStringHorizontallyCenteredAt(b, "Foraged", (int)(this.xPositionOnScreen + 1244 * this.Scale), (int)(this.yPositionOnScreen + 995 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "by", (int)(this.xPositionOnScreen + 1244 * this.Scale), (int)(this.yPositionOnScreen + 1080 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Leah", (int)(this.xPositionOnScreen + 1244 * this.Scale), (int)(this.yPositionOnScreen + 1165 * this.Scale), 999999, -1, 999999, 1, 0.88f, false, 4);

                this.upperRightCloseButton.draw(b);
            }
            this.drawMouse(b);
        }


        /*********
        ** Private methods
        *********/
        private Dictionary<Item, int[]> GetAdventureShopStock()
        {
            Dictionary<Item, int[]> itemPriceAndStock = new Dictionary<Item, int[]>();
            int maxValue = int.MaxValue;
            itemPriceAndStock.Add(new MeleeWeapon(12), new[] { 250, maxValue });
            if (Game1.mine != null)
            {
                if (MineShaft.lowestLevelReached >= 15)
                    itemPriceAndStock.Add(new MeleeWeapon(17), new[] { 500, maxValue });
                if (MineShaft.lowestLevelReached >= 20)
                    itemPriceAndStock.Add(new MeleeWeapon(1), new[] { 750, maxValue });
                if (MineShaft.lowestLevelReached >= 25)
                {
                    itemPriceAndStock.Add(new MeleeWeapon(43), new[] { 850, maxValue });
                    itemPriceAndStock.Add(new MeleeWeapon(44), new[] { 1500, maxValue });
                }
                if (MineShaft.lowestLevelReached >= 40)
                    itemPriceAndStock.Add(new MeleeWeapon(27), new[] { 2000, maxValue });
                if (MineShaft.lowestLevelReached >= 45)
                    itemPriceAndStock.Add(new MeleeWeapon(10), new[] { 2000, maxValue });
                if (MineShaft.lowestLevelReached >= 55)
                    itemPriceAndStock.Add(new MeleeWeapon(7), new[] { 4000, maxValue });
                if (MineShaft.lowestLevelReached >= 75)
                    itemPriceAndStock.Add(new MeleeWeapon(5), new[] { 6000, maxValue });
                if (MineShaft.lowestLevelReached >= 90)
                    itemPriceAndStock.Add(new MeleeWeapon(50), new[] { 9000, maxValue });
                if (MineShaft.lowestLevelReached >= 120)
                    itemPriceAndStock.Add(new MeleeWeapon(9), new[] { 25000, maxValue });
                if (Game1.player.mailReceived.Contains("galaxySword"))
                {
                    itemPriceAndStock.Add(new MeleeWeapon(4), new[] { 50000, maxValue });
                    itemPriceAndStock.Add(new MeleeWeapon(23), new[] { 350000, maxValue });
                    itemPriceAndStock.Add(new MeleeWeapon(29), new[] { 75000, maxValue });
                }
            }
            itemPriceAndStock.Add(new Boots(504), new[] { 500, maxValue });
            if (Game1.mine != null && MineShaft.lowestLevelReached >= 40)
                itemPriceAndStock.Add(new Boots(508), new[] { 1250, maxValue });
            if (Game1.mine != null && MineShaft.lowestLevelReached >= 80)
                itemPriceAndStock.Add(new Boots(511), new[] { 2500, maxValue });
            itemPriceAndStock.Add(new Ring(529), new[] { 1000, maxValue });
            itemPriceAndStock.Add(new Ring(530), new[] { 1000, maxValue });
            if (Game1.mine != null && MineShaft.lowestLevelReached >= 40)
            {
                itemPriceAndStock.Add(new Ring(531), new[] { 2500, maxValue });
                itemPriceAndStock.Add(new Ring(532), new[] { 2500, maxValue });
            }
            if (Game1.mine != null && MineShaft.lowestLevelReached >= 80)
            {
                itemPriceAndStock.Add(new Ring(533), new[] { 5000, maxValue });
                itemPriceAndStock.Add(new Ring(534), new[] { 5000, maxValue });
            }
            if (Game1.player.hasItemWithNameThatContains("Slingshot") != null)
                itemPriceAndStock.Add(new Object(441, int.MaxValue), new[] { 100, maxValue });
            if (Game1.player.mailReceived.Contains("Gil_Slime Charmer Ring"))
                itemPriceAndStock.Add(new Ring(520), new[] { 25000, maxValue });
            if (Game1.player.mailReceived.Contains("Gil_Savage Ring"))
                itemPriceAndStock.Add(new Ring(523), new[] { 25000, maxValue });
            if (Game1.player.mailReceived.Contains("Gil_Burglar's Ring"))
                itemPriceAndStock.Add(new Ring(526), new[] { 20000, maxValue });
            if (Game1.player.mailReceived.Contains("Gil_Vampire Ring"))
                itemPriceAndStock.Add(new Ring(522), new[] { 15000, maxValue });
            if (Game1.player.mailReceived.Contains("Gil_Skeleton Mask"))
                itemPriceAndStock.Add(new Hat(8), new[] { 20000, maxValue });
            if (Game1.player.mailReceived.Contains("Gil_Hard Hat"))
                itemPriceAndStock.Add(new Hat(27), new[] { 20000, maxValue });
            if (Game1.player.mailReceived.Contains("Gil_Insect Head"))
                itemPriceAndStock.Add(new MeleeWeapon(13), new[] { 10000, maxValue });
            //Game1.activeClickableMenu = (IClickableMenu)new ShopMenu(itemPriceAndStock, 0, "Marlon");

            return itemPriceAndStock;
        }
    }
}
