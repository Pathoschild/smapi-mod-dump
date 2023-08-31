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
using System.Threading;
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
        private readonly IModHelper _helper;
        private readonly Action _onLoaded;
        public readonly Dictionary<string, Action> AfterCustomSavingCompleted;
        public readonly Dictionary<string, Action> AfterSaveLoaded;
        public readonly Dictionary<string, Action> BeforeCustomSavingBegins;
        private SaveGameMenu _currentSaveMenu;
        private bool _waitingToSave;

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
            _currentSaveMenu = new SaveGameMenu();
            Game1.activeClickableMenu = _currentSaveMenu;
            _waitingToSave = false;
        }

        public void SaveComplete(object sender, EventArgs e)
        {
            _currentSaveMenu = null;
            SaveAnywhere.RestoreMonsters();
            Game1.getFarm().removeObject(new Vector2(-100, -100), false);
            Game1.activeClickableMenu = new FTMCompMenu();
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


            SaveAnywhere.Instance.cleanMonsters();
            var farm = Game1.getFarm();
            var drink = Game1.buffsDisplay.drink;
            BuffData drinkdata = null;

            if (drink != null)
                drinkdata = new BuffData(drink.displaySource, drink.source, drink.millisecondsDuration,
                    drink.buffAttributes);

            var food = Game1.buffsDisplay.food;
            BuffData fooddata = null;
            if (food != null)
                fooddata = new BuffData(food.displaySource, food.source, food.millisecondsDuration,
                    food.buffAttributes);

            _helper.Data.WriteSaveData("midday-save", new PlayerData
            {
                Time = Game1.timeOfDay,
                OtherBuffs = GetotherBuffs().ToArray(),
                DrinkBuff = drinkdata,
                FoodBuff = fooddata,
                Position = GetPosition().ToArray(),
                IsCharacterSwimming = Game1.player.swimming.Value
            });
            Game1.newDaySync =
                new NewDaySynchronizer(); //SaveComplete was never called because newDaySync was never assigned, causing AfterSave event to never fire
            Game1.newDaySync.start();
            Game1.flushLocationLookup();
            Game1.weatherForTomorrow = Game1.getWeatherModificationsForDate(Game1.Date, Game1.weatherForTomorrow);
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

            Game1.getFarm().setObject(new Vector2(-100, -100), tempShippingBin);
            Game1.activeClickableMenu = new NewShippingMenuV2(farm.getShippingBin(Game1.player));
            _waitingToSave = true;
            RemoveLegacyDataForThisPlayer();
        }

        public void LoadData()
        {
            var data = _helper.Data.ReadSaveData<PlayerData>("midday-save") ??
                       _helper.Data.ReadJsonFile<PlayerData>(RelativeDataPath);
            if (data == null)
                ClearData();


            foreach (var item in (Game1.getFarm().getObjectAtTile(-100, -100) as Chest).items)
                if (item.modData["farmerSelling"] == Game1.player.UniqueMultiplayerID.ToString())
                {
                    if (item.modData["last"] == "true")
                        Game1.getFarm().lastItemShipped = item;
                    Game1.getFarm().getShippingBin(Game1.player).Add(item);
                }

            Game1.getFarm().removeObject(new Vector2(-100, -100), false);
            SetPositions(data.Position, data.Time);
            if (data.OtherBuffs != null)
                foreach (var buff in data.OtherBuffs)
                {
                    var atts = buff.Attributes;
                    Game1.buffsDisplay.addOtherBuff(new Buff(atts[0],
                        atts[1],
                        atts[2],
                        atts[3],
                        atts[4],
                        atts[5],
                        atts[6],
                        atts[7],
                        atts[8],
                        atts[9],
                        atts[10],
                        atts[11],
                        buff.MillisecondsDuration * 10 / 7000,
                        buff.Source,
                        buff.DisplaySource));
                }

            var datadrink = data.DrinkBuff;
            var datafood = data.FoodBuff;

            if (datadrink != null)
            {
                var atts = datadrink.Attributes;
                Game1.buffsDisplay.tryToAddDrinkBuff(new Buff(atts[0],
                    atts[1],
                    atts[2],
                    atts[3],
                    atts[4],
                    atts[5],
                    atts[6],
                    atts[7],
                    atts[8],
                    atts[9],
                    atts[10],
                    atts[11],
                    datadrink.MillisecondsDuration * 10 / 7000,
                    datadrink.Source,
                    datadrink.DisplaySource));
            }

            if (datafood != null)
            {
                var atts = datafood.Attributes;
                Game1.buffsDisplay.tryToAddFoodBuff(new Buff(atts[0],
                    atts[1],
                    atts[2],
                    atts[3],
                    atts[4],
                    atts[5],
                    atts[6],
                    atts[7],
                    atts[8],
                    atts[9],
                    atts[10],
                    atts[11],
                    datafood.MillisecondsDuration * 10 / 7000,
                    datafood.Source,
                    datafood.DisplaySource), datafood.MillisecondsDuration);
            }

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
            foreach (var buff in Game1.buffsDisplay.otherBuffs)
                yield return new BuffData(buff.displaySource, buff.source, buff.millisecondsDuration,
                    buff.buffAttributes);
        }

        private IEnumerable<PositionData> GetPosition()
        {
            var player = Game1.player;
            var name1 = player.Name;
            var map1 = player.currentLocation.uniqueName.Value;
            if (string.IsNullOrEmpty(map1))
                map1 = player.currentLocation.Name;
            var tile1 = player.getTileLocationPoint();
            int facingDirection1 = player.facingDirection;
            yield return new PositionData(name1, map1, tile1.X, tile1.Y, facingDirection1);

            foreach (var allCharacter in Utility.getAllCharacters())
                if (allCharacter.isVillager())
                    yield return new PositionData(allCharacter.Name, allCharacter.currentLocation.Name,
                        allCharacter.getTileLocationPoint().X, allCharacter.getTileLocationPoint().Y,
                        allCharacter.facingDirection);
        }

        private void SetPositions(PositionData[] position, int time)
        {
            Game1.player.faceDirection(position[0].FacingDirection);
            foreach (var allCharacter in Utility.getAllCharacters())
                if (allCharacter.isVillager())
                {
                    allCharacter.dayUpdate(Game1.dayOfMonth);
                    var pos = position.FirstOrDefault(p => p.Name == allCharacter.Name);
                    if (pos != null)
                    {
                        Game1.xLocationAfterWarp = pos.X;
                        Game1.yLocationAfterWarp = pos.Y;
                        Game1.facingDirectionAfterWarp = pos.FacingDirection;
                        Game1.warpCharacter(allCharacter, pos.Map, new Point(pos.X, pos.Y));
                        allCharacter.faceDirection(pos.FacingDirection);
                        var newSchedule = allCharacter.getSchedule(Game1.dayOfMonth);
                        if (newSchedule != null)
                        {
                            var dest = allCharacter.getSchedule(Game1.dayOfMonth)
                                .OrderBy(pair => pair.Key).LastOrDefault(data => data.Key < time);

                            if (dest.Key != 0)
                            {
                                newSchedule.Remove(dest.Key);
                                while (newSchedule.ContainsKey(dest.Key))
                                {
                                }

                                var scheduleEntry =
                                    allCharacter.getMasterScheduleEntry(getCurrentEndLocation(allCharacter));
                                if (scheduleEntry.Contains("GOTO"))
                                    scheduleEntry =
                                        allCharacter.getMasterScheduleEntry(scheduleEntry.Replace("GOTO ", ""));
                                var destMap = scheduleEntry.Split("/")
                                    .LastOrDefault(s =>
                                        int.Parse(s.Replace("/", "").Split(" ")[0].Replace(" ", "")) < time)
                                    .Split(" ")[1].Replace(" ", "");
                                newSchedule.TryAdd(time,
                                    allCharacter.pathfindToNextScheduleLocation(pos.Map, pos.X, pos.Y, destMap,
                                        dest.Value.route.Last().X, dest.Value.route.Last().Y,
                                        dest.Value.facingDirection,
                                        dest.Value.endOfRouteBehavior, dest.Value.endOfRouteMessage));
                                allCharacter.Schedule = newSchedule;
                            }
                        }
                    }

                    Game1.player.previousLocationName = Game1.player.currentLocation.Name;
                    Game1.xLocationAfterWarp = position[0].X;
                    Game1.yLocationAfterWarp = position[0].Y;
                    Game1.facingDirectionAfterWarp = position[0].FacingDirection;
                    Game1.fadeScreenToBlack();
                    Game1.warpFarmer(position[0].Map, position[0].X, position[0].Y, false);
                }

            SafelySetTime(time);
            foreach (var allCharacter in Utility.getAllCharacters())
                if (allCharacter.isVillager())
                {
                    allCharacter.update(Game1.currentGameTime, Utility.getGameLocationOfCharacter(allCharacter));
                    if (allCharacter.doingEndOfRouteAnimation.Value)
                        continue;
                    allCharacter.controller = null;
                    allCharacter.checkSchedule(time);
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

        private string getCurrentEndLocation(NPC allCharacter)
        {
            if (allCharacter.isMarried())
            {
                if (allCharacter.hasMasterScheduleEntry("marriage_" + Game1.currentSeason + "_" +
                                                        Game1.dayOfMonth))
                    return "marriage_" + Game1.currentSeason + "_" + Game1.dayOfMonth;

                var str = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
                if ((allCharacter.Name.Equals("Penny") &&
                     (str.Equals("Tue") || str.Equals("Wed") || str.Equals("Fri"))) ||
                    (allCharacter.Name.Equals("Maru") && (str.Equals("Tue") || str.Equals("Thu"))) ||
                    (allCharacter.Name.Equals("Harvey") && (str.Equals("Tue") || str.Equals("Thu"))))
                    return "marriageJob";

                if (!Game1.isRaining &&
                    allCharacter.hasMasterScheduleEntry("marriage_" +
                                                        Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)))
                    return "marriage_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
            }

            if (allCharacter.hasMasterScheduleEntry(Game1.currentSeason + "_" + Game1.dayOfMonth))
                return Game1.currentSeason + "_" + Game1.dayOfMonth;
            var playerFriendshipLevel = Utility.GetAllPlayerFriendshipLevel(allCharacter);
            if (playerFriendshipLevel >= 0)
                playerFriendshipLevel /= 250;
            for (; playerFriendshipLevel > 0; --playerFriendshipLevel)
                if (allCharacter.hasMasterScheduleEntry(Game1.dayOfMonth + "_" +
                                                        playerFriendshipLevel))
                    return Game1.dayOfMonth + "_" +
                           playerFriendshipLevel;

            if (allCharacter.hasMasterScheduleEntry(string.Empty + Game1.dayOfMonth))
                return string.Empty + Game1.dayOfMonth;
            if (allCharacter.Name.Equals("Pam") && Game1.player.mailReceived.Contains("ccVault"))
                return "bus";
            if (Game1.IsRainingHere(allCharacter.currentLocation))
            {
                if (Game1.random.NextDouble() < 0.5 && allCharacter.hasMasterScheduleEntry("rain2"))
                    return "rain2";
                if (allCharacter.hasMasterScheduleEntry("rain"))
                    return "rain";
            }

            var values = new List<string>
            {
                Game1.currentSeason,
                Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)
            };
            playerFriendshipLevel = Utility.GetAllPlayerFriendshipLevel(allCharacter);
            if (playerFriendshipLevel >= 0)
                playerFriendshipLevel /= 250;
            while (playerFriendshipLevel > 0)
            {
                values.Add(string.Empty + playerFriendshipLevel);
                if (allCharacter.hasMasterScheduleEntry(string.Join("_", values)))
                    return string.Join("_", values);
                --playerFriendshipLevel;
                values.RemoveAt(values.Count - 1);
            }

            if (allCharacter.hasMasterScheduleEntry(string.Join("_", values)))
                return string.Join("_", values);
            if (allCharacter.hasMasterScheduleEntry(Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)))
                return Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
            if (allCharacter.hasMasterScheduleEntry(Game1.currentSeason))
                return Game1.currentSeason;
            if (allCharacter.hasMasterScheduleEntry("spring_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)))
                return "spring_" +
                       Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
            values.RemoveAt(values.Count - 1);
            values.Add("spring");
            playerFriendshipLevel = Utility.GetAllPlayerFriendshipLevel(allCharacter);
            if (playerFriendshipLevel >= 0)
                playerFriendshipLevel /= 250;
            while (playerFriendshipLevel > 0)
            {
                values.Add(string.Empty + playerFriendshipLevel);
                if (allCharacter.hasMasterScheduleEntry(string.Join("_", values)))
                    return string.Join("_", values);
                --playerFriendshipLevel;
                values.RemoveAt(values.Count - 1);
            }

            return "spring";
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
    }
}