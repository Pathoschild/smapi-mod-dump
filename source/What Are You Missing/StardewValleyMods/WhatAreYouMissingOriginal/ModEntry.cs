using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Buildings;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace WhatAreYouMissingOriginal
{
    public class ModEntry : Mod
    {
        private ModConfig modConfig;
        private SButton buttonToBringUpInterface;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            //ModEntry is called once before SDV loads.In 3.0 its called even earlier than 2.x - things like Game1.objectInformation 
            //aren't ready yet. If you need to run stuff when a save is ready use the save loaded event
            helper.Events.GameLoop.DayStarted += this.OnDayStarting;
            //helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            modConfig = Helper.ReadConfig<ModConfig>();
            buttonToBringUpInterface = modConfig.button;
            initializeFiles();
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// 
        private void OnDayStarting(object sender, DayStartedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            UpdateFiles();

        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if(e.Button.Equals(buttonToBringUpInterface))
            {
                CheckForMissingItems();
            }
        }

        private void initializeFiles()
        {
            TravellingMerchantStock stock = new TravellingMerchantStock();
            this.Helper.Data.WriteJsonFile("TravellingMerchantStock.json", stock);
        }

        private void UpdateFiles()
        {
            bool showItemsFromCurrentSeasonButInLockedPlaces = modConfig.showItemsFromCurrentSeasonButInLockedPlaces;
            bool showEveryItemFromCurrentSeason = modConfig.showEveryItemFromCurrentSeason;
            bool showAllFish = modConfig.showAllFish;
            bool showCommonCommunityCenterItems = modConfig.showCommonCommunityCenterItems;
            bool showFruitTreesInPreviousSeason = modConfig.showFruitTreesInPreviousSeason;
            bool checkGrowingCrops = modConfig.checkGrowingCrops;
            bool onlyShowWhatCanBeGrownBeforeEndOfSeason = modConfig.onlyShowWhatCanBeGrownBeforeEndOfSeason;

            SpringSpecifics springSpecifics = new SpringSpecifics(showItemsFromCurrentSeasonButInLockedPlaces,
                                                                    showEveryItemFromCurrentSeason,
                                                                    showAllFish,
                                                                    showCommonCommunityCenterItems,
                                                                    showFruitTreesInPreviousSeason,
                                                                    checkGrowingCrops,
                                                                    onlyShowWhatCanBeGrownBeforeEndOfSeason);
            this.Helper.Data.WriteJsonFile(springSpecifics.fileName, springSpecifics);

            SummerSpecifics summerSpecifics = new SummerSpecifics(showItemsFromCurrentSeasonButInLockedPlaces,
                                                                    showEveryItemFromCurrentSeason,
                                                                    showAllFish,
                                                                    showCommonCommunityCenterItems,
                                                                    showFruitTreesInPreviousSeason,
                                                                    checkGrowingCrops,
                                                                    onlyShowWhatCanBeGrownBeforeEndOfSeason);
            this.Helper.Data.WriteJsonFile(summerSpecifics.fileName, summerSpecifics);

            FallSpecifics fallSpecifics = new FallSpecifics(showItemsFromCurrentSeasonButInLockedPlaces,
                                                                showEveryItemFromCurrentSeason,
                                                                showAllFish,
                                                                showCommonCommunityCenterItems,
                                                                showFruitTreesInPreviousSeason,
                                                                checkGrowingCrops,
                                                                onlyShowWhatCanBeGrownBeforeEndOfSeason);
            this.Helper.Data.WriteJsonFile(fallSpecifics.fileName, fallSpecifics);

            WinterSpecifics winterSpecifics = new WinterSpecifics(showItemsFromCurrentSeasonButInLockedPlaces,
                                                                    showEveryItemFromCurrentSeason,
                                                                    showAllFish,
                                                                    showCommonCommunityCenterItems,
                                                                    showFruitTreesInPreviousSeason,
                                                                    checkGrowingCrops,
                                                                    onlyShowWhatCanBeGrownBeforeEndOfSeason);
            this.Helper.Data.WriteJsonFile(winterSpecifics.fileName, winterSpecifics);

            CommonCommunityCenterItems ccItems = new CommonCommunityCenterItems(showItemsFromCurrentSeasonButInLockedPlaces,
                                                                                    showEveryItemFromCurrentSeason,
                                                                                    showAllFish,
                                                                                    showCommonCommunityCenterItems,
                                                                                    showFruitTreesInPreviousSeason,
                                                                                    checkGrowingCrops,
                                                                                    onlyShowWhatCanBeGrownBeforeEndOfSeason);
            this.Helper.Data.WriteJsonFile(ccItems.fileName, ccItems);
        }
        private string WriteFruitTreeWarning(AllItems allItems)
        {
            string result = "";
            if (modConfig.showFruitTreesInPreviousSeason)
            {
                result = " WARNING: The respective saplings for these tree(s) need to be planted now in order to have them produce fruit in their season: ";
                switch (Game1.currentSeason)
                {
                    case "spring":
                        if (allItems.CheckForAnyQuality(Constants.ORANGE).Equals("NoKeys"))
                        {
                            result = String.Concat(result, " Orange Tree ");
                        }
                        if (allItems.CheckForAnyQuality(Constants.PEACH).Equals("NoKeys"))
                        {
                            result = String.Concat(result, " Peach Tree ");
                        }
                        break;
                    case "summer":
                        if (allItems.CheckForAnyQuality(Constants.APPLE).Equals("NoKeys"))
                        {
                            result = String.Concat(result, " Apple Tree ");
                        }
                        if (allItems.CheckForAnyQuality(Constants.POMEGRANATE).Equals("NoKeys"))
                        {
                            result = String.Concat(result, " Pomegranate Tree ");
                        }
                        break;
                    case "fall":
                        break;
                    case "winter":
                        if (allItems.CheckForAnyQuality(Constants.APRICOT).Equals("NoKeys"))
                        {
                            result = String.Concat(result, " Apricot Tree ");
                        }
                        if (allItems.CheckForAnyQuality(Constants.CHEERY).Equals("NoKeys"))
                        {
                            result = String.Concat(result, " Cheery Tree ");
                        }
                        break;
                    default:
                        //should never reach here
                        break;
                }
            }
            return result;
        }
        private void CheckForMissingItems()
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            IEnumerable<Item> ownedItems = OwnedItems();
            AllItems allItems = new AllItems(ownedItems, Helper);
            List<List<SObject>> allMissingItems = allItems.CompareBasedOnSeason();
            List<SObject> specificMissingCommunityCenterItems = allMissingItems[0];
            List<SObject> generalMissingCommunityCenterItems = allMissingItems[1];
            List<SObject> generalMissingItems = allMissingItems[2];

            String listOfMissingItems = "GENERAL MISSING ITEMS ";

            bool firstPass = true;
            foreach(Item item in generalMissingItems)
            {
                if (firstPass)
                {
                    firstPass = false;
                }
                else
                {
                    listOfMissingItems = String.Concat(listOfMissingItems, ", ");
                }
                //When I get around to updating how the info is shown, I should 
                //probably also say like under what conditions you can obtain the item
                listOfMissingItems = String.Concat(listOfMissingItems, item.DisplayName); 
            }
            firstPass = true;
            listOfMissingItems = String.Concat(listOfMissingItems, " SPECIFIC COMMUNITY CENTER ITEMS ");
            foreach (SObject item in specificMissingCommunityCenterItems)
            {
                if (firstPass)
                {
                    firstPass = false;
                }
                else
                {
                    listOfMissingItems = String.Concat(listOfMissingItems, ", ");
                }
                //When I get around to updating how the info is shown, I should 
                //probably also say like under what conditions you can obtain the item
                listOfMissingItems = String.Concat(listOfMissingItems, item.DisplayName, " ", Convert.ToString(item.Stack));
            }
            firstPass = true;
            listOfMissingItems = String.Concat(listOfMissingItems, " GENERAL COMMUNITY CENTER ITEMS ");
            foreach (SObject item in generalMissingCommunityCenterItems)
            {
                if (firstPass)
                {
                    firstPass = false;
                }
                else
                {
                    listOfMissingItems = String.Concat(listOfMissingItems, ", ");
                }
                //When I get around to updating how the info is shown, I should 
                //probably also say like under what conditions you can obtain the item
                listOfMissingItems = String.Concat(listOfMissingItems, item.DisplayName, " ", Convert.ToString(item.Stack));
            }
            FishTabItem testing = new FishTabItem(Constants.ALBACORE);
            Game1.spriteBatch.Begin();
            testing.draw(Game1.spriteBatch, (int)Utility.getTopLeftPositionForCenteringOnScreen(1280, 720).X, (int)Utility.getTopLeftPositionForCenteringOnScreen(1280, 720).Y);
            Game1.spriteBatch.End();
            listOfMissingItems = String.Concat(listOfMissingItems, WriteFruitTreeWarning(allItems));
            LetterViewerMenu letter = new LetterViewerMenu(listOfMissingItems);
            Game1.activeClickableMenu = letter;
            
            
        }

        private IEnumerable<GameLocation> GetAllLocations()
        {
            return Game1.locations
                .Concat(
                from location in Game1.locations.OfType<BuildableGameLocation>()
                from building in location.buildings
                where building.indoors.Value != null
                select building.indoors.Value
                );
        }

        //This might need some extra work based on what exactly I want to give information on
        private IEnumerable<Item> OwnedItems()
        {
            List<Item> ownedItems = new List<Item>();

            //add the current inventory to the list
            ownedItems.AddRange(Game1.player.items);

            foreach(GameLocation location in GetAllLocations())
            {
                
                foreach(SObject obj in location.Objects.Values)
                {
                    //this should add everything that is a chest
                    //it might not add if something is like done in the furnace/cask/keg/preserve jar
                    //i think it covers the auto grabber

                    if (obj is Chest chest)
                    {
                        if (chest.playerChest.Value)
                        {
                            ownedItems.Add(chest);
                            ownedItems.AddRange(chest.items);
                        }
                    }
                    //auto grabber
                    else if (obj.ParentSheetIndex == 165 && obj.heldObject.Value is Chest grabberChest)
                    {
                        ownedItems.Add(obj);
                        ownedItems.AddRange(grabberChest.items);
                    }

                    //cask
                    else if (obj is Cask)
                    {
                        ownedItems.Add(obj);
                        ownedItems.Add(obj.heldObject.Value); // cask contents can be retrieved anytime
                    }

                    //craftable
                    else if (obj.bigCraftable.Value)
                    {
                        ownedItems.Add(obj);
                        if (obj.MinutesUntilReady == 0)
                        {
                            ownedItems.Add(obj.heldObject.Value);
                        }
                    }

                    //anything else
                    else if (!obj.IsSpawnedObject)
                    {
                        ownedItems.Add(obj);
                        ownedItems.Add(obj.heldObject.Value);
                    }
                }

                if (location is Farm farm)
                {
                    foreach(Building building in farm.buildings)
                    {
                        if(building is JunimoHut hut)
                        {
                            ownedItems.AddRange(hut.output.Value.items);
                        }

                        if(building is Mill mill)
                        {
                            ownedItems.AddRange(mill.output.Value.items);
                        }
                    }
                }

                if(location is DecoratableLocation decoratableLocation)
                {
                    foreach(Furniture furniture in decoratableLocation.furniture)
                    {
                        ownedItems.Add(furniture);
                        ownedItems.Add(furniture.heldObject.Value);
                    }
                }
            }
            return ownedItems.Where(p => p != null);
        }



    }
}
