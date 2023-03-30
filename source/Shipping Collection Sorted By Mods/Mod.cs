/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/holy-the-sea/SortShippingCollection
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using GenericModConfigMenu;

namespace SortShippingCollection
{
    public class ModEntry : Mod
    {
        private ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.Display.MenuChanged += Display_OnMenuChanged;
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // add GMCM
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Mod-sort Shipping Collections Tab",
                getValue: () => Config.ModSortShippingCollection,
                setValue: value => Config.ModSortShippingCollection = value
            );
        }

        private void Display_OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is not GameMenu)
                return;

            if (Config.ModSortShippingCollection)
            {
                var menu = (GameMenu)e.NewMenu;
                if (menu.pages[5] is CollectionsPage)
                {
                    var collectionsTab = (CollectionsPage)menu.pages[5];
                    collectionsTab.collections[0] = ModSortShippingCollection(menu);
                }
            }
        }

        private static List<List<ClickableTextureComponent>> ModSortShippingCollection(GameMenu menu)
        {
            // get ObjectInformation data
            IDictionary<int, string> objectInformation = new Dictionary<int, string>(Game1.objectInformation);

            var menuPages = menu.pages;
            var collectionsTab = (CollectionsPage)menuPages[5];

            // retrieve all objects that make up shipping collections tab
            var objectDataModOrdered = new List<string>();
            objectDataModOrdered.AddRange(objectInformation.Values);
            var shippingTab = collectionsTab.collections[0];
            int shippingPageSize = 0;
            List<ClickableTextureComponent> allShippingItems = new List<ClickableTextureComponent>();
            foreach (var page in shippingTab)
            {
                allShippingItems.AddRange(page);
                if (page.Count > shippingPageSize)
                {
                    shippingPageSize = page.Count;
                }
            }

            // grab the sort from the ObjectInformation data, which is actually not numerically sorted and retains the original add-order
            allShippingItems.Sort(delegate (ClickableTextureComponent a, ClickableTextureComponent b)
            {
                int num = -1;
                int value = -1;
                if (a != null && int.TryParse(a.name.Split(" ")[0], out var objectID1))
                {
                    if (objectInformation.ContainsKey(objectID1))
                    {
                        string objectData1 = objectInformation[objectID1];
                        num = objectDataModOrdered.IndexOf(objectData1);
                    }
                    else // handle DGA-added items
                    {
                        num = objectInformation.Count + 1;
                    }
                }
                if (b != null && int.TryParse(b.name.Split(" ")[0], out var objectID2))
                {
                    if (objectInformation.ContainsKey(objectID2))
                    {
                        string objectData2 = objectInformation[objectID2];
                        value = objectDataModOrdered.IndexOf(objectData2);
                    }
                    else // handle DGA-added items
                    {
                        value = objectInformation.Count + 1;
                    }
                }
                return num.CompareTo(value);
            });

            // go through the sorted list and dupe them for the correct objects
            List<List<ClickableTextureComponent>> sortedShippingCollection = new List<List<ClickableTextureComponent>>();
            List<ClickableTextureComponent> newShippingPage = new List<ClickableTextureComponent>();
            int shippingPageIndex = 0;
            for (var i = 0; i < allShippingItems.Count; i++)
            {
                int indexOnPage = i % shippingPageSize;

                // steal fields to make new clickable texture component in desired spot
                var targetIcon = shippingTab[shippingPageIndex][indexOnPage];
                newShippingPage.Add(new ClickableTextureComponent(allShippingItems[i].name, targetIcon.bounds, allShippingItems[i].label, allShippingItems[i].hoverText, allShippingItems[i].texture, allShippingItems[i].sourceRect, allShippingItems[i].scale, allShippingItems[i].drawShadow)
                {
                    myID = targetIcon.myID,
                    rightNeighborID = targetIcon.rightNeighborID,
                    leftNeighborID = targetIcon.leftNeighborID,
                    downNeighborID = targetIcon.downNeighborID,
                    upNeighborID = targetIcon.upNeighborID,
                    fullyImmutable = true
                });

                // after you've filled up a page, add it to the collections and start a new one
                if (newShippingPage.Count == shippingPageSize)
                {
                    sortedShippingCollection.Add(newShippingPage);
                    shippingPageIndex++;
                    newShippingPage = new List<ClickableTextureComponent>();
                }
            }
            sortedShippingCollection.Add(newShippingPage);
            return sortedShippingCollection;
        }
    }
    public class ModConfig
    {
        public bool ModSortShippingCollection { get; set; } = true;
    }

}
