using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//Created by Musbah Sinno

//Resources:Got GetHoveredItemFromMenu and DrawHoverTextbox from a CJB mod and modified them to suit my needs.
//          They also inspired me to make GetHoveredItemFromToolbar, so thank you CJB
//https://github.com/CJBok/SDV-Mods/blob/master/CJBShowItemSellPrice/StardewCJB.cs

namespace StardewValleyBundleTooltips
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        //Needed to make sure essential variables are loaded before running what needs them
        bool isLoaded = false;

        // check if a mod is loaded
        bool isCJBSellItemPriceLoaded;

        private bool isUiInfoSuiteLoaded; //Check to see if UiInfoSuiteIsLoaded

        Item toolbarItem;
        List<int> itemsInBundles;
        Dictionary<int, int[][]> bundles;
        Dictionary<int, string[]> bundleNamesAndSubNames;

        string language = "";


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            isCJBSellItemPriceLoaded = this.Helper.ModRegistry.IsLoaded("CJBok.ShowItemSellPrice");
            isUiInfoSuiteLoaded = this.Helper.ModRegistry.IsLoaded("Cdaragorn.UiInfoSuite");

            //Events
            helper.Events.GameLoop.SaveLoaded += SaveEvents_AfterLoad;
            helper.Events.Display.RenderedHud += GraphicsEvents_OnPostRenderHudEvent;
            helper.Events.Display.RenderingHud += GraphicsEvents_OnPreRenderHudEvent;
            helper.Events.Display.RenderedActiveMenu += GraphicsEvents_OnPostRenderGuiEvent;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked when the player presses a keyboard button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// 
        private void SaveEvents_AfterLoad(object sender, SaveLoadedEventArgs e)
        {
            //This will be filled with the itemIDs of every item in every bundle (for a fast search without details)
            itemsInBundles = new List<int>();
            bundles = getBundles();

            //remove duplicates
            itemsInBundles = new HashSet<int>(itemsInBundles).ToList();

            isLoaded = true;
        }

        private void GraphicsEvents_OnPreRenderHudEvent(object sender, RenderingHudEventArgs e)
        {
            //I have to get it on preRendering because it gets set to null post
            toolbarItem = GetHoveredItemFromToolbar();
        }

        private void GraphicsEvents_OnPostRenderHudEvent(object sender, RenderedHudEventArgs e)
        {
            if (isLoaded && !Game1.MasterPlayer.mailReceived.Contains("JojaMember") && Game1.activeClickableMenu == null && toolbarItem != null)
            {
                PopulateHoverTextBoxAndDraw(toolbarItem,true);
                toolbarItem = null;
            }
        }

        private void GraphicsEvents_OnPostRenderGuiEvent(object sender, RenderedActiveMenuEventArgs e)
        {
            if (isLoaded && !Game1.MasterPlayer.mailReceived.Contains("JojaMember") && Game1.activeClickableMenu != null)
            {
                Item item = this.GetHoveredItemFromMenu(Game1.activeClickableMenu);
                if (item != null)
                    PopulateHoverTextBoxAndDraw(item,false);
            }
        }

        private void PopulateHoverTextBoxAndDraw(Item item, bool isItFromToolbar)
        {
            StardewValley.Locations.CommunityCenter communityCenter = Game1.getLocationFromName("CommunityCenter") as StardewValley.Locations.CommunityCenter;

            List<int[]> itemInfo = new List<int[]>();
            Dictionary<string, List<string>> descriptions = new Dictionary<string, List<string>>();

            foreach (int itemInBundles in itemsInBundles)
            {
                if (item.ParentSheetIndex == itemInBundles)
                {
                    foreach (KeyValuePair<int, int[][]> bundle in bundles)
                    {
                        for (int i = 0; i < bundle.Value.Length; i++)
                        {
                            //Getting the item name because the bundle itself doesn't actually make sure that the correct item is being placed
                            //(parentSheetIndex of object can overlap with another item from another sheet)
                            string itemName = "";
                            if (Game1.objectInformation.ContainsKey(bundle.Value[i][0]))
                            {
                                if(language == "")
                                    itemName = Game1.objectInformation[bundle.Value[i][0]].Split('/')[0];
                                else
                                    itemName = Game1.objectInformation[bundle.Value[i][0]].Split('/')[4];
                            }

                            var isItemInBundleSlot = communityCenter.bundles[bundle.Key][bundle.Value[i][3]];
                            if ((item is StardewValley.Object) && item.Stack != 0 && bundle.Value[i] != null && bundle.Value[i][0] == item.ParentSheetIndex && itemName == item.DisplayName && bundle.Value[i][2] <= ((StardewValley.Object)item).Quality)
                            {
                                if(!isItemInBundleSlot)
                                {
                                    //Saving i to check if the items are the same or not later on
                                    itemInfo.Add(new int[] {bundle.Key,bundle.Value[i][1],i});
                                    descriptions[bundleNamesAndSubNames[bundle.Key][0]] = new List<string>();
                                }
                            }
                        }
                    }
                }
            }

            foreach (int[] info in itemInfo)
            {
                string bundleName = bundleNamesAndSubNames[info[0]][0];
                string bundleSubName = bundleNamesAndSubNames[info[0]][1];
                int quantity = info[1];

                descriptions[bundleName].Add(bundleSubName + " | Qty: " + quantity);
            }

            if (descriptions.Count > 0)
            {
                string tooltipText = "";
                int count = 0;

                foreach (KeyValuePair<string, List<string>> keyValuePair in descriptions)
                {
                    if (count > 0)
                        tooltipText += "\n";

                    tooltipText += keyValuePair.Key;
                    foreach(string value in keyValuePair.Value)
                    {
                        tooltipText += "\n    " + value;
                    }
                    count++;
                    
                }

                this.DrawHoverTextBox(Game1.smallFont, tooltipText, isItFromToolbar , item.Stack);
            }
        }

        private Item GetHoveredItemFromMenu(IClickableMenu menu)
        {
            // game menu
            if (menu is GameMenu gameMenu)
            {
                IClickableMenu page = this.Helper.Reflection.GetField<List<IClickableMenu>>(gameMenu, "pages").GetValue()[gameMenu.currentTab];
                if (page is InventoryPage)
                    return this.Helper.Reflection.GetField<Item>(page, "hoveredItem").GetValue();
            }
            // from inventory UI (so things like shops and so on)
            else if (menu is MenuWithInventory inventoryMenu)
            {
                return inventoryMenu.hoveredItem;
            }

            return null;
        }

        private Item GetHoveredItemFromToolbar()
        {
            foreach (IClickableMenu menu in Game1.onScreenMenus)
            {
                if (menu is Toolbar toolbar)
                {
                    return this.Helper.Reflection.GetField<Item>(menu, "hoverItem").GetValue();
                }
            }

            return null;
        }

        private void DrawHoverTextBox(SpriteFont font, string description, bool isItFromToolbar, int itemStack)
        {
            Vector2 stringLength = font.MeasureString(description);
            int width = (int)stringLength.X + Game1.tileSize / 2 + 40;
            int height = (int)stringLength.Y + Game1.tileSize / 3 + 5;

            int x = (int)(Mouse.GetState().X / Game1.options.zoomLevel) - Game1.tileSize / 2 - width;
            int y = (int)(Mouse.GetState().Y / Game1.options.zoomLevel) + Game1.tileSize / 2;

            //So that the tooltips don't overlap
            if ((isCJBSellItemPriceLoaded || isUiInfoSuiteLoaded) && !isItFromToolbar)
            {
                if (itemStack > 1)
                    y += 95;
                else
                    y += 55;
            }   

            if (x < 0)
                x = 0;

            if (y + height > Game1.graphics.GraphicsDevice.Viewport.Height)
                y = Game1.graphics.GraphicsDevice.Viewport.Height - height;

            IClickableMenu.drawTextureBox(Game1.spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y, width, height, Color.White);
            Utility.drawTextWithShadow(Game1.spriteBatch, description, font, new Vector2(x + Game1.tileSize / 4, y + Game1.tileSize / 4), Game1.textColor);
        }

        private Dictionary<int, int[][]> getBundles()
        {
            switch (LocalizedContentManager.CurrentLanguageCode)
            {
                case LocalizedContentManager.LanguageCode.ja:
                    language = ".ja-JP";
                    break;
                case LocalizedContentManager.LanguageCode.ru:
                    language = ".ru-RU";
                    break;
                case LocalizedContentManager.LanguageCode.pt:
                    language = ".pt-BR";
                    break;
                case LocalizedContentManager.LanguageCode.es:
                    language = ".es-ES";
                    break;
                case LocalizedContentManager.LanguageCode.de:
                    language = ".de-DE";
                    break;
                case LocalizedContentManager.LanguageCode.zh:
                    language = ".zh-CN";
                    break;
            }

            Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\Bundles" + language);

            Dictionary<int, int[][]> bundles = new Dictionary<int, int[][]>();
            bundleNamesAndSubNames = new Dictionary<int, string[]>();

            foreach (KeyValuePair<string, string> keyValuePair in dictionary)
            {
                //format of the values are itemID itemAmount itemQuality

                //if bundleIndex is between 23 and 26, then they're vault bundles so don't add to dictionary

                string[] split = keyValuePair.Key.Split('/');
                string bundleName = split[0];

                string bundleSubName;

                if (language == "")
                    bundleSubName = keyValuePair.Value.Split('/')[0];
                else
                    bundleSubName = keyValuePair.Value.Split('/')[4];

                int bundleIndex = Convert.ToInt32(split[1]);
                if (!(bundleIndex >= 23 && bundleIndex <= 26))
                {
                    //creating an array for the bundle names
                    string[] bundleNames = new string[] {bundleName,bundleSubName} ;

                    //creating an array of items[i][j] , i is the item index, j=0 itemId, j=1 itemAmount, j=2 itemQuality, j=3 order of the item for it's own bundle
                    string[] allItems = keyValuePair.Value.Split('/')[2].Split(' ');
                    int allItemsLength = allItems.Length / 3;
                    
                    int[][] items = new int[allItemsLength][];

                    int j = 0;
                    int i = 0;
                    while(j< allItemsLength)
                    {
                        items[j] = new int[4];
                        items[j][0] = Convert.ToInt32(allItems[0 + i]);
                        items[j][1] = Convert.ToInt32(allItems[1 + i]);
                        items[j][2] = Convert.ToInt32(allItems[2 + i]);
                        items[j][3] = i/3;

                        itemsInBundles.Add(items[j][0]);
                        i = i + 3;
                        j++;
                    }

                    bundles.Add(bundleIndex, items);
                    bundleNamesAndSubNames.Add(bundleIndex, bundleNames);
                }
            }

            return bundles;
        }
    }
}