using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;
using Microsoft.Xna.Framework.Input;
using SObject = StardewValley.Object;

namespace WhatAreYouMissing
{
    public enum TabName
    {
        SpecificSeasonTab = 0,
        SpecificCCSeasonTab = 1,
        CommonCCTab = 2,
        MerchantTab = 3,
        CookedItemsTab = 4

    }
    public class Menu : IClickableMenu
    {
        private int GetTabPosition(TabName name)
        {
            return (int)name + 1;
        }

        public const int REGION_SEASON_TAB = 16430;
        public const int REGION_CC_SEASON_TAB = 16431;
        public const int REGION_CC_COMMON_TAB = 16432;
        public const int REGION_MERCHANT_TAB = 16433;
        public const int REGION_RECIPES_TAB = 16434;


        public const int NUM_TABS = 5;

        public int CurrentTab;
        private bool invisible;
        private string HoverText;
        public static bool ForcePreventClose;

        private ConfigOptions Config;

        private List<ClickableComponent> tabs;
        private List<MenuTab> pages;
        private List<int> tabIconParentSheetIndices;

        public Menu() : base(Game1.viewport.Width / 2 - (800 + borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + borderWidth * 2) / 2, 800 + borderWidth * 2, 600 + borderWidth * 2, true)
        {
            Config = ModEntry.Config;
            tabs = new List<ClickableComponent>();
            pages = new List<MenuTab>();
            tabIconParentSheetIndices = new List<int>();
            invisible = false;
            HoverText = "";
            CurrentTab = (int)TabName.SpecificSeasonTab;
            ForcePreventClose = false;
            AddTabs();
            AddPages();
        }

        private void AddTabs()
        {
            tabs.Add(new ClickableComponent(MakeTabRectangle(GetTabPosition(TabName.SpecificSeasonTab)), TabName.SpecificSeasonTab.ToString(), Utilities.GetTranslation("SEASON_SPECIFIC_DESCRIPTION"))
            {
                myID = 16430,
                downNeighborID = 0,
                rightNeighborID = 16431,
                tryDefaultIfNoDownNeighborExists = true,
                fullyImmutable = true
            });
            tabIconParentSheetIndices.Add(Constants.PARSNIP_SEEDS);

            tabs.Add(new ClickableComponent(MakeTabRectangle(GetTabPosition(TabName.SpecificCCSeasonTab)), TabName.SpecificCCSeasonTab.ToString(), Utilities.GetTranslation("SEASON_SPECIFIC_CC_DESCRIPTION"))
            {
                myID = 16431,
                downNeighborID = 1,
                rightNeighborID = 16432,
                leftNeighborID = 16430,
                tryDefaultIfNoDownNeighborExists = true,
                fullyImmutable = true
            });
            tabIconParentSheetIndices.Add(Constants.MELON);

            tabs.Add(new ClickableComponent(MakeTabRectangle(GetTabPosition(TabName.CommonCCTab)), TabName.CommonCCTab.ToString(), Utilities.GetTranslation("COMMON_CC_DESCRIPTION"))
            {
                myID = 16432,
                downNeighborID = 2,
                rightNeighborID = 16433,
                leftNeighborID = 16431,
                tryDefaultIfNoDownNeighborExists = true,
                fullyImmutable = true
            });
            tabIconParentSheetIndices.Add(Constants.JELLY);

            tabs.Add(new ClickableComponent(MakeTabRectangle(GetTabPosition(TabName.MerchantTab)), TabName.MerchantTab.ToString(), Utilities.GetTranslation("MERCHANT_DESCRIPTION"))
            {
                myID = 16433,
                downNeighborID = 3,
                leftNeighborID = 16432,
                tryDefaultIfNoDownNeighborExists = true,
                fullyImmutable = true
            });
            tabIconParentSheetIndices.Add(Constants.RARE_SEED);

            tabs.Add(new ClickableComponent(MakeTabRectangle(GetTabPosition(TabName.CookedItemsTab)), TabName.CookedItemsTab.ToString(), Utilities.GetTranslation("COOKED_ITEMS_DESCRIPTION"))
            {
                myID = 16434,
                leftNeighborID = 16433,
                tryDefaultIfNoDownNeighborExists = true,
                fullyImmutable = true
            });
            tabIconParentSheetIndices.Add(Constants.SPICY_EEL);
        }

        private Rectangle MakeTabRectangle(int tabPosistion)
        {
            return new Rectangle(xPositionOnScreen + Game1.tileSize * tabPosistion, yPositionOnScreen + tabYPositionRelativeToMenuY + Game1.tileSize, Game1.tileSize, Game1.tileSize);
        }

        private void AddPages()
        {
            pages.Add(new MenuTab(xPositionOnScreen, yPositionOnScreen, width, height, TabName.SpecificSeasonTab, ModEntry.MissingItems.GetMissingSpecifics()));
            pages.Add(new MenuTab(xPositionOnScreen, yPositionOnScreen, width, height, TabName.SpecificCCSeasonTab, ModEntry.MissingItems.GetMissingSpecificCCItems()));
            pages.Add(new MenuTab(xPositionOnScreen, yPositionOnScreen, width, height, TabName.CommonCCTab, ModEntry.MissingItems.GetMissingCommonCCItems()));

            List<SObject> allMissingMerchantItems = ModEntry.MissingItems.GetMissingMerchantCCItems();
            allMissingMerchantItems.AddRange(ModEntry.MissingItems.GetMissingMerchantItems());
            pages.Add(new MenuTab(xPositionOnScreen, yPositionOnScreen, width, height, TabName.MerchantTab, allMissingMerchantItems ));

            pages.Add(new MenuTab(xPositionOnScreen, yPositionOnScreen, width, height, TabName.CookedItemsTab, ModEntry.MissingItems.GetMissingRecipes()));
        }

        public override void draw(SpriteBatch b)
        {
            if (!invisible)
            {
                if (!Game1.options.showMenuBackground)
                    b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
                Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, pages[CurrentTab].width, pages[CurrentTab].height, false, true, (string)null, false);
                pages[CurrentTab].draw(b);
                b.End();
                
                if (!ForcePreventClose)
                {
                    for (int i = 0; i < tabs.Count; ++i)
                    {
                        b.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);

                        ClickableComponent tab = tabs[i];
                        int SpriteXCoord = tab.bounds.X + (int)(Game1.tileSize / 3.25) + 3;
                        int SpriteYCoord = tab.bounds.Y + (int)(Game1.tileSize / 2.25) + (CurrentTab == i ? 8 : 0);
                        b.Draw(Game1.mouseCursors, new Vector2(tab.bounds.X, tab.bounds.Y + (CurrentTab == i ? 8 : 0)), new Rectangle?(new Rectangle(16, 368, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0001f);
                        b.Draw(Game1.objectSpriteSheet, new Vector2(SpriteXCoord, SpriteYCoord), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, tabIconParentSheetIndices[i], 16, 16)), Color.White, 0f, new Vector2(4f, 4f), 2.5f, SpriteEffects.None, 0.9f);
                        if(i == (int)TabName.SpecificCCSeasonTab)
                        {
                            float scaleSize = 1.75f;
                            b.Draw(Game1.mouseCursors, new Vector2(tab.bounds.X + 20f, tab.bounds.Y + (CurrentTab == i ? 8 : 0) + (float)(Game1.tileSize - 12)), new Rectangle?(new Rectangle(346, 400, 8, 8)), Color.White * 1, 0.0f, new Vector2(4f, 4f), scaleSize, SpriteEffects.None, 1f);
                            int numberXCoord = tab.bounds.X + Game1.tileSize - 20;
                            int numberYCoord = tab.bounds.Y + Game1.tileSize - 16 + +(CurrentTab == i ? 8 : 0);
                            Utility.drawTinyDigits(5, b, new Vector2(numberXCoord, numberYCoord), scaleSize, 0.95f, Color.White);
                        }
                        b.End();
                    }
                    b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                    if (!HoverText.Equals(""))
                    {
                        Utilities.DrawHoverTextBox(b, HoverText, 4);
                    }
                }
            }
            else
                pages[CurrentTab].draw(b);
            if (!ForcePreventClose)
                base.draw(b);
            if (Game1.options.hardwareCursor)
                return;
            b.Draw(Game1.mouseCursors, new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16)), Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
        }

        private void ChangeTab(int tabIndex)
        {
            CurrentTab = tabIndex;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (upperRightCloseButton.containsPoint(x, y))
            {
                if (playSound)
                    Game1.playSound("bigDeSelect");
                Game1.exitActiveMenu();
                return;
            }

            for (int i = 0; i < NUM_TABS; ++i)
            {
                ClickableComponent tab = tabs[i];
                if (tab.containsPoint(x, y) && CurrentTab != i)
                {
                    ChangeTab(i);
                }
            }
            pages[CurrentTab].receiveLeftClick(x, y, playSound);
        }

        public override void receiveScrollWheelAction(int direction)
        {
            pages[CurrentTab].receiveScrollWheelAction(direction);
        }

        public override void leftClickHeld(int x, int y)
        {
            pages[CurrentTab].leftClickHeld(x, y);
        }

        public override void releaseLeftClick(int x, int y)
        {
            pages[CurrentTab].releaseLeftClick(x, y);
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            HoverText = "";
            pages[CurrentTab].performHoverAction(x, y);
            HoverText = pages[CurrentTab].HoverText;
            foreach (ClickableComponent tab in tabs)
            {
                if (tab.containsPoint(x, y))
                {
                    HoverText = tab.label;
                    return;
                }
            }
            return;
        }

    }
}
