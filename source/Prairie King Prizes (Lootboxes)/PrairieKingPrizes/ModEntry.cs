using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using PrairieKingPrizes.Framework;
using xTile.Layers;
using xTile.Tiles;

namespace PrairieKingPrizes
{
    public class ModEntry : Mod, IAssetEditor
    {
        private int _coinsCollected;
        private int _totalTokens;
        private object _lastMinigame;
        private ModConfig _config;
        private readonly int[] _common = { 495, 496, 497, 498, 390, 388, 441, 463, 464, 465, 535, 709 };
        private readonly int[] _uncommon = { 88, 301, 302, 431, 453, 472, 473, 475, 477, 478, 479, 480, 481, 482, 483, 484, 485, 487, 488, 489, 490, 491, 492, 493, 494, 466, 340, 724, 725, 726, 536, 537, 335 };
        private readonly int[] _rare = { 72, 337, 417, 305, 308, 336, 787, 710, 413, 430, 433, 437, 444, 446, 439, 680, 749, 797, 486, 681, 690, 688, 689 };
        private readonly int[] _coveted = { 499, 347, 417, 163, 166, 107, 341, 645, 789, 520, 682, 585, 586, 587, 373 };
        private readonly int[] _legendary = { 74 };
        IDictionary<int, string> _objectData;
        

        public override void Entry(IModHelper helper)
        {
            _config = Helper.ReadConfig<ModConfig>();
            //Events
            helper.Events.GameLoop.UpdateTicked += GameEvents_UpdateTick;
            helper.Events.GameLoop.SaveLoaded += AfterSaveLoaded;
            helper.Events.Input.ButtonPressed += CheckAction;
            helper.Events.GameLoop.Saved += UpdateSavedData;

            //Custom Commands
            helper.ConsoleCommands.Add("gettokens", "Retrieves the value of your current amount of tokens.", GetCoins);
            helper.ConsoleCommands.Add("orange", "Debug stuff, outputs a list of all items in the loot pool. Needs 3 special words in order to activate.", OrangeMonkeyEagle);
            
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data/ObjectInformation");
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/ObjectInformation"))
                _objectData = asset.AsDictionary<int, string>().Data;
        }

        private void GetCoins(string command, string[] args)
        {
            Monitor.Log($"You currently have {_totalTokens} coins.");
        }

        private void OrangeMonkeyEagle(string command, string[] args)
        {
            if (args.Length == 2)
            {
                if (args[0].ToLower() == "monkey" && args[1].ToLower() == "eagle")
                {


                    //double allItems = _common.Length + _uncommon.Length + _rare.Length + _coveted.Length + _legendary.Length;
                    double totalPercentageBasic = 0;
                    double totalPercentagePremium = 0;


                    Monitor.Log("--- Common Items - 40% Basic/20% Premium ---");
                    foreach (int index in _common)
                    {
                        if (_objectData.TryGetValue(index, out string entry))
                        {
                            string[] fields = entry.Split('/');
                            string name = fields[0];
                            totalPercentageBasic += Math.Round((0.4 / _common.Length) * 100, 2);
                            totalPercentagePremium += Math.Round((0.2 / _common.Length) * 100, 2);
                            Monitor.Log($"ID {index} is a {name}. Drops {Math.Round((0.4 / _common.Length) * 100, 2)}% of the time in a basic box and a {Math.Round((0.2 / _common.Length) * 100, 2)}% in the premium box.");
                        }
                    }
                    Monitor.Log("--- Uncommon Items - 30% Basic/25% Premium ---");
                    foreach (int index in _uncommon)
                    {
                        if (_objectData.TryGetValue(index, out string entry))
                        {
                            string[] fields = entry.Split('/');
                            string name = fields[0];
                            totalPercentageBasic += Math.Round((0.3 / _uncommon.Length) * 100, 2);
                            totalPercentagePremium += Math.Round((0.25 / _uncommon.Length) * 100, 2);
                            Monitor.Log($"ID {index} is a {name}. Drops {Math.Round((0.3 / _uncommon.Length) * 100, 2)}% of the time in a basic box and a {Math.Round((0.25 / _uncommon.Length) * 100, 2)}% in the premium box.");
                        }
                    }
                    Monitor.Log("--- Rare Items - 20% Basic/30% Premium ---");
                    foreach (int index in _rare)
                    {
                        if (_objectData.TryGetValue(index, out string entry))
                        {
                            string[] fields = entry.Split('/');
                            string name = fields[0];
                            totalPercentageBasic += Math.Round((0.2 / _rare.Length) * 100, 2);
                            totalPercentagePremium += Math.Round((0.3 / _rare.Length) * 100, 2);
                            Monitor.Log($"ID {index} is a {name}. Drops {Math.Round((0.2 / _rare.Length) * 100, 2)}% of the time in a basic box and a {Math.Round((0.3 / _rare.Length) * 100, 2)}% in the premium box.");
                        }
                    }
                    Monitor.Log("--- Coveted Items - 9.9% Basic/24% Premium ---");
                    foreach (int index in _coveted)
                    {
                        if (_objectData.TryGetValue(index, out string entry))
                        {
                            string[] fields = entry.Split('/');
                            string name = fields[0];
                            totalPercentageBasic += Math.Round((0.099 / _coveted.Length) * 100, 2);
                            totalPercentagePremium += Math.Round((0.24 / _coveted.Length) * 100, 2);
                            Monitor.Log($"ID {index} is a {name}. Drops {Math.Round((0.099 / _coveted.Length) * 100, 2)}% of the time in a basic box and a {Math.Round((0.24 / _coveted.Length) * 100),2}% in the premium box.");
                        }
                    }
                    Monitor.Log("--- Legendary Items - 0.1% Basic/1% Premium ---");
                    foreach (int index in _legendary)
                    {
                        if (_objectData.TryGetValue(index, out string entry))
                        {
                            string[] fields = entry.Split('/');
                            string name = fields[0];
                            totalPercentageBasic += Math.Round((0.001 / _legendary.Length) * 100, 2);
                            totalPercentagePremium += Math.Round((0.01 / _legendary.Length) * 100, 2);
                            Monitor.Log($"ID {index} is a {name}. Drops {Math.Round((0.001 / _legendary.Length) * 100, 2)}% of the time in a basic box and a {Math.Round((0.01 / _legendary.Length) * 100, 2)}% in the premium box.");
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

        private void AfterSaveLoaded(object sender, SaveLoadedEventArgs args)
        {
            var savedData = Helper.Data.ReadJsonFile<SavedData>($"data/{Constants.SaveFolderName}.json") ?? new SavedData();
            _totalTokens = savedData.TotalTokens;

            string tilesheetPath = Helper.Content.GetActualAssetKey("assets/z_extraSaloonTilesheet2.png");

            GameLocation location = Game1.getLocationFromName("Saloon");
            TileSheet tilesheet = new TileSheet("z_extraSaloonTilesheet2", location.map, tilesheetPath, new xTile.Dimensions.Size(48, 16), new xTile.Dimensions.Size(16, 16));

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

        private void UpdateSavedData(object sender, SavedEventArgs args)
        {
            var savedData = Helper.Data.ReadJsonFile<SavedData>($"data/{Constants.SaveFolderName}.json") ?? new SavedData();
            savedData.TotalTokens = _totalTokens;
            Helper.Data.WriteJsonFile($"data/{Constants.SaveFolderName}.json", savedData);
        }

        private void CheckAction(object sender, ButtonPressedEventArgs e)
        {
            if (Context.IsPlayerFree && e.Button.IsActionButton())
            {
                Vector2 grabTile = new Vector2(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y) / Game1.tileSize;
                if (!Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 1, Game1.player))
                {
                    grabTile = Game1.player.GetGrabTile();
                }
                Tile tile = Game1.currentLocation.map.GetLayer("Buildings").PickTile(new xTile.Dimensions.Location((int)grabTile.X * Game1.tileSize, (int)grabTile.Y * Game1.tileSize), Game1.viewport.Size);
                xTile.ObjectModel.PropertyValue propertyValue = null;
                tile?.Properties.TryGetValue("Action", out propertyValue);
                if (propertyValue != null)
                {
                    if (propertyValue == "TokenMachine")
                    {
                        Response basic = new Response("Basic", $"Basic Tier ({_config.BasicBoxCost} Tokens)");
                        Response premium = new Response("Premium", $"Premium Tier ({_config.PremiumBoxCost} Tokens)");
                        Response cancel = new Response("Cancel", "Cancel");
                        Response[] answers = { basic, premium, cancel, };

                        Game1.player.currentLocation.createQuestionDialogue($"Would you like to spend your tokens to receive a random item? You currently have {_totalTokens} tokens.", answers, AfterQuestion);
                    }
                }
            }
        }

        private void AfterQuestion(Farmer who, string whichAnswer)
        {
            //this.Monitor.Log("Successfully called the AfterQuestion method");
            if (whichAnswer == "Basic")
            {
                GivePlayerBasicItem();
            }
            else if (whichAnswer == "Premium")
            {
                GivePlayerPremiumItem();
            }
        }

        int _coinStorage;
        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (_config.RequireGameCompletion && !Game1.player.mailReceived.Contains("Beat_PK")) return;
            if (Game1.currentMinigame != null && "AbigailGame".Equals(Game1.currentMinigame.GetType().Name))
            {
                Type minigameType = Game1.currentMinigame.GetType();
                _coinsCollected = Convert.ToInt32(minigameType.GetField("coins").GetValue(Game1.currentMinigame));
                if (_config.AlternateCoinMethod) {
                    if (_coinsCollected > _coinStorage) _coinStorage = _coinsCollected;
                } else {
                    _coinStorage = _coinsCollected;
                }
                _lastMinigame = Game1.currentMinigame.GetType().Name;
            }

            if (Game1.currentMinigame == null && "AbigailGame".Equals(_lastMinigame))
            {
                _totalTokens += _coinStorage;
                _coinsCollected = 0;
                _coinStorage = 0;
            }
        }

        private void GivePlayerBasicItem()
        {
            Random random = new Random();
            double diceRoll = random.NextDouble();

            if (_totalTokens >= _config.BasicBoxCost)
            {
                _totalTokens -= _config.BasicBoxCost;
                Game1.addHUDMessage(new HUDMessage($"Your current Token balance is now {_totalTokens}.", 2));
                Game1.playSound("purchase");
                
                if (diceRoll <= 0.001)
                {
                    //give legendary item
                    //this.Monitor.Log($"Attempting to give player an item with the ID of 74.");
                    Game1.player.addItemByMenuIfNecessary(new StardewValley.Object(74, 1));
                }
                if (diceRoll > 0.001 && diceRoll <= 0.1)
                {
                    //give coveted item
                    Random rnd = new Random();
                    int r = rnd.Next(_coveted.Length);
                    //this.Monitor.Log($"Attempting to give player an item with the ID of {coveted[r]}.");
                    Game1.player.addItemByMenuIfNecessary(new StardewValley.Object(_coveted[r], 1));
                }
                if (diceRoll > 0.1 && diceRoll <= 0.3)
                {
                    //give rare item
                    Random rnd = new Random();
                    int r = rnd.Next(_rare.Length);
                    //this.Monitor.Log($"Attempting to give player an item with the ID of {rare[r]}.");
                    Game1.player.addItemByMenuIfNecessary(new StardewValley.Object(_rare[r], 2));
                }
                if (diceRoll > 0.3 && diceRoll <= 0.6)
                {
                    //give uncommon item
                    Random rnd = new Random();
                    int r = rnd.Next(_uncommon.Length);
                    //this.Monitor.Log($"Attempting to give player an item with the ID of {uncommon[r]}.");
                    Game1.player.addItemByMenuIfNecessary(new StardewValley.Object(_uncommon[r], 3));
                }
                if (diceRoll > 0.6 && diceRoll <= 1)
                {
                    //give common item
                    Random rnd = new Random();
                    int r = rnd.Next(_common.Length);
                    //this.Monitor.Log($"Attempting to give player an item with the ID of {common[r]}.");
                    if(_common[r] == 390 || _common[r] == 388)
                    {
                        Game1.player.addItemByMenuIfNecessary(new StardewValley.Object(_common[r], 30));
                    }
                    Game1.player.addItemByMenuIfNecessary(_common[r] == 709
                        ? new StardewValley.Object(_common[r], 15)
                        : new StardewValley.Object(_common[r], 5));
                }
            }
            else
            {
                Game1.addHUDMessage(new HUDMessage("You do not have enough Tokens.", 3));
                Game1.addHUDMessage(new HUDMessage($"Your current Token balance is {_totalTokens}.", 2));
                Game1.playSound("cancel");
            }
        }

        private void GivePlayerPremiumItem()
        {
            Random random = new Random();
            double diceRoll = random.NextDouble();

            if (_totalTokens >= _config.PremiumBoxCost)
            {
                _totalTokens -= _config.PremiumBoxCost;
                Game1.addHUDMessage(new HUDMessage($"Your current Token balance is now {_totalTokens}.", 2));
                Game1.playSound("purchase");

                if (diceRoll <= 0.01)
                {
                    //give legendary premium item
                    //this.Monitor.Log($"Attempting to give player an item with the ID of 74.");
                    Game1.player.addItemByMenuIfNecessary(new StardewValley.Object(74, 2));
                }
                if (diceRoll > 0.01 && diceRoll <= 0.20)
                {
                    //give coveted premium item
                    Random rnd = new Random();
                    int r = rnd.Next(_coveted.Length);
                    //this.Monitor.Log($"Attempting to give player an item with the ID of {coveted[r]}.");
                    Game1.player.addItemByMenuIfNecessary(new StardewValley.Object(_coveted[r], 5));
                }
                if (diceRoll > 0.20 && diceRoll <= 0.45)
                {
                    //give rare premium item
                    Random rnd = new Random();
                    int r = rnd.Next(_rare.Length);
                    //this.Monitor.Log($"Attempting to give player an item with the ID of {rare[r]}.");
                    Game1.player.addItemByMenuIfNecessary(new StardewValley.Object(_rare[r], 10));
                }
                if (diceRoll > 0.45 && diceRoll <= 0.8)
                {
                    //give uncommon premium item
                    Random rnd = new Random();
                    int r = rnd.Next(_uncommon.Length);
                    //this.Monitor.Log($"Attempting to give player an item with the ID of {uncommon[r]}.");
                    Game1.player.addItemByMenuIfNecessary(new StardewValley.Object(_uncommon[r], 15));
                }
                if (diceRoll > 0.8 && diceRoll <= 1)
                {
                    //give common premium item
                    Random rnd = new Random();
                    int r = rnd.Next(_common.Length);
                    //this.Monitor.Log($"Attempting to give player an item with the ID of {common[r]}.");
                    if (_common[r] == 390 || _common[r] == 388)
                    {
                        Game1.player.addItemByMenuIfNecessary(new StardewValley.Object(_common[r], 65));
                    }
                    Game1.player.addItemByMenuIfNecessary(_common[r] == 709
                        ? new StardewValley.Object(_common[r], 40)
                        : new StardewValley.Object(_common[r], 25));
                }
            }
            else
            {
                Game1.addHUDMessage(new HUDMessage($"Your current Token balance is {_totalTokens}.", 2));
                Game1.addHUDMessage(new HUDMessage($"You do not have enough Tokens.", 3));
                Game1.playSound("cancel");
            }
        }

        //Secrets - Basic Item
        //    Common - 40% | Quantity: 5
        //    Uncommon - 30% | Quantity: 3
        //    Rare - 20% | Quantity: 2
        //    Coveted - 9.9% | Quantity: 1
        //    Legendary - 0.1% | Quantity: 1
        //Secrets - Premium Item
        //    Common - 20% | Quantity: 25
        //    Uncommon - 25% | Quantity: 15
        //    Rare - 30% | Quantity: 10
        //    Coveted - 24% | Quantity: 5
        //    Legendary - 1% | Quantity: 2
    }
}