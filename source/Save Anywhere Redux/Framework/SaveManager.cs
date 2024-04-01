/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/RealSweetPanda/SaveAnywhereRedux
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using SaveAnywhere.Framework.Model;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace SaveAnywhere.Framework
{
    public class SaveManager
    {
        public static event EventHandler SaveComplete;
        private readonly IModHelper _helper;
        private readonly Action _onLoaded;
        public readonly Dictionary<string, Action> AfterCustomSavingCompleted;
        public readonly Dictionary<string, Action> AfterSaveLoaded;
        public readonly Dictionary<string, Action> BeforeCustomSavingBegins;
        private static SaveGameMenu _currentSaveMenu;
        private bool _waitingToSave;
        private static bool _middaysaving;


        public SaveManager(IModHelper helper, Action onLoaded)
        {
            _helper = helper;
            _onLoaded = onLoaded;
            BeforeCustomSavingBegins = new Dictionary<string, Action>();
            AfterCustomSavingCompleted = new Dictionary<string, Action>();
            AfterSaveLoaded = new Dictionary<string, Action>();
        }

        private string RelativeDataPath => Path.Combine("data", Constants.SaveFolderName + ".json");

        public event EventHandler BeforeSave;

        public event EventHandler AfterSave;

        public event EventHandler AfterLoad;


        public void Update()
        {
            if (!_waitingToSave || Game1.activeClickableMenu != null)
                return;
            Game1.newDaySync = new NewDaySynchronizer();
            Game1.newDaySync.start();
            Game1.weatherForTomorrow = Game1.getWeatherModificationsForDate(Game1.Date, Game1.weatherForTomorrow);
            _currentSaveMenu = new SaveGameMenu();
            SaveComplete += CurrentSaveMenu_SaveComplete;
            Game1.activeClickableMenu = _currentSaveMenu;
            _waitingToSave = false;
        }

        private void CurrentSaveMenu_SaveComplete(object sender, EventArgs e)
        {
            SaveComplete -= CurrentSaveMenu_SaveComplete;
            _currentSaveMenu = null;
            SaveAnywhere.Instance.RestoreMonsters();
            AfterSave?.Invoke(this, EventArgs.Empty);
            foreach (var keyValuePair in AfterCustomSavingCompleted)
                keyValuePair.Value?.Invoke();
        }


        public void ClearData()
        {
            if (File.Exists(Path.Combine(_helper.DirectoryPath, RelativeDataPath)))
                File.Delete(Path.Combine(_helper.DirectoryPath, RelativeDataPath));
            _helper.Data.WriteSaveData<PlayerData>("midday-save", null);
            RemoveLegacyDataForThisPlayer();
        }

        public bool saveDataExists()
        {
            return File.Exists(Path.Combine(_helper.DirectoryPath, RelativeDataPath)) ||
                   _helper.Data.ReadSaveData<PlayerData>("midday-save") != null;
        }

        public void BeginSaveData()
        {
            BeforeSave?.Invoke(this, EventArgs.Empty);
            foreach (var customSavingBegin in BeforeCustomSavingBegins)
                customSavingBegin.Value?.Invoke();


            var farm = Game1.getFarm();
            // hnnnnnnnnnnnnnnnnnnn
            // var drink = Game1.buffsDisplay.GetSortedBuffs();
            // BuffData data = null;
            //
            // if (drink != null)
            //     drinkdata = new BuffData(drink.displaySource, drink.source, drink.millisecondsDuration,
            //         drink.buffAttributes);

            // var food = Game1.buffsDisplay.food;
            BuffData drinkdata = null;

            BuffData fooddata = null;
            // if (food != null)
            //     fooddata = new BuffData(food.displaySource, food.source, food.millisecondsDuration,
            //         food.buffAttributes);

            _helper.Data.WriteSaveData("midday-save", new PlayerData
            {
                Time = Game1.timeOfDay,
                OtherBuffs = null,
                DrinkBuff = drinkdata,
                FoodBuff = fooddata,
                Position = GetPosition().ToArray(),
                IsCharacterSwimming = Game1.player.swimming.Value
            });
            var tempShippingBin = new Chest(true, new Vector2(-100, -100));
            foreach (var item in farm.getShippingBin(Game1.player))
            {
                item.modData["farmerSelling"] = Game1.player.UniqueMultiplayerID.ToString();
                if (item == Game1.getFarm().lastItemShipped)
                    item.modData["last"] = "true";
                else
                    item.modData["last"] = "false";
                tempShippingBin.addItem(item);
            }

            SaveAnywhere.Instance.cleanMonsters();
            Game1.getFarm().setObject(new Vector2(-100, -100), tempShippingBin);
            Game1.getFarm().getObjectAtTile(-100, -100).modData["Pathoschild.ChestsAnywhere/IsIgnored"] = "true";
            Game1.activeClickableMenu = new ShippingMenu(Game1.getFarm().getShippingBin(Game1.player));
            _waitingToSave = true;
            _middaysaving = true;
            RemoveLegacyDataForThisPlayer();
        }

        public void LoadData()
        {
            var data = _helper.Data.ReadSaveData<PlayerData>("midday-save") ??
                       _helper.Data.ReadJsonFile<PlayerData>(RelativeDataPath);
            if (data == null)
                ClearData();


            foreach (var item in (Game1.getFarm().getObjectAtTile(-100, -100) as Chest).Items)
            {
                if (item.modData["farmerSelling"] == Game1.player.UniqueMultiplayerID.ToString())
                {
                    if (item.modData["last"] == "true")
                        Game1.getFarm().lastItemShipped = item;
                    Game1.getFarm().getShippingBin(Game1.player).Add(item);
                }
            }

            Game1.getFarm().removeObject(new Vector2(-100, -100), false);
            SetPositions(data.Position, data.Time);
            // if (data.OtherBuffs != null)
            // foreach (var buff in data.OtherBuffs)
            // {
            //     var atts = buff.Attributes;
            //     Game1.buffsDisplay.addOtherBuff(new Buff(atts[0],
            //         atts[1],
            //         atts[2],
            //         atts[3],
            //         atts[4],
            //         atts[5],
            //         atts[6],
            //         atts[7],
            //         atts[8],
            //         atts[9],
            //         atts[10],
            //         atts[11],
            //         buff.MillisecondsDuration * 10 / 7000,
            //         buff.Source,
            //         buff.DisplaySource));
            // }

            var datadrink = data.DrinkBuff;
            var datafood = data.FoodBuff;

            // if (datadrink != null)
            // {
            //     var atts = datadrink.Attributes;
            //     Game1.buffsDisplay.tryToAddDrinkBuff(new Buff(atts[0],
            //         atts[1],
            //         atts[2],
            //         atts[3],
            //         atts[4],
            //         atts[5],
            //         atts[6],
            //         atts[7],
            //         atts[8],
            //         atts[9],
            //         atts[10],
            //         atts[11],
            //         datadrink.MillisecondsDuration * 10 / 7000,
            //         datadrink.Source,
            //         datadrink.DisplaySource));
            // }
            //
            // if (datafood != null)
            // {
            //     var atts = datafood.Attributes;
            //     Game1.buffsDisplay.tryToAddFoodBuff(new Buff(atts[0],
            //         atts[1],
            //         atts[2],
            //         atts[3],
            //         atts[4],
            //         atts[5],
            //         atts[6],
            //         atts[7],
            //         atts[8],
            //         atts[9],
            //         atts[10],
            //         atts[11],
            //         datafood.MillisecondsDuration * 10 / 7000,
            //         datafood.Source,
            //         datafood.DisplaySource), datafood.MillisecondsDuration);
            // }

            ResumeSwimming(data);
            _onLoaded?.Invoke();
            AfterLoad?.Invoke(this, EventArgs.Empty);
            foreach (var keyValuePair in AfterSaveLoaded)
                keyValuePair.Value?.Invoke();
        }

        private void ResumeSwimming(PlayerData data)
        {
            try
            {
                if (!data.IsCharacterSwimming)
                    return;
                Game1.player.changeIntoSwimsuit();
                Game1.player.swimming.Value = true;
            }
            catch
            {
                // ignored
            }
        }

        private IEnumerable<BuffData> GetotherBuffs()
        {
            // new BuffEffects().
            // foreach (var buff in Game1.player.buffs.AppliedBuffs)
            //     yield return new BuffData(buff., buff.source, buff.millisecondsDuration,
            //         buff.);
            return null;
        }

        private IEnumerable<PositionData> GetPosition()
        {
            var player = Game1.player;
            var name1 = player.Name + "FARMERTAGSA";
            var map1 = player.currentLocation.uniqueName.Value;
            if (string.IsNullOrEmpty(map1))
                map1 = player.currentLocation.Name;
            var tile1 = player.TilePoint;
            int facingDirection1 = player.FacingDirection;
            yield return new PositionData(name1, map1, tile1.X, tile1.Y, facingDirection1);

            foreach (var allCharacter in Utility.getAllCharacters())
                yield return new PositionData(allCharacter.Name, allCharacter.currentLocation.Name,
                    allCharacter.TilePoint.X, allCharacter.TilePoint.Y,
                    allCharacter.FacingDirection);
        }

        private void SetPositions(PositionData[] position, int time)
        {
            Game1.player.faceDirection(position[0].FacingDirection);

            Game1.fadeScreenToBlack();
            Game1.warpFarmer(position[0].Map, position[0].X, position[0].Y, false);
            SafelySetTime(time);

            foreach (var allCharacter in Utility.getAllCharacters())
            {
                var pos = position.FirstOrDefault(p => p.Name == allCharacter.Name);
                if (pos == null) continue;
                Game1.warpCharacter(allCharacter, pos.Map, new Point(pos.X, pos.Y));
                allCharacter.faceDirection(pos.FacingDirection);
                if (!allCharacter.IsVillager) continue;
                allCharacter.TryLoadSchedule();
                if (allCharacter.Schedule == null) continue;
                var dest = allCharacter.Schedule
                    .OrderBy(pair => pair.Key).LastOrDefault(data => data.Key < time);

                if (dest.Key == 0) continue;
                var endingLocation = dest.Value.targetLocationName;
                allCharacter.Schedule.Remove(dest.Key);
                    
                var schedule = allCharacter.pathfindToNextScheduleLocation(
                    allCharacter.ScheduleKey, pos.Map, pos.X, pos.Y,
                    endingLocation,
                    dest.Value.targetTile.X,
                    dest.Value.targetTile.Y,
                    dest.Value.facingDirection,
                    dest.Value.endOfRouteBehavior,
                    dest.Value.endOfRouteMessage);

                allCharacter.update(Game1.currentGameTime, Utility.getGameLocationOfCharacter(allCharacter));
                allCharacter.Schedule.TryAdd(time, schedule);
                if (allCharacter.doingEndOfRouteAnimation.Value || allCharacter.controller == null)
                    continue;
                allCharacter.controller.pathToEndPoint = schedule.route;
                allCharacter.controller.update(Game1.currentGameTime);
            }

            Utility.fixAllAnimals();
        }


        private void RemoveLegacyDataForThisPlayer()
        {
            var directoryInfo1 = new DirectoryInfo(Path.Combine(_helper.DirectoryPath, "Save_Data"));
            var directoryInfo2 = new DirectoryInfo(Path.Combine(directoryInfo1.FullName, Game1.player.Name));
            if (directoryInfo2.Exists)
                directoryInfo2.Delete(true);
            if (!directoryInfo1.Exists || directoryInfo1.EnumerateDirectories().Any())
                return;
            directoryInfo1.Delete(true);
        }

        private void SafelySetTime(int time)
        {
            // transition to new time
            var intervals = Utility.CalculateMinutesBetweenTimes(Game1.timeOfDay, time) / 10;
            if (intervals > 0)
                for (var i = 0; i < intervals; i++)
                    Game1.performTenMinuteClockUpdate();
            else if (intervals < 0)
                for (var i = 0; i > intervals; i--)
                {
                    Game1.timeOfDay =
                        Utility.ModifyTime(Game1.timeOfDay, -20); // offset 20 mins so game updates to next interval
                    Game1.performTenMinuteClockUpdate();
                }

            // reset ambient light
            // White is the default non-raining color. If it's raining or dark out, UpdateGameClock
            // below will update it automatically.
            Game1.outdoorLight = Color.White;
            Game1.ambientLight = Color.White;

            // run clock update (to correct lighting, etc)
            Game1.gameTimeInterval = 0;
            Game1.UpdateGameClock(Game1.currentGameTime);
        }

        internal void RunAfterSave(object sender, SavedEventArgs e)
        {
            if (_middaysaving)
            {
                SaveComplete?.Invoke("", EventArgs.Empty);
                _middaysaving = false;
            }
        }
    }
}