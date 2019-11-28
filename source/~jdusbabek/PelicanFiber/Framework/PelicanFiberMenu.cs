using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
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

        /// <summary>Simplifies access to private game code.</summary>
        private readonly IReflectionHelper Reflection;
        private readonly ItemUtils ItemUtils;
        private readonly Action OnLinkOpened;
        private readonly Func<long> GetNewId;


        /*********
        ** Public methods
        *********/
        public PelicanFiberMenu(Texture2D websites, IReflectionHelper reflection, ItemUtils itemUtils, Func<long> getNewId, Action onLinkOpened, float scale = 1.0f, bool unfiltered = true)
          : base(Game1.viewport.Width / 2 - (int)(PelicanFiberMenu.MenuWidth * scale) / 2 - IClickableMenu.borderWidth * 2,
                Game1.viewport.Height / 2 - (int)(PelicanFiberMenu.MenuHeight * scale) / 2 - IClickableMenu.borderWidth * 2,
                (int)(PelicanFiberMenu.MenuWidth * scale) + IClickableMenu.borderWidth * 2,
                (int)(PelicanFiberMenu.MenuHeight * scale) + IClickableMenu.borderWidth, true)
        {
            this.Reflection = reflection;
            this.ItemUtils = itemUtils;
            this.GetNewId = getNewId;
            this.OnLinkOpened = onLinkOpened;

            this.height += Game1.tileSize;
            this.Scale = scale;
            this.Unfiltered = unfiltered;

            this.AddLink("blacksmith_tools", 55, 185, websites, new Rectangle(0, 0, 256, 128));
            this.AddLink("blacksmith", 55, 313, websites, new Rectangle(0, 128, 256, 128));
            this.AddLink("animals", 321, 185, websites, new Rectangle(257, 0, 256, 128));
            this.AddLink("animal_supplies", 321, 313, websites, new Rectangle(257, 128, 256, 128));
            this.AddLink("produce", 587, 185, websites, new Rectangle(513, 0, 256, 256));
            this.AddLink("carpentry_build", 853, 185, websites, new Rectangle(769, 0, 256, 128));
            this.AddLink("carpentry", 853, 313, websites, new Rectangle(769, 128, 256, 128));
            this.AddLink("sauce", 1119, 185, websites, new Rectangle(1025, 0, 256, 256));

            this.AddLink("fish", 55, 451, websites, new Rectangle(0, 257, 256, 256));
            this.AddLink("dining", 321, 451, websites, new Rectangle(257, 257, 256, 256));
            this.AddLink("imports", 587, 451, websites, new Rectangle(513, 257, 256, 256));
            this.AddLink("adventure", 853, 451, websites, new Rectangle(769, 257, 256, 256));
            this.AddLink("bundle", 1119, 451, websites, new Rectangle(1025, 257, 256, 256));

            this.AddLink("wizard", 55, 717, websites, new Rectangle(0, 513, 256, 256));
            this.AddLink("hats", 321, 717, websites, new Rectangle(257, 513, 256, 256));
            this.AddLink("hospital", 587, 717, websites, new Rectangle(513, 513, 256, 256));
            this.AddLink("krobus", 853, 717, websites, new Rectangle(769, 513, 256, 256));
            this.AddLink("artifact", 1119, 717, websites, new Rectangle(1025, 513, 256, 256));

            this.AddLink("dwarf", 55, 983, websites, new Rectangle(0, 769, 256, 256));
            this.AddLink("qi", 321, 983, websites, new Rectangle(257, 769, 256, 256));
            this.AddLink("sandy", 587, 983, websites, new Rectangle(513, 769, 256, 256));
            this.AddLink("joja", 853, 983, websites, new Rectangle(769, 769, 256, 256));
            this.AddLink("leah", 1119, 983, websites, new Rectangle(1025, 769, 256, 256));

            this.upperRightCloseButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 9 * Game1.pixelZoom, this.yPositionOnScreen - Game1.pixelZoom * 2, 12 * Game1.pixelZoom, 12 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), Game1.pixelZoom);
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
                            this.OpenLink(() => new ShopMenu(this.ItemUtils.GetBlacksmithStock(this.Unfiltered)), "Blacksmith");
                            break;

                        case "blacksmith_tools":
                            this.OpenLink(new ShopMenu(Utility.getBlacksmithUpgradeStock(Game1.player)));
                            break;

                        case "animals":
                            this.OpenLink(() => new ShopMenu(Utility.getAnimalShopStock()), "AnimalShop");
                            break;

                        case "animal_supplies":
                            if (Game1.currentLocation is AnimalHouse)
                                this.OpenLink(new MailOrderPigMenu(this.ItemUtils.GetPurchaseAnimalStock(), this.ItemUtils, this.OnLinkOpened, this.GetNewId));
                            else
                                this.OpenLink(new BuyAnimalMenu(Utility.getPurchaseAnimalStock(), this.OnLinkOpened, this.GetNewId));
                            break;

                        case "produce":
                            this.OpenLink(() => new ShopMenu(this.ItemUtils.GetShopStock(true, this.Unfiltered)), "SeedShop");
                            break;

                        case "carpentry":
                            this.OpenLink(() => new ShopMenu(this.ItemUtils.GetCarpenterStock(this.Unfiltered)), "ScienceHouse");
                            break;

                        case "carpentry_build":
                            this.OpenLink(new ConstructionMenu(false, this.OnLinkOpened));
                            break;

                        case "fish":
                            this.OpenLink(() => new ShopMenu(this.ItemUtils.GetFishShopStock(Game1.player, this.Unfiltered)), "FishShop");
                            break;

                        case "dining":
                            this.OpenLink(new ShopMenu(this.ItemUtils.GetSaloonStock(this.Unfiltered)));
                            break;

                        case "imports":
                            this.OpenLink(new ShopMenu(Utility.getTravelingMerchantStock((int)(Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed))));
                            break;

                        case "adventure":
                            this.OpenLink(() => new ShopMenu(this.GetAdventureShopStock()), "AdventureGuild");
                            break;

                        case "hats":
                            this.OpenLink(new ShopMenu(Utility.getHatStock()));
                            break;

                        case "hospital":
                            this.OpenLink(new ShopMenu(Utility.getHospitalStock()));
                            break;

                        case "wizard":
                            this.OpenLink(new ConstructionMenu(true, this.OnLinkOpened));
                            break;

                        case "dwarf":
                            this.OpenLink(new ShopMenu(Utility.getDwarfShopStock()));
                            break;

                        case "krobus":
                            this.OpenLink(new ShopMenu(((Sewer)Game1.getLocationFromName("Sewer")).getShadowShopStock(), 0, "Krobus"));
                            break;

                        case "qi":
                            this.OpenLink(new ShopMenu(Utility.getQiShopStock()));
                            break;

                        case "joja":
                            this.OpenLink(new ShopMenu(Utility.getJojaStock()));
                            break;

                        case "sandy":
                            this.OpenLink(new ShopMenu(this.ItemUtils.GetShopStock(false, this.Unfiltered)));
                            break;

                        case "sauce":
                            this.OpenLink(new ShopMenu(this.ItemUtils.GetRecipesStock(this.Unfiltered)));
                            break;

                        case "bundle":
                            this.OpenLink(new ShopMenu(this.ItemUtils.GetJunimoStock()));
                            break;

                        case "artifact":
                            this.OpenLink(new ShopMenu(this.ItemUtils.GetMineralsAndArtifactsStock(this.Unfiltered)));
                            break;

                        case "leah":
                            this.OpenLink(new ShopMenu(this.ItemUtils.GetLeahShopStock(this.Unfiltered), 0, "Leah"));
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
        /// <summary>Add a site link to the menu.</summary>
        /// <param name="name">The internal link name.</param>
        /// <param name="x">The pixel X position relative to the top-left corner of the menu.</param>
        /// <param name="y">The pixel Y position relative to the top-left corner of the menu.</param>
        /// <param name="texture">The texture containing the site image.</param>
        /// <param name="sourceArea">The area within the texture to draw.</param>
        private void AddLink(string name, int x, int y, Texture2D texture, Rectangle sourceArea)
        {
            ClickableTextureComponent component = new ClickableTextureComponent(
                bounds: new Rectangle((int)(this.xPositionOnScreen + x * this.Scale), (int)(this.yPositionOnScreen + y * this.Scale), (int)(sourceArea.Width * this.Scale), (int)(sourceArea.Height * this.Scale)),
                texture: texture,
                sourceRect: sourceArea,
                scale: this.Scale
            );
            component.name = name;
            this.LinksToVisit.Add(component);
        }

        /// <summary>Track and open a link menu.</summary>
        /// <param name="menu">The menu to open.</param>
        private void OpenLink(IClickableMenu menu)
        {
            this.OpenLink(() => menu);
        }

        /// <summary>Track and open a link menu.</summary>
        /// <param name="menu">The menu to open.</param>
        /// <param name="locationName">The location name to simulate.</param>
        [SuppressMessage("SMAPI", "AvoidNetField", Justification = "Net fields are accessed deliberately to bypass network sync.")]
        private void OpenLink(Func<IClickableMenu> menu, string locationName = null)
        {
            // close main menu
            this.exitThisMenu();

            // simulate location name if needed
            if (locationName != null && locationName != Game1.currentLocation.Name)
            {
                string prevLocationName = Game1.currentLocation.Name;
                try
                {
                    this.DirectlySetValue(Game1.currentLocation.name, locationName);
                    Game1.activeClickableMenu = menu();
                }
                finally
                {
                    this.DirectlySetValue(Game1.currentLocation.name, prevLocationName);
                }
            }
            else
                Game1.activeClickableMenu = menu();

            // track link opened
            this.OnLinkOpened();
        }

        /// <summary>Set a net string value without triggering sync logic.</summary>
        /// <param name="field">The net field to update.</param>
        /// <param name="value">The new value to set.</param>
        private void DirectlySetValue(NetString field, string value)
        {
            this.Reflection.GetField<string>(field, "value").SetValue(value);
        }

        private Dictionary<ISalable, int[]> GetAdventureShopStock()
        {
            Dictionary<ISalable, int[]> itemPriceAndStock = new Dictionary<ISalable, int[]>();
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
