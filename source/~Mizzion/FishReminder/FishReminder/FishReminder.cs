/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using FishReminder.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.GameData.Locations;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;

namespace FishReminder;

    public class FishReminder : Mod//To do Change the fish dictionary to int, string. That wasy I can cross reference based on int.
    {
        //private string fishNeeded;

        private FishConfig _config;

        private Dictionary<string, FishInfo> _fish = new();

        private List<string> caughtFish = new();
        
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<FishConfig>();
            //Events
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            //helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.GameLoop.DayEnding += OnDayEnding;
            helper.Events.Content.AssetRequested += AssetRequested;
        }


        private void AssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Mail"))
            {
                e.Edit(asset =>
                {
                    var i18N = Helper.Translation;
                    var data = asset.AsDictionary<string, string>().Data;
                    data["fishReminderDaily"] = i18N.Get("fishReminderDaily",
                        new { player = Game1.player.Name, fish = GetNeededFish() });

                    data["fishReminderWeekly"] = i18N.Get("fishReminderWeekly",
                        new { player = Game1.player.Name, fish = GetNeededFish() });

                    data["fishReminderMonthly"] = i18N.Get("fishReminderMonthly",
                        new { player = Game1.player.Name, fish = GetNeededFish() });
                });
            }
        }
        //Event methods
        /// <summary>Event that fires when a button is pressed</summary>
        /// <param name="sender">The sender</param>
        /// /// <param name="e">Information related to which button was pressed</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            //Process button pressed
            if (e.IsDown(SButton.F5))
            {
                _config = Helper.ReadConfig<FishConfig>();
                Monitor.Log("Config file was reloaded.");
            }
            else if (e.IsDown(SButton.Home))
            {
                //GrabFishInfo();
                GrabFishInfo();
                GetNeededFish();
            }
            else if (e.IsDown(SButton.NumPad3))
            {
                var locations = DataLoader.Locations(Game1.content);
                var fishData = DataLoader.Fish(Game1.content);

                //Print out Location Data

                foreach (var location in locations)
                {
                    var fishInArea = location.Value.Fish;
                    var fishString = "";
                    var itemName = "";



                    foreach (var fin in fishInArea)
                    {

                        if (!string.IsNullOrEmpty(fin.FishAreaId))
                        {
                            if (!string.IsNullOrEmpty(fin.ItemId))
                            {
                                var item = ItemRegistry.GetMetadata(fin.ItemId);
                                if (!item.Exists())
                                    continue;

                                var it = item.CreateItemOrErrorItem();
                                itemName = it.Name;
                            }


                            //fishString += $"{fin.ItemId}({itemName}), ";

                            //fishString += $"{fin.ItemId}, ";
                        }

                        if (!string.IsNullOrEmpty(itemName))
                        {
                            fishString += $"Location: {location.Key} Chance: {fin.Chance} Season: {fin.Season} FishAreaId: {fin.FishAreaId} MinFishingLevel: {fin.MinFishingLevel} MinDistanceFromShore: {fin.MinDistanceFromShore} MaxDistanceFromShore: {fin.MaxDistanceFromShore} ApplyDailyLuck: {fin.ApplyDailyLuck} CuriosityLureBuff: {fin.CuriosityLureBuff} CatchLimit: {fin.CatchLimit} IsBossFish: {fin.IsBossFish} SetFlagOnCatch: {fin.SetFlagOnCatch} RequiredMagicBelt: {fin.RequireMagicBait} Precedence: {fin.Precedence} IgnoreFishDataRequirements: {fin.IgnoreFishDataRequirements} CanBeInherited: {fin.CanBeInherited} ItemId: {fin.ItemId}({itemName}) \r";
                        }
                        else
                            fishString += $"Location: {location.Key} Chance: {fin.Chance} Season: {fin.Season} FishAreaId: {fin.FishAreaId} MinFishingLevel: {fin.MinFishingLevel} MinDistanceFromShore: {fin.MinDistanceFromShore} MaxDistanceFromShore: {fin.MaxDistanceFromShore} ApplyDailyLuck: {fin.ApplyDailyLuck} CuriosityLureBuff: {fin.CuriosityLureBuff} CatchLimit: {fin.CatchLimit} IsBossFish: {fin.IsBossFish} SetFlagOnCatch: {fin.SetFlagOnCatch} RequiredMagicBelt: {fin.RequireMagicBait} Precedence: {fin.Precedence} IgnoreFishDataRequirements: {fin.IgnoreFishDataRequirements} CanBeInherited: {fin.CanBeInherited} ItemId: {fin.ItemId} \r";
                    }
                    if (!string.IsNullOrEmpty(fishString))
                        Monitor.Log($"[Fish Area Id] Key: {location.Key} Value: {fishString}");

                    /*
                    foreach (var f in location.Value.FishAreas)
                    {
                        Monitor.Log($"[Fish Areas] Key: {f.Key} Value: {f.Value.DisplayName}");
                    }*/
                }


                //Print out Fish Data
                /*
                foreach (var fish in fishData)
                {
                    Monitor.Log($"[Fish Data] Key: {fish.Key} Value: {fish.Value}");
                }*/
            }

        }

        /// <summary>Event that fires before a world is saved</summary>
        /// <param name="sender">The sender</param>
        /// /// <param name="e">Information related to which button was pressed</param>
        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            var today = SDate.Now();
            var tomorrow = today.AddDays(1);

            var fishNed = GetNeededFish();
            if (string.IsNullOrEmpty(fishNed))
                return;

            switch (tomorrow.Day)
            {
                case 1:
                    if (_config.SendReminderMailMonthly)
                    {
                        Helper.GameContent.InvalidateCache("Data/mail");
                        if(!Game1.player.mailbox.Contains("fishReminderMonthly"))
                            Game1.player.mailForTomorrow.Add("fishReminderMonthly");
                    }
                    break;
                case 8:
                case 15:
                case 22:
                    if (_config.SendReminderMailWeekly)
                    {
                        Helper.GameContent.InvalidateCache("Data/mail");
                        if (!Game1.player.mailbox.Contains("fishReminderWeekly"))
                            Game1.player.mailForTomorrow.Add("fishReminderWeekly");
                    }
                    break;
                default:
                    if (_config.SendReminderMailDaily)
                    {
                        Helper.GameContent.InvalidateCache("Data/mail");
                        if (!Game1.player.mailbox.Contains("fishReminderDaily"))
                            Game1.player.mailForTomorrow.Add("fishReminderDaily");
                    }

                    break;
            }
        }

        //Private Methods

        

        private string GetNeededFish(bool doSeason = true, bool doWeather = false)
        {
            var locations = DataLoader.Locations(Game1.content);
            GrabFishInfo();

            var neededFish = "";

            foreach (var location in locations)
            {
                var fishAvailable = location.Value.Fish;
                var fishNotCaught = "";

                foreach (var fish in fishAvailable)
                {
                    if (!string.IsNullOrEmpty(fish.ItemId))
                    {
                        var fishToBeCaught = ItemRegistry.GetMetadata(fish.ItemId);

                        if (!fishToBeCaught.Exists())
                            continue;

                        if (!Game1.player.fishCaught.ContainsKey(fish.ItemId) && _fish.ContainsKey(fish.ItemId))
                        {

                            fishNotCaught += $"{fishToBeCaught.CreateItemOrErrorItem().Name} ";
                        }

                    }
                }

                if (!string.IsNullOrEmpty(fishNotCaught))
                {
                neededFish = $"{location.Key}: {fishNotCaught}";
                Monitor.Log($"{neededFish}");
                }
                

            }


            return neededFish;

        }

        private void GrabFishInfo()
        {
            _fish.Clear();
            var fishData = DataLoader.Fish(Game1.content);
            var outPut = new Dictionary<string, string>();

            foreach (var fish in fishData)
            {
                var fisher = fish.Value.Split('/');
                if (fisher.Length == 14)
                {
                   // Monitor.Log($"Id: {fish.Key}, Fish Name: {fisher[0]} Spawns In: {fisher[6]}");
                    _fish.Add($"(O){fish.Key}", new FishInfo()
                    {
                        FishId = fish.Key,
                        FishName = fisher[0],
                        Difficulty = fisher[1],
                        DartingRandomness = fisher[2],
                        MinSize = fisher[3],
                        MaxSize = fisher[4],
                        Times = fisher[5],
                        Season = fisher[6],
                        Weather = fisher[7],
                        Locations = fisher[8],
                        MaxDepth = fisher[9],
                        SpawnMultiplier = fisher[10],
                        DepthMultiplier = fisher[11],
                        FishingLevelNeeded = fisher[12],
                        IncludeInFirstCatchTutorial = fisher[13]



                    });
                }
            }
            //caughtFish.Clear();

        //Populate caughtFish
        /*
        foreach (var f in Game1.player.fishCaught.Keys)
        {
            if (caughtFish.Contains(f))
                continue;
            
            caughtFish.Add(f);
            Monitor.Log(f);
        }*/
        }
    }