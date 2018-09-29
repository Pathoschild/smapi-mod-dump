using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Layers;
using xTile.Tiles;

namespace PrairieKingPrizes
{
    public class ModEntry : Mod, IAssetEditor
    {
        private int coinsCollected;
        private int totalTokens;
        private object LastMinigame;
        private int[] common = { 495, 496, 497, 498, 390, 388, 441, 463, 464, 465, 535, 709 };
        private int[] uncommon = { 88, 301, 302, 431, 453, 472, 473, 475, 477, 478, 479, 480, 481, 482, 483, 484, 485, 487, 488, 489, 490, 491, 492, 493, 494, 466, 340, 724, 725, 726, 536, 537, 335 };
        private int[] rare = { 72, 337, 417, 305, 308, 336, 787, 710, 413, 430, 433, 437, 444, 446, 439, 680, 749, 797, 486, 681, 690, 688, 689 };
        private int[] coveted = { 499, 347, 417, 163, 166, 107, 341, 645, 789, 520, 682, 585, 586, 587, 373 };
        private int[] legendary = { 74 };
        IDictionary<int, string> objectData;
        

        public override void Entry(IModHelper helper)
        {
            GameEvents.UpdateTick += GameEvents_UpdateTick;
            SaveEvents.AfterLoad += AfterSaveLoaded;
            helper.ConsoleCommands.Add("gettokens", "Retrieves the value of your current amount of tokens.", this.GetCoins);
            helper.ConsoleCommands.Add("orange", "Debug stuff, outputs a list of all items in the loot pool. Needs 3 special words in order to activate.", this.OrangeMonkeyEagle);
            InputEvents.ButtonPressed += CheckAction;
            SaveEvents.AfterSave += UpdateSavedData;
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Data/ObjectInformation"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/ObjectInformation"))
            {
                objectData = asset.AsDictionary<int, string>().Data;
            }
        }

        private void GetCoins(string command, string[] args)
        {
            this.Monitor.Log($"You currently have {totalTokens} coins.");
        }

        private void OrangeMonkeyEagle(string command, string[] args)
        {
            if (args.Length == 2)
            {
                if (args[0].ToLower() == "monkey" && args[1].ToLower() == "eagle")
                {


                    double allItems = common.Count() + uncommon.Count() + rare.Count() + coveted.Count() + legendary.Count();
                    double totalPercentageBasic = 0;
                    double totalPercentagePremium = 0;


                    Monitor.Log("--- Common Items - 40% Basic/20% Premium ---");
                    foreach (int index in common)
                    {
                        if (objectData.TryGetValue(index, out string entry))
                        {
                            string[] fields = entry.Split('/');
                            string name = fields[0];
                            totalPercentageBasic += Math.Round((0.4 / common.Count()) * 100, 2);
                            totalPercentagePremium += Math.Round((0.2 / common.Count()) * 100, 2);
                            this.Monitor.Log($"ID {index} is a {name}. Drops {Math.Round((0.4 / common.Count()) * 100, 2)}% of the time in a basic box and a {Math.Round((0.2 / common.Count()) * 100, 2)}% in the premium box.");
                        }
                    }
                    Monitor.Log("--- Uncommon Items - 30% Basic/25% Premium ---");
                    foreach (int index in uncommon)
                    {
                        if (objectData.TryGetValue(index, out string entry))
                        {
                            string[] fields = entry.Split('/');
                            string name = fields[0];
                            totalPercentageBasic += Math.Round((0.3 / uncommon.Count()) * 100, 2);
                            totalPercentagePremium += Math.Round((0.25 / uncommon.Count()) * 100, 2);
                            this.Monitor.Log($"ID {index} is a {name}. Drops {Math.Round((0.3 / uncommon.Count()) * 100, 2)}% of the time in a basic box and a {Math.Round((0.25 / uncommon.Count()) * 100, 2)}% in the premium box.");
                        }
                    }
                    Monitor.Log("--- Rare Items - 20% Basic/30% Premium ---");
                    foreach (int index in rare)
                    {
                        if (objectData.TryGetValue(index, out string entry))
                        {
                            string[] fields = entry.Split('/');
                            string name = fields[0];
                            totalPercentageBasic += Math.Round((0.2 / rare.Count()) * 100, 2);
                            totalPercentagePremium += Math.Round((0.3 / rare.Count()) * 100, 2);
                            this.Monitor.Log($"ID {index} is a {name}. Drops {Math.Round((0.2 / rare.Count()) * 100, 2)}% of the time in a basic box and a {Math.Round((0.3 / rare.Count()) * 100, 2)}% in the premium box.");
                        }
                    }
                    Monitor.Log("--- Coveted Items - 9% Basic/20% Premium ---");
                    foreach (int index in coveted)
                    {
                        if (objectData.TryGetValue(index, out string entry))
                        {
                            string[] fields = entry.Split('/');
                            string name = fields[0];
                            totalPercentageBasic += Math.Round((0.09 / coveted.Count()) * 100, 2);
                            totalPercentagePremium += Math.Round((0.2 / coveted.Count()) * 100, 2);
                            this.Monitor.Log($"ID {index} is a {name}. Drops {Math.Round((0.09 / coveted.Count()) * 100, 2)}% of the time in a basic box and a {Math.Round((0.2 / coveted.Count()) * 100),2}% in the premium box.");
                        }
                    }
                    Monitor.Log("--- Legendary Items - 1% Basic/5% Premium ---");
                    foreach (int index in legendary)
                    {
                        if (objectData.TryGetValue(index, out string entry))
                        {
                            string[] fields = entry.Split('/');
                            string name = fields[0];
                            totalPercentageBasic += Math.Round((0.01 / legendary.Count()) * 100, 2);
                            totalPercentagePremium += Math.Round((0.05 / legendary.Count()) * 100, 2);
                            this.Monitor.Log($"ID {index} is a {name}. Drops {Math.Round((0.01 / legendary.Count()) * 100, 2)}% of the time in a basic box and a {Math.Round((0.05 / legendary.Count()) * 100, 2)}% in the premium box.");
                        }
                    }
                    Monitor.Log($"--- Basic: {Math.Round(totalPercentageBasic, 2)}% | Premium: {Math.Round(totalPercentagePremium, 2)}% | Both should be 100% (Or close to it)");
                }
            }
            else
            {
                Monitor.Log("Invalid Arguments");
            }
        }

        private void AfterSaveLoaded(object sender, EventArgs args)
        {
            var savedData = this.Helper.ReadJsonFile<SavedData>($"data/{Constants.SaveFolderName}.json") ?? new SavedData();
            totalTokens = savedData.TotalTokens;

            string tilesheetPath = this.Helper.Content.GetActualAssetKey("assets/z_extraSaloonTilesheet2.png", ContentSource.ModFolder);

            GameLocation location = Game1.getLocationFromName("Saloon");
            TileSheet tilesheet = new TileSheet(
               id: "z_extraSaloonTilesheet2",
               map: location.map,
               imageSource: tilesheetPath,
               sheetSize: new xTile.Dimensions.Size(48, 16),
               tileSize: new xTile.Dimensions.Size(16, 16)
            );
            location.map.AddTileSheet(tilesheet);
            location.map.LoadTileSheets(Game1.mapDisplayDevice);

            Layer layerBack = location.map.GetLayer("Back");
            Layer layerFront = location.map.GetLayer("Front");
            Layer layerBuildings = location.map.GetLayer("Buildings");

            location.removeTile(34, 18, "Back");
            TileSheet customTileSheet = location.map.GetTileSheet("z_extraSaloonTilesheet2");
            layerFront.Tiles[34, 16] = new StaticTile(layerFront, customTileSheet, BlendMode.Alpha, 0);
            layerBuildings.Tiles[34, 17] = new StaticTile(layerBuildings, customTileSheet, BlendMode.Alpha, 1);
            layerBack.Tiles[34, 18] = new StaticTile(layerBack, customTileSheet, BlendMode.Alpha, 2);

            location.setTileProperty(34, 17, "Buildings", "Action", "TokenMachine");
        }

        private void UpdateSavedData(object sender, EventArgs args)
        {
            var savedData = this.Helper.ReadJsonFile<SavedData>($"data/{Constants.SaveFolderName}.json") ?? new SavedData();
            savedData.TotalTokens = totalTokens;
            this.Helper.WriteJsonFile($"data/{Constants.SaveFolderName}.json", savedData);
        }

        private void CheckAction(object sender, EventArgsInput e)
        {
            if (Context.IsPlayerFree && e.IsActionButton)
            {
                Vector2 grabTile = new Vector2((float)(Game1.getOldMouseX() + Game1.viewport.X), (float)(Game1.getOldMouseY() + Game1.viewport.Y)) / (float)Game1.tileSize;
                if (!Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 1, Game1.player))
                {
                    grabTile = Game1.player.GetGrabTile();
                }
                Tile tile = Game1.currentLocation.map.GetLayer("Buildings").PickTile(new xTile.Dimensions.Location((int)grabTile.X * Game1.tileSize, (int)grabTile.Y * Game1.tileSize), Game1.viewport.Size);
                xTile.ObjectModel.PropertyValue propertyValue = null;
                if (tile != null)
                {
                    tile.Properties.TryGetValue("Action", out propertyValue);
                }
                if (propertyValue != null)
                {
                    if (propertyValue == "TokenMachine")
                    {
                        Response basic = new Response("Basic", "Basic Tier (10 Tokens)");
                        Response premium = new Response("Premium", "Premium Tier (50 Tokens)");
                        Response cancel = new Response("Cancel", "Cancel");
                        Response checkTokens = new Response("Tokens", "Check Tokens");
                        Response[] answers = { basic, premium, checkTokens, cancel, };

                        Game1.player.currentLocation.createQuestionDialogue("Would you like to spend your tokens to receive a random item?", answers, AfterQuestion, null);
                    }
                }
            }
        }

        private void AfterQuestion(Farmer who, string whichAnswer)
        {
            //this.Monitor.Log("Successfully called the AfterQuestion method");
            if (whichAnswer == "Basic")
            {
                givePlayerBasicItem();
            }
            else if (whichAnswer == "Premium")
            {
                givePlayerPremiumItem();
            }
            else if (whichAnswer == "Tokens")
            {
                checkPlayersTokens();
            }
            else
            {
                return;
            }
        }

        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (Game1.currentMinigame != null && "AbigailGame".Equals(Game1.currentMinigame.GetType().Name))
            {
                Type minigameType = Game1.currentMinigame.GetType();
                coinsCollected = Convert.ToInt32(minigameType.GetField("coins").GetValue(Game1.currentMinigame));
                LastMinigame = Game1.currentMinigame.GetType().Name;
            }

            if (Game1.currentMinigame == null && "AbigailGame".Equals(LastMinigame))
            {
                totalTokens += coinsCollected;
                coinsCollected = 0;
            }
        }

        private void checkPlayersTokens()
        {
            Game1.playSound("purchase");
            Game1.drawDialogueBox($"You currently have {totalTokens} tokens.");
            return;
        }

        private void givePlayerBasicItem()
        {
            Random random = new Random();
            double diceRoll = random.NextDouble();

            if (totalTokens >= 10)
            {
                totalTokens -= 10;
                Game1.addHUDMessage(new HUDMessage($"Your current Token balance is now {totalTokens}.", 2));
                Game1.playSound("purchase");

                if (diceRoll <= 0.01)
                {
                    //give legendary item
                    //this.Monitor.Log($"Attempting to give player an item with the ID of 74.");
                    Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(74, 1, false, -1, 0));
                }
                if (diceRoll > 0.01 && diceRoll <= 0.1)
                {
                    //give coveted item
                    Random rnd = new Random();
                    int r = rnd.Next(coveted.Length);
                    //this.Monitor.Log($"Attempting to give player an item with the ID of {coveted[r]}.");
                    Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(coveted[r], 1, false, -1, 0));
                }
                if (diceRoll > 0.1 && diceRoll <= 0.3)
                {
                    //give rare item
                    Random rnd = new Random();
                    int r = rnd.Next(rare.Length);
                    //this.Monitor.Log($"Attempting to give player an item with the ID of {rare[r]}.");
                    Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(rare[r], 2, false, -1, 0));
                }
                if (diceRoll > 0.3 && diceRoll <= 0.6)
                {
                    //give uncommon item
                    Random rnd = new Random();
                    int r = rnd.Next(uncommon.Length);
                    //this.Monitor.Log($"Attempting to give player an item with the ID of {uncommon[r]}.");
                    Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(uncommon[r], 3, false, -1, 0));
                }
                if (diceRoll > 0.6 && diceRoll <= 1)
                {
                    //give common item
                    Random rnd = new Random();
                    int r = rnd.Next(common.Length);
                    //this.Monitor.Log($"Attempting to give player an item with the ID of {common[r]}.");
                    if(common[r] == 390 || common[r] == 388)
                    {
                        Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(common[r], 30, false, -1, 0));
                    }
                    if (common[r] == 709)
                    {
                        Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(common[r], 15, false, -1, 0));
                    }
                    else
                    {
                        Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(common[r], 5, false, -1, 0));
                    }
                }
            }
            else
            {
                Game1.addHUDMessage(new HUDMessage($"You do not have enough Tokens.", 3));
                Game1.addHUDMessage(new HUDMessage($"Your current Token balance is {totalTokens}.", 2));
                Game1.playSound("cancel");
                return;
            }
        }

        private void givePlayerPremiumItem()
        {
            Random random = new Random();
            double diceRoll = random.NextDouble();

            if (totalTokens >= 50)
            {
                totalTokens -= 50;
                Game1.addHUDMessage(new HUDMessage($"Your current Token balance is now {totalTokens}.", 2));
                Game1.playSound("purchase");

                if (diceRoll <= 0.05)
                {
                    //give legendary premium item
                    //this.Monitor.Log($"Attempting to give player an item with the ID of 74.");
                    Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(74, 2, false, -1, 0));
                }
                if (diceRoll > 0.05 && diceRoll <= 0.25)
                {
                    //give coveted premium item
                    Random rnd = new Random();
                    int r = rnd.Next(coveted.Length);
                    //this.Monitor.Log($"Attempting to give player an item with the ID of {coveted[r]}.");
                    Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(coveted[r], 5, false, -1, 0));
                }
                if (diceRoll > 0.25 && diceRoll <= 0.45)
                {
                    //give rare premium item
                    Random rnd = new Random();
                    int r = rnd.Next(rare.Length);
                    //this.Monitor.Log($"Attempting to give player an item with the ID of {rare[r]}.");
                    Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(rare[r], 10, false, -1, 0));
                }
                if (diceRoll > 0.45 && diceRoll <= 0.8)
                {
                    //give uncommon premium item
                    Random rnd = new Random();
                    int r = rnd.Next(uncommon.Length);
                    //this.Monitor.Log($"Attempting to give player an item with the ID of {uncommon[r]}.");
                    Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(uncommon[r], 15, false, -1, 0));
                }
                if (diceRoll > 0.8 && diceRoll <= 1)
                {
                    //give common premium item
                    Random rnd = new Random();
                    int r = rnd.Next(common.Length);
                    //this.Monitor.Log($"Attempting to give player an item with the ID of {common[r]}.");
                    if (common[r] == 390 || common[r] == 388)
                    {
                        Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(common[r], 65, false, -1, 0));
                    }
                    if (common[r] == 709)
                    {
                        Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(common[r], 40, false, -1, 0));
                    }
                    else
                    {
                        Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(common[r], 25, false, -1, 0));
                    }
                }
            }
            else
            {
                Game1.addHUDMessage(new HUDMessage($"Your current Token balance is {totalTokens}.", 2));
                Game1.addHUDMessage(new HUDMessage($"You do not have enough Tokens.", 3));
                Game1.playSound("cancel");
                return;
            }
        }

        //Secrets - Basic Item
        //    Common - 40% | Quantity: 5
        //    Uncommon - 30% | Quantity: 3
        //    Rare - 20% | Quantity: 2
        //    Coveted - 9% | Quantity: 1
        //    Legendary - 1% | Quantity: 1
        //Secrets - Premium Item
        //    Common - 20% | Quantity: 25
        //    Uncommon - 25% | Quantity: 15
        //    Rare - 30% | Quantity: 10
        //    Coveted - 20% | Quantity: 5
        //    Legendary - 5% | Quantity: 2

        internal class SavedData
        {
            public int TotalTokens { get; set; }
        }
    }
}