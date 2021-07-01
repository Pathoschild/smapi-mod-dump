/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jdusbabek/stardewvalley
**
*************************************************/

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
        public PelicanFiberMenu(Texture2D websites, IReflectionHelper reflection, ItemUtils itemUtils,
            Func<long> getNewId, Action onLinkOpened, float scale = 1.0f, bool unfiltered = true)
            : base(Game1.uiViewport.Width / 2 - (int) (MenuWidth * scale) / 2 - borderWidth * 2,
                Game1.uiViewport.Height / 2 - (int) (MenuHeight * scale) / 2 - borderWidth * 2,
                (int) (MenuWidth * scale) + borderWidth * 2,
                (int) (MenuHeight * scale) + borderWidth, true)
        {
            Reflection = reflection;
            ItemUtils = itemUtils;
            GetNewId = getNewId;
            OnLinkOpened = onLinkOpened;

            height += Game1.tileSize;
            Scale = scale;
            Unfiltered = unfiltered;

            AddLink("blacksmith_tools", 55, 185, websites, new Rectangle(0, 0, 256, 128));
            AddLink("blacksmith", 55, 313, websites, new Rectangle(0, 128, 256, 128));
            AddLink("animals", 321, 185, websites, new Rectangle(257, 0, 256, 128));
            AddLink("animal_supplies", 321, 313, websites, new Rectangle(257, 128, 256, 128));
            AddLink("produce", 587, 185, websites, new Rectangle(513, 0, 256, 256));
            AddLink("carpentry_build", 853, 185, websites, new Rectangle(769, 0, 256, 128));
            AddLink("carpentry", 853, 313, websites, new Rectangle(769, 128, 256, 128));
            AddLink("sauce", 1119, 185, websites, new Rectangle(1025, 0, 256, 256));

            AddLink("fish", 55, 451, websites, new Rectangle(0, 257, 256, 256));
            AddLink("dining", 321, 451, websites, new Rectangle(257, 257, 256, 256));
            AddLink("imports", 587, 451, websites, new Rectangle(513, 257, 256, 256));
            AddLink("adventure", 853, 451, websites, new Rectangle(769, 257, 256, 256));
            AddLink("bundle", 1119, 451, websites, new Rectangle(1025, 257, 256, 256));

            AddLink("wizard", 55, 717, websites, new Rectangle(0, 513, 256, 256));
            AddLink("hats", 321, 717, websites, new Rectangle(257, 513, 256, 256));
            AddLink("hospital", 587, 717, websites, new Rectangle(513, 513, 256, 256));
            AddLink("krobus", 853, 717, websites, new Rectangle(769, 513, 256, 256));
            AddLink("artifact", 1119, 717, websites, new Rectangle(1025, 513, 256, 256));

            AddLink("dwarf", 55, 983, websites, new Rectangle(0, 769, 256, 256));
            AddLink("qi", 321, 983, websites, new Rectangle(257, 769, 256, 256));
            AddLink("sandy", 587, 983, websites, new Rectangle(513, 769, 256, 256));
            AddLink("joja", 853, 983, websites, new Rectangle(769, 769, 256, 256));
            AddLink("leah", 1119, 983, websites, new Rectangle(1025, 769, 256, 256));

            upperRightCloseButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + width - 9 * Game1.pixelZoom, yPositionOnScreen - Game1.pixelZoom * 2,
                    12 * Game1.pixelZoom, 12 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(337, 494, 12, 12),
                Game1.pixelZoom);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);

            foreach (var textureComponent in LinksToVisit)
                if (textureComponent.containsPoint(x, y))
                    switch (textureComponent.name)
                    {
                        case "blacksmith":
                            OpenLink(() => new ShopMenu(ItemUtils.GetBlacksmithStock(Unfiltered)), "Blacksmith");
                            break;

                        case "blacksmith_tools":
                            OpenLink(new ShopMenu(Utility.getBlacksmithUpgradeStock(Game1.player)));
                            break;

                        case "animals":
                            OpenLink(() => new ShopMenu(Utility.getAnimalShopStock()), "AnimalShop");
                            break;

                        case "animal_supplies":
                            if (Game1.currentLocation is AnimalHouse)
                                OpenLink(new MailOrderPigMenu(ItemUtils.GetPurchaseAnimalStock(), ItemUtils,
                                    OnLinkOpened, GetNewId));
                            else
                                OpenLink(new BuyAnimalMenu(Utility.getPurchaseAnimalStock(), OnLinkOpened, GetNewId));
                            break;

                        case "produce":
                            OpenLink(() => new ShopMenu(ItemUtils.GetShopStock(true, Unfiltered)), "SeedShop");
                            break;

                        case "carpentry":
                            OpenLink(() => new ShopMenu(ItemUtils.GetCarpenterStock(Unfiltered)), "ScienceHouse");
                            break;

                        case "carpentry_build":
                            OpenLink(new ConstructionMenu(false, OnLinkOpened));
                            break;

                        case "fish":
                            OpenLink(() => new ShopMenu(ItemUtils.GetFishShopStock(Game1.player, Unfiltered)),
                                "FishShop");
                            break;

                        case "dining":
                            OpenLink(new ShopMenu(ItemUtils.GetSaloonStock(Unfiltered)));
                            break;

                        case "imports":
                            OpenLink(new ShopMenu(
                                Utility.getTravelingMerchantStock((int) (Game1.uniqueIDForThisGame +
                                                                         Game1.stats.DaysPlayed))));
                            break;

                        case "adventure":
                            OpenLink(() => new ShopMenu(GetAdventureShopStock()), "AdventureGuild");
                            break;

                        case "hats":
                            OpenLink(new ShopMenu(Utility.getHatStock()));
                            break;

                        case "hospital":
                            OpenLink(new ShopMenu(Utility.getHospitalStock()));
                            break;

                        case "wizard":
                            OpenLink(new ConstructionMenu(true, OnLinkOpened));
                            break;

                        case "dwarf":
                            OpenLink(new ShopMenu(Utility.getDwarfShopStock()));
                            break;

                        case "krobus":
                            OpenLink(new ShopMenu(((Sewer) Game1.getLocationFromName("Sewer")).getShadowShopStock(), 0,
                                "Krobus"));
                            break;

                        case "qi":
                            OpenLink(new ShopMenu(Utility.getQiShopStock()));
                            break;

                        case "joja":
                            OpenLink(new ShopMenu(Utility.getJojaStock()));
                            break;

                        case "sandy":
                            OpenLink(new ShopMenu(ItemUtils.GetShopStock(false, Unfiltered)));
                            break;

                        case "sauce":
                            OpenLink(new ShopMenu(ItemUtils.GetRecipesStock(Unfiltered)));
                            break;

                        case "bundle":
                            OpenLink(new ShopMenu(ItemUtils.GetJunimoStock()));
                            break;

                        case "artifact":
                            OpenLink(new ShopMenu(ItemUtils.GetMineralsAndArtifactsStock(Unfiltered)));
                            break;

                        case "leah":
                            OpenLink(new ShopMenu(ItemUtils.GetLeahShopStock(Unfiltered), 0, "Leah"));
                            break;
                    }
        }

        public override void performHoverAction(int x, int y)
        {
            upperRightCloseButton.tryHover(x, y, 0.5f);

            foreach (var textureComponent in LinksToVisit)
                textureComponent.scale = textureComponent.containsPoint(x, y)
                    ? Math.Min(textureComponent.scale + 0.05f, textureComponent.baseScale - 0.05f)
                    : Math.Max(textureComponent.baseScale, textureComponent.scale - 0.025f);
        }

        public override void draw(SpriteBatch b)
        {
            if (!Game1.dialogueUp && !Game1.globalFade)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

                if (Scale > .9f)
                    SpriteText.drawStringWithScrollCenteredAt(b, "PelicanFiber 3.0 (a subsidiary of JojaNet, Inc.)",
                        Game1.uiViewport.Width / 2, (int) (yPositionOnScreen * Scale));
                else
                    SpriteText.drawStringWithScrollCenteredAt(b, "PelicanFiber 3.0 (a subsidiary of JojaNet, Inc.)",
                        Game1.uiViewport.Width / 2, (int) (yPositionOnScreen * Scale) + 25);

                Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);

                if (Scale < 1.0f)
                    SpriteText.drawStringHorizontallyCenteredAt(b, "Click a Link Below to Shop Online",
                        Game1.uiViewport.Width / 2, (int) (yPositionOnScreen + 92 * Scale) + 35);
                else
                    SpriteText.drawStringHorizontallyCenteredAt(b, "Click a Link Below to Shop Online",
                        Game1.uiViewport.Width / 2, (int) (yPositionOnScreen + 92 * Scale));

                Game1.dayTimeMoneyBox.drawMoneyBox(b);
                foreach (var textureComponent in LinksToVisit)
                    textureComponent.draw(b);

                SpriteText.drawStringHorizontallyCenteredAt(b, "Blacksmith", (int) (xPositionOnScreen + 182 * Scale),
                    (int) (yPositionOnScreen + 381 * Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Animals", (int) (xPositionOnScreen + 448 * Scale),
                    (int) (yPositionOnScreen + 381 * Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Produce", (int) (xPositionOnScreen + 714 * Scale),
                    (int) (yPositionOnScreen + 381 * Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Carpentry", (int) (xPositionOnScreen + 980 * Scale),
                    (int) (yPositionOnScreen + 381 * Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Recipes", (int) (xPositionOnScreen + 1246 * Scale),
                    (int) (yPositionOnScreen + 381 * Scale), 999999, -1, 999999, 1, 0.88f, false, 1);

                SpriteText.drawStringHorizontallyCenteredAt(b, "Upgrades", (int) (xPositionOnScreen + 182 * Scale),
                    (int) (yPositionOnScreen + 211 * Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Supplies", (int) (xPositionOnScreen + 448 * Scale),
                    (int) (yPositionOnScreen + 211 * Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                //SpriteText.drawStringHorizontallyCenteredAt(b, "Produce", (int)(this.xPositionOnScreen + 714 * scale), (int)(this.yPositionOnScreen + 381 * scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Buildings", (int) (xPositionOnScreen + 980 * Scale),
                    (int) (yPositionOnScreen + 211 * Scale), 999999, -1, 999999, 1, 0.88f, false, 1);

                SpriteText.drawStringHorizontallyCenteredAt(b, "Fish", (int) (xPositionOnScreen + 182 * Scale),
                    (int) (yPositionOnScreen + 643 * Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Dining", (int) (xPositionOnScreen + 448 * Scale),
                    (int) (yPositionOnScreen + 643 * Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Imports", (int) (xPositionOnScreen + 714 * Scale),
                    (int) (yPositionOnScreen + 643 * Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Adventure", (int) (xPositionOnScreen + 980 * Scale),
                    (int) (yPositionOnScreen + 643 * Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Bundles", (int) (xPositionOnScreen + 1246 * Scale),
                    (int) (yPositionOnScreen + 643 * Scale), 999999, -1, 999999, 1, 0.88f, false, 1);


                SpriteText.drawStringHorizontallyCenteredAt(b, "Wizard", (int) (xPositionOnScreen + 182 * Scale),
                    (int) (yPositionOnScreen + 905 * Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Hats", (int) (xPositionOnScreen + 448 * Scale),
                    (int) (yPositionOnScreen + 905 * Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Hospital", (int) (xPositionOnScreen + 714 * Scale),
                    (int) (yPositionOnScreen + 905 * Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Krobus", (int) (xPositionOnScreen + 980 * Scale),
                    (int) (yPositionOnScreen + 905 * Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Artifacts", (int) (xPositionOnScreen + 1246 * Scale),
                    (int) (yPositionOnScreen + 905 * Scale), 999999, -1, 999999, 1, 0.88f, false, 1);

                SpriteText.drawStringHorizontallyCenteredAt(b, "Dwarf", (int) (xPositionOnScreen + 182 * Scale),
                    (int) (yPositionOnScreen + 1167 * Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Qi", (int) (xPositionOnScreen + 448 * Scale),
                    (int) (yPositionOnScreen + 1167 * Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Oaisis", (int) (xPositionOnScreen + 714 * Scale),
                    (int) (yPositionOnScreen + 1167 * Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Joja", (int) (xPositionOnScreen + 980 * Scale),
                    (int) (yPositionOnScreen + 1167 * Scale), 999999, -1, 999999, 1, 0.88f, false, 1);

                SpriteText.drawStringHorizontallyCenteredAt(b, "Foraged", (int) (xPositionOnScreen + 1246 * Scale),
                    (int) (yPositionOnScreen + 997 * Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "by", (int) (xPositionOnScreen + 1246 * Scale),
                    (int) (yPositionOnScreen + 1082 * Scale), 999999, -1, 999999, 1, 0.88f, false, 1);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Leah", (int) (xPositionOnScreen + 1246 * Scale),
                    (int) (yPositionOnScreen + 1167 * Scale), 999999, -1, 999999, 1, 0.88f, false, 1);


                SpriteText.drawStringHorizontallyCenteredAt(b, "Blacksmith", (int) (xPositionOnScreen + 180 * Scale),
                    (int) (yPositionOnScreen + 379 * Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Animals", (int) (xPositionOnScreen + 446 * Scale),
                    (int) (yPositionOnScreen + 379 * Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Produce", (int) (xPositionOnScreen + 712 * Scale),
                    (int) (yPositionOnScreen + 379 * Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Carpentry", (int) (xPositionOnScreen + 978 * Scale),
                    (int) (yPositionOnScreen + 379 * Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Recipes", (int) (xPositionOnScreen + 1244 * Scale),
                    (int) (yPositionOnScreen + 379 * Scale), 999999, -1, 999999, 1, 0.88f, false, 4);


                SpriteText.drawStringHorizontallyCenteredAt(b, "Upgrades", (int) (xPositionOnScreen + 180 * Scale),
                    (int) (yPositionOnScreen + 209 * Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Supplies", (int) (xPositionOnScreen + 446 * Scale),
                    (int) (yPositionOnScreen + 209 * Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                //SpriteText.drawStringHorizontallyCenteredAt(b, "Produce", (int)(this.xPositionOnScreen + 712 * scale), (int)(this.yPositionOnScreen + 379 * scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Buildings", (int) (xPositionOnScreen + 978 * Scale),
                    (int) (yPositionOnScreen + 209 * Scale), 999999, -1, 999999, 1, 0.88f, false, 4);

                SpriteText.drawStringHorizontallyCenteredAt(b, "Fish", (int) (xPositionOnScreen + 180 * Scale),
                    (int) (yPositionOnScreen + 641 * Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Dining", (int) (xPositionOnScreen + 446 * Scale),
                    (int) (yPositionOnScreen + 641 * Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Imports", (int) (xPositionOnScreen + 712 * Scale),
                    (int) (yPositionOnScreen + 641 * Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Adventure", (int) (xPositionOnScreen + 978 * Scale),
                    (int) (yPositionOnScreen + 641 * Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Bundles", (int) (xPositionOnScreen + 1244 * Scale),
                    (int) (yPositionOnScreen + 641 * Scale), 999999, -1, 999999, 1, 0.88f, false, 4);


                SpriteText.drawStringHorizontallyCenteredAt(b, "Wizard", (int) (xPositionOnScreen + 180 * Scale),
                    (int) (yPositionOnScreen + 903 * Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Hats", (int) (xPositionOnScreen + 446 * Scale),
                    (int) (yPositionOnScreen + 903 * Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Hospital", (int) (xPositionOnScreen + 712 * Scale),
                    (int) (yPositionOnScreen + 903 * Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Krobus", (int) (xPositionOnScreen + 978 * Scale),
                    (int) (yPositionOnScreen + 903 * Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Artifacts", (int) (xPositionOnScreen + 1244 * Scale),
                    (int) (yPositionOnScreen + 903 * Scale), 999999, -1, 999999, 1, 0.88f, false, 4);

                SpriteText.drawStringHorizontallyCenteredAt(b, "Dwarf", (int) (xPositionOnScreen + 180 * Scale),
                    (int) (yPositionOnScreen + 1165 * Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Qi", (int) (xPositionOnScreen + 448 * Scale),
                    (int) (yPositionOnScreen + 1165 * Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Oaisis", (int) (xPositionOnScreen + 714 * Scale),
                    (int) (yPositionOnScreen + 1165 * Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Joja", (int) (xPositionOnScreen + 980 * Scale),
                    (int) (yPositionOnScreen + 1165 * Scale), 999999, -1, 999999, 1, 0.88f, false, 4);

                SpriteText.drawStringHorizontallyCenteredAt(b, "Foraged", (int) (xPositionOnScreen + 1244 * Scale),
                    (int) (yPositionOnScreen + 995 * Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "by", (int) (xPositionOnScreen + 1244 * Scale),
                    (int) (yPositionOnScreen + 1080 * Scale), 999999, -1, 999999, 1, 0.88f, false, 4);
                SpriteText.drawStringHorizontallyCenteredAt(b, "Leah", (int) (xPositionOnScreen + 1244 * Scale),
                    (int) (yPositionOnScreen + 1165 * Scale), 999999, -1, 999999, 1, 0.88f, false, 4);

                upperRightCloseButton.draw(b);
            }

            drawMouse(b);
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
            var component = new ClickableTextureComponent(
                new Rectangle((int) (xPositionOnScreen + x * Scale), (int) (yPositionOnScreen + y * Scale),
                    (int) (sourceArea.Width * Scale), (int) (sourceArea.Height * Scale)),
                texture,
                sourceArea,
                Scale
            );
            component.name = name;
            LinksToVisit.Add(component);
        }

        /// <summary>Track and open a link menu.</summary>
        /// <param name="menu">The menu to open.</param>
        private void OpenLink(IClickableMenu menu)
        {
            OpenLink(() => menu);
        }

        /// <summary>Track and open a link menu.</summary>
        /// <param name="menu">The menu to open.</param>
        /// <param name="locationName">The location name to simulate.</param>
        [SuppressMessage("SMAPI", "AvoidNetField",
            Justification = "Net fields are accessed deliberately to bypass network sync.")]
        private void OpenLink(Func<IClickableMenu> menu, string locationName = null)
        {
            // close main menu
            exitThisMenu();

            // simulate location name if needed
            if (locationName != null && locationName != Game1.currentLocation.Name)
            {
                var prevLocationName = Game1.currentLocation.Name;
                try
                {
                    DirectlySetValue(Game1.currentLocation.name, locationName);
                    Game1.activeClickableMenu = menu();
                }
                finally
                {
                    DirectlySetValue(Game1.currentLocation.name, prevLocationName);
                }
            }
            else
            {
                Game1.activeClickableMenu = menu();
            }

            // track link opened
            OnLinkOpened();
        }

        /// <summary>Set a net string value without triggering sync logic.</summary>
        /// <param name="field">The net field to update.</param>
        /// <param name="value">The new value to set.</param>
        private void DirectlySetValue(NetString field, string value)
        {
            Reflection.GetField<string>(field, "value").SetValue(value);
        }

        private Dictionary<ISalable, int[]> GetAdventureShopStock()
        {
            var itemPriceAndStock = new Dictionary<ISalable, int[]>();
            var maxValue = int.MaxValue;
            itemPriceAndStock.Add(new MeleeWeapon(12), new[] {250, maxValue});
            if (Game1.mine != null)
            {
                if (MineShaft.lowestLevelReached >= 15)
                    itemPriceAndStock.Add(new MeleeWeapon(17), new[] {500, maxValue});
                if (MineShaft.lowestLevelReached >= 20)
                    itemPriceAndStock.Add(new MeleeWeapon(1), new[] {750, maxValue});
                if (MineShaft.lowestLevelReached >= 25)
                {
                    itemPriceAndStock.Add(new MeleeWeapon(43), new[] {850, maxValue});
                    itemPriceAndStock.Add(new MeleeWeapon(44), new[] {1500, maxValue});
                }

                if (MineShaft.lowestLevelReached >= 40)
                    itemPriceAndStock.Add(new MeleeWeapon(27), new[] {2000, maxValue});
                if (MineShaft.lowestLevelReached >= 45)
                    itemPriceAndStock.Add(new MeleeWeapon(10), new[] {2000, maxValue});
                if (MineShaft.lowestLevelReached >= 55)
                    itemPriceAndStock.Add(new MeleeWeapon(7), new[] {4000, maxValue});
                if (MineShaft.lowestLevelReached >= 75)
                    itemPriceAndStock.Add(new MeleeWeapon(5), new[] {6000, maxValue});
                if (MineShaft.lowestLevelReached >= 90)
                    itemPriceAndStock.Add(new MeleeWeapon(50), new[] {9000, maxValue});
                if (MineShaft.lowestLevelReached >= 120)
                    itemPriceAndStock.Add(new MeleeWeapon(9), new[] {25000, maxValue});
                if (Game1.player.mailReceived.Contains("galaxySword"))
                {
                    itemPriceAndStock.Add(new MeleeWeapon(4), new[] {50000, maxValue});
                    itemPriceAndStock.Add(new MeleeWeapon(23), new[] {350000, maxValue});
                    itemPriceAndStock.Add(new MeleeWeapon(29), new[] {75000, maxValue});
                }
            }

            itemPriceAndStock.Add(new Boots(504), new[] {500, maxValue});
            if (Game1.mine != null && MineShaft.lowestLevelReached >= 40)
                itemPriceAndStock.Add(new Boots(508), new[] {1250, maxValue});
            if (Game1.mine != null && MineShaft.lowestLevelReached >= 80)
                itemPriceAndStock.Add(new Boots(511), new[] {2500, maxValue});
            itemPriceAndStock.Add(new Ring(529), new[] {1000, maxValue});
            itemPriceAndStock.Add(new Ring(530), new[] {1000, maxValue});
            if (Game1.mine != null && MineShaft.lowestLevelReached >= 40)
            {
                itemPriceAndStock.Add(new Ring(531), new[] {2500, maxValue});
                itemPriceAndStock.Add(new Ring(532), new[] {2500, maxValue});
            }

            if (Game1.mine != null && MineShaft.lowestLevelReached >= 80)
            {
                itemPriceAndStock.Add(new Ring(533), new[] {5000, maxValue});
                itemPriceAndStock.Add(new Ring(534), new[] {5000, maxValue});
            }

            if (Game1.player.hasItemWithNameThatContains("Slingshot") != null)
                itemPriceAndStock.Add(new Object(441, int.MaxValue), new[] {100, maxValue});
            if (Game1.player.mailReceived.Contains("Gil_Slime Charmer Ring"))
                itemPriceAndStock.Add(new Ring(520), new[] {25000, maxValue});
            if (Game1.player.mailReceived.Contains("Gil_Savage Ring"))
                itemPriceAndStock.Add(new Ring(523), new[] {25000, maxValue});
            if (Game1.player.mailReceived.Contains("Gil_Burglar's Ring"))
                itemPriceAndStock.Add(new Ring(526), new[] {20000, maxValue});
            if (Game1.player.mailReceived.Contains("Gil_Vampire Ring"))
                itemPriceAndStock.Add(new Ring(522), new[] {15000, maxValue});
            if (Game1.player.mailReceived.Contains("Gil_Skeleton Mask"))
                itemPriceAndStock.Add(new Hat(8), new[] {20000, maxValue});
            if (Game1.player.mailReceived.Contains("Gil_Hard Hat"))
                itemPriceAndStock.Add(new Hat(27), new[] {20000, maxValue});
            if (Game1.player.mailReceived.Contains("Gil_Insect Head"))
                itemPriceAndStock.Add(new MeleeWeapon(13), new[] {10000, maxValue});
            //Game1.activeClickableMenu = (IClickableMenu)new ShopMenu(itemPriceAndStock, 0, "Marlon");

            return itemPriceAndStock;
        }
    }
}