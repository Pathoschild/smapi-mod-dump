/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/OfficialRenny/PrairieKingPrizes
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using PrairieKingPrizes.Framework;
using PrairieKingPrizes.Framework.Config;
using xTile.Layers;
using xTile.Tiles;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace PrairieKingPrizes
{
    public class ModEntry : Mod
    {
        private int _coinsCollected;
        private int _totalTokens;
        private object _lastMinigame;
        private ModConfig _config;
        private Random _random;
        IDictionary<int, string> _objectData;

        public override void Entry(IModHelper helper)
        {
            _config = Helper.ReadConfig<ModConfig>();
            _random = new Random();

            //Events
            helper.Events.GameLoop.UpdateTicked += GameEvents_UpdateTick;
            helper.Events.GameLoop.SaveLoaded += AfterSaveLoaded;
            helper.Events.Input.ButtonPressed += CheckAction;
            helper.Events.GameLoop.Saved += UpdateSavedData;

            // asset placing
            helper.Events.Content.AssetReady += Asset_Ready;

            //Custom Commands
            helper.ConsoleCommands.Add("gettokens", "Retrieves the value of your current amount of tokens.", GetCoins);
            helper.ConsoleCommands.Add("addtokens", "Adds a specified amount of tokens to your total.", AddCoins);
            helper.ConsoleCommands.Add("settokens", "Sets your total amount of tokens to a specified amount.", SetCoins);
            helper.ConsoleCommands.Add("lootbox", "Lootbox helpers.", LootboxCommands);            
        }

        private void Asset_Ready(object sender, AssetReadyEventArgs e)
        {
            if (e.NameWithoutLocale.BaseName == "Data/ObjectInformation")
            {
                _objectData = Helper.GameContent.Load<Dictionary<int, string>>(e.NameWithoutLocale);
            }
        }

        private void GetCoins(string command, string[] args)
        {
            Monitor.Log($"You currently have {_totalTokens} tokens.", LogLevel.Info);
        }

        private void AddCoins(string command, string[] args)
        {
            if (int.TryParse(args[0], out int amount))
            {
                _totalTokens += amount;
                Monitor.Log($"You now have {_totalTokens} tokens.", LogLevel.Info);
            }
            else
            {
                Monitor.Log("Invalid Arguments", LogLevel.Warn);
            }
        }

        private void SetCoins(string command, string[] args)
        {
            if (int.TryParse(args[0], out int amount))
            {
                _totalTokens = amount;
                Monitor.Log($"You now have {_totalTokens} tokens.", LogLevel.Info);
            }
            else
            {
                Monitor.Log("Invalid Arguments", LogLevel.Warn);
            }
        }

        private void LootboxCommands(string command, string[] args)
        {
            if (!args.Any())
            {
                LootboxHelp();
                return;
            }

            if (args.ElementAtOrDefault(0) == "list")
            {
                if (args.Length > 1)
                {
                    var lootbox = _config.Lootboxes.FirstOrDefault(x => x.Key.ToLower() == args.ElementAtOrDefault(1).ToLower());
                    if (lootbox == null) return;

                    var prizeString = string.Join("\n", lootbox.PrizeTiers.Select(x => $"Name: {x.Name}\nChance: {x.Chance}\nPrizes:\n{string.Join("\n\t", x.Prizes.Select(y => $"- {y.Quantity}x {_objectData[y.ItemId].Split('/').FirstOrDefault()} ({y.ItemId})"))}"));
                    Monitor.Log($"Lootbox: {lootbox.Name}\nCost: {lootbox.Cost}\nPrize Tiers: {prizeString}", LogLevel.Info);
                }

                var lootboxString = string.Join("\n", _config.Lootboxes.Select(x => $"Key: {x.Key} - Name: {x.Name} - Cost: {x.Cost}"));
                Monitor.Log($"Lootboxes: \n{lootboxString}", LogLevel.Info);
                return;
            }

            if (args.ElementAtOrDefault(0) == "simulate")
            {
                var key = args.ElementAtOrDefault(1);
                if (key == null)
                {
                    LootboxHelp(); return;
                }

                var lootbox = _config.Lootboxes.FirstOrDefault(x => x.Key.ToLower() == key.ToLower());
                if (lootbox == null) return;

                int times = 1;

                if (args.ElementAtOrDefault(2) != null)
                {
                    int.TryParse(args.ElementAtOrDefault(2), out times);
                    if (times < 1)
                        times = 1;
                }

                Dictionary<Prize, int> prizes = new();
                Dictionary<PrizeTier, int> chosenPrizeTiers = new();

                var totalTierWeight = lootbox.PrizeTiers.Sum(x => x.Chance);

                for (int i = 0; i < times; i++)
                {
                    var prizeTier = _random.PickPrizeTier(lootbox.PrizeTiers);
                    if (chosenPrizeTiers.ContainsKey(prizeTier))
                        chosenPrizeTiers[prizeTier]++;
                    else
                        chosenPrizeTiers.Add(prizeTier, 1);
                    
                    var prize = _random.PickPrize(prizeTier);
                    if (prizes.ContainsKey(prize))
                        prizes[prize]++;
                    else
                        prizes.Add(prize, 1);
                }

                var prizeString = string.Join("\n\t", prizes.OrderByDescending(x => x.Value).Select(x => $"- {x.Key.Quantity}x {_objectData[x.Key.ItemId].Split('/').FirstOrDefault()} ({x.Key.ItemId}, quality {x.Key.Quality ?? 0}, won {x.Value} times.)"));
                Monitor.Log("You won:\n\t" + prizeString, LogLevel.Info);
                var prizeTierString = string.Join("\n\t", chosenPrizeTiers.Select(x => $"- {x.Key.Name}: {x.Key.Chance / totalTierWeight * 100:0.#####}% - {x.Value} times, {x.Value / (double)times * 100:0.#####}% of attempts."));
                Monitor.Log(prizeTierString, LogLevel.Info);

                return;
            }

            LootboxHelp();
            return;
        }

        private void LootboxHelp()
        {
            Monitor.Log(
                        "Lootbox Commands:\n" +
                        "list - Lists all lootboxes and their prizes.\n" +
                        "simulate <key> <times> - Simulates opening a lootbox. <key> is the key of the lootbox you want to open. <times> is the amount of times you want to open the lootbox. Default is 1."
                    , LogLevel.Info);
            return;
        }

        private void AfterSaveLoaded(object sender, SaveLoadedEventArgs args)
        {
            var savedData = Helper.Data.ReadJsonFile<SavedData>($"data/{Constants.SaveFolderName}.json") ?? new SavedData();
            _totalTokens = savedData.TotalTokens;

            var tilesheetPath = Helper.ModContent.GetInternalAssetName("assets/z_extraSaloonTilesheet2.png");

            GameLocation location = Game1.getLocationFromName(_config.MachineLocation.LocationName);
            if (location != null)
            {
                TileSheet tilesheet = new TileSheet("z_extraSaloonTilesheet2", location.map, tilesheetPath.Name, new xTile.Dimensions.Size(48, 16), new xTile.Dimensions.Size(16, 16));

                location.map.AddTileSheet(tilesheet);
                location.map.LoadTileSheets(Game1.mapDisplayDevice);

                Layer layerBack = location.map.GetLayer("Back");
                Layer layerFront = location.map.GetLayer("Front");
                Layer layerBuildings = location.map.GetLayer("Buildings");

                location.removeTile(_config.MachineLocation.X, _config.MachineLocation.Y + 1, "Back");
                TileSheet customTileSheet = location.map.GetTileSheet("z_extraSaloonTilesheet2");
                layerFront.Tiles[_config.MachineLocation.X, _config.MachineLocation.Y - 1] = new StaticTile(layerFront, customTileSheet, BlendMode.Alpha, 0);
                layerBuildings.Tiles[_config.MachineLocation.X, _config.MachineLocation.Y] = new StaticTile(layerBuildings, customTileSheet, BlendMode.Alpha, 1);
                layerBack.Tiles[_config.MachineLocation.X, _config.MachineLocation.Y + 1] = new StaticTile(layerBack, customTileSheet, BlendMode.Alpha, 2);

                location.setTileProperty(_config.MachineLocation.X, _config.MachineLocation.Y, "Buildings", "Action", "TokenMachine");
            }
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
                var grabTile = e.Cursor.GrabTile;
                Tile tile = Game1.currentLocation.map.GetLayer("Buildings").PickTile(new xTile.Dimensions.Location((int)grabTile.X * Game1.tileSize, (int)grabTile.Y * Game1.tileSize), Game1.viewport.Size);
                xTile.ObjectModel.PropertyValue propertyValue = null;
                tile?.Properties.TryGetValue("Action", out propertyValue);
                if (propertyValue != null)
                {
                    if (propertyValue == "TokenMachine")
                    {
                        List<Response> responses = new List<Response>();
                        responses.AddRange(
                            _config.Lootboxes
                            .OrderBy(x => x.Cost)
                            .Select(x => new Response(x.Key, $"{x.Name} ({x.Cost} Tokens)"))
                        );
                        responses.Add(new Response("Cancel", "Cancel"));

                        Game1.player.currentLocation.createQuestionDialogue($"Would you like to spend your tokens to receive a random item? You currently have {_totalTokens} tokens.", responses.ToArray(), AfterQuestion);
                    }
                }
            }
        }

        private void AfterQuestion(Farmer who, string whichAnswer)
        {
            var lootbox = _config.Lootboxes.FirstOrDefault(x => x.Key == whichAnswer);

            if (lootbox == null)
                return;

            if (lootbox.Cost > _totalTokens)
            {
                Game1.addHUDMessage(new HUDMessage("You do not have enough Tokens.", 3));
                //Game1.addHUDMessage(new HUDMessage($"Your current Token balance is {_totalTokens}.", 2));
                Game1.playSound("cancel");
                return;
            }

            GivePlayerItem(lootbox);
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

        private void GivePlayerItem(Lootbox lootbox, bool isFree = false)
        {
            var prizeTier = _random.PickPrizeTier(lootbox.PrizeTiers);
            if (prizeTier == null)
                return;

            var prize = _random.PickPrize(prizeTier);
            if (prize == null)
                return;

            if (!isFree)
            {
                _totalTokens -= lootbox.Cost;
                Game1.addHUDMessage(new HUDMessage($"Your current Token balance is now {_totalTokens}.", 2));
            }

            Game1.playSound("purchase");

            Game1.player.addItemByMenuIfNecessary(new StardewValley.Object(prize.ItemId, prize.Quantity, quality: prize.Quality ?? StardewValley.Object.lowQuality));
        }
    }
}