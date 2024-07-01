/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net.Enums;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Archipelago.Gifting;
using StardewArchipelago.Extensions;
using StardewArchipelago.GameModifications;
using StardewArchipelago.Items.Mail;
using StardewArchipelago.Items.Traps.Shuffle;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace StardewArchipelago.Items.Traps
{
    public class TrapManager
    {
        private const string BURNT = "Burnt";
        private const string DARKNESS = "Darkness";
        private const string FROZEN = "Frozen";
        private const string JINXED = "Jinxed";
        private const string NAUSEATED = "Nauseated";
        private const string SLIMED = "Slimed";
        private const string WEAKNESS = "Weakness";
        private const string TAXES = "Taxes";
        private const string RANDOM_TELEPORT = "Random Teleport";
        private const string CROWS = "The Crows";
        private const string MONSTERS = "Monsters";
        private const string ENTRANCE_RESHUFFLE = "Entrance Reshuffle";
        private const string DEBRIS = "Debris";
        private const string SHUFFLE = "Shuffle";
        private const string WINTER = "Temporary Winter";
        private const string PARIAH = "Pariah";
        private const string DROUGHT = "Drought";
        private const string TIME_FLIES = "Time Flies";
        private const string BABIES = "Babies";
        private const string MEOW = "Meow";
        private const string BARK = "Bark";
        private const string DEPRESSION = "Depression";
        private const string UNGROWTH = "Benjamin Budton";
        private const string INFLATION = "Inflation";
        private const string BOMB = "Bomb";

        private static IMonitor _monitor;
        private readonly IModHelper _helper;
        private readonly Harmony _harmony;
        private static ArchipelagoClient _archipelago;
        private static TrapDifficultyBalancer _difficultyBalancer;
        private readonly TileChooser _tileChooser;
        private readonly MonsterSpawner _monsterSpawner;
        private readonly BabyBirther _babyBirther;
        private readonly DebrisSpawner _debrisSpawner;
        private readonly InventoryShuffler _inventoryShuffler;
        private Dictionary<string, Action> _traps;

        public TrapManager(IMonitor monitor, IModHelper helper, Harmony harmony, ArchipelagoClient archipelago, TileChooser tileChooser, BabyBirther babyBirther, GiftSender giftSender)
        {
            _monitor = monitor;
            _helper = helper;
            _harmony = harmony;
            _archipelago = archipelago;
            _difficultyBalancer = new TrapDifficultyBalancer();
            _tileChooser = tileChooser;
            _monsterSpawner = new MonsterSpawner(_tileChooser);
            _babyBirther = babyBirther;
            _debrisSpawner = new DebrisSpawner(monitor, archipelago, _difficultyBalancer);
            _inventoryShuffler = new InventoryShuffler(monitor, giftSender);
            _traps = new Dictionary<string, Action>();
            RegisterTraps();

            _harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.salePrice)),
                prefix: new HarmonyMethod(typeof(TrapManager), nameof(SalePrice_GetCorrectInflation_Prefix))
            );
            InitializeTemporaryBaby(monitor, helper, harmony);
        }

        private void InitializeTemporaryBaby(IMonitor monitor, IModHelper helper, Harmony harmony)
        {
            TemporaryBaby.Initialize(monitor, helper);
            harmony.Patch(
                original: AccessTools.Method(typeof(Child), nameof(Child.tenMinuteUpdate)),
                prefix: new HarmonyMethod(typeof(TemporaryBaby), nameof(TemporaryBaby.ChildTenMinuteUpdate_MoveBabiesAnywhere_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performTenMinuteUpdate)),
                prefix: new HarmonyMethod(typeof(TemporaryBaby), nameof(TemporaryBaby.GameLocationPerformTenMinuteUpdate_MoveBabiesAnywhere_Prefix))
            );
        }

        public bool IsTrap(string unlockName)
        {
            return _traps.ContainsKey(unlockName);
        }

        public LetterAttachment GenerateTrapLetter(ReceivedItem unlock)
        {
            return new LetterTrapAttachment(unlock, unlock.ItemName);
        }

        public bool TryExecuteTrapImmediately(string trapName)
        {
            if (Game1.player.currentLocation is FarmHouse
                or IslandFarmHouse) // || Game1.eventUp || Game1.fadeToBlack || Game1.currentMinigame != null || Game1.isWarping || Game1.killScreen)
            {
                return false;
            }

            _traps[trapName]();
            return true;
        }

        private void RegisterTraps()
        {
            _traps.Add(BURNT, AddBurntDebuff);
            _traps.Add(DARKNESS, AddDarknessDebuff);
            _traps.Add(FROZEN, AddFrozenDebuff);
            _traps.Add(JINXED, AddJinxedDebuff);
            _traps.Add(NAUSEATED, AddNauseatedDebuff);
            _traps.Add(SLIMED, AddSlimedDebuff);
            _traps.Add(WEAKNESS, AddWeaknessDebuff);
            _traps.Add(TAXES, ChargeTaxes);
            _traps.Add(RANDOM_TELEPORT, TeleportRandomly);
            _traps.Add(CROWS, SendCrows);
            _traps.Add(MONSTERS, SpawnMonsters);
            // _traps.Add(ENTRANCE_RESHUFFLE, );
            _traps.Add(DEBRIS, _debrisSpawner.CreateDebris);
            _traps.Add(SHUFFLE, ShuffleInventory);
            // _traps.Add(WINTER, );
            _traps.Add(PARIAH, SendDislikedGiftToEveryone);
            _traps.Add(DROUGHT, PerformDroughtTrap);
            _traps.Add(TIME_FLIES, SkipTimeForward);
            _traps.Add(BABIES, SpawnTemporaryBabies);
            _traps.Add(MEOW, PlayMeows);
            _traps.Add(BARK, PlayBarks);
            _traps.Add(DEPRESSION, ForceNextMultisleep);
            _traps.Add(UNGROWTH, UngrowCrops);
            _traps.Add(INFLATION, ActivateInflation);
            _traps.Add(BOMB, Explode);

            RegisterTrapsWithTrapSuffix();
            RegisterTrapsWithDifferentSpace();
        }

        private void RegisterTrapsWithDifferentSpace()
        {
            foreach (var trapName in _traps.Keys.ToArray())
            {
                var differentSpacedTrapName = trapName.Replace(" ", "_");
                if (differentSpacedTrapName != trapName)
                {
                    _traps.Add(differentSpacedTrapName, _traps[trapName]);
                }
            }
        }

        private void RegisterTrapsWithTrapSuffix()
        {
            foreach (var trapName in _traps.Keys.ToArray())
            {
                var trapWithSuffix = $"{trapName} Trap";
                _traps.Add(trapWithSuffix, _traps[trapName]);
            }
        }

        private void AddBurntDebuff()
        {
            AddDebuff(Buffs.GoblinsCurse);
        }

        private void AddDarknessDebuff()
        {
            AddDebuff(Buffs.Darkness);
        }

        private void AddFrozenDebuff()
        {
            var duration = _difficultyBalancer.FrozenDebuffDurations[_archipelago.SlotData.TrapItemsDifficulty];
            AddDebuff(Buffs.Frozen, duration);
        }

        public void AddJinxedDebuff()
        {
            AddDebuff(Buffs.EvilEye);
        }

        private void AddNauseatedDebuff()
        {
            AddDebuff(Buffs.Nauseous);
        }

        private void AddSlimedDebuff()
        {
            AddDebuff(Buffs.Slimed);
        }

        private void AddWeaknessDebuff()
        {
            AddDebuff(Buffs.Weakness);
        }

        private void AddDebuff(Buffs whichBuff)
        {
            var duration = _difficultyBalancer.DefaultDebuffDurations[_archipelago.SlotData.TrapItemsDifficulty];
            AddDebuff(whichBuff, duration);
        }

        private void AddDebuff(Buffs whichBuff, BuffDuration duration)
        {
            if (duration == BuffDuration.Zero)
            {
                return;
            }

            var debuff = new Buff(((int)whichBuff).ToString());
            debuff.millisecondsDuration = (int)duration;
            debuff.totalMillisecondsDuration = (int)duration;
            Game1.player.applyBuff(debuff);
        }

        private void ChargeTaxes()
        {
            var difficulty = _archipelago.SlotData.TrapItemsDifficulty;
            var taxRate = _difficultyBalancer.TaxRates[difficulty];
            var player = Game1.player;
            var currentMoney = player.Money;
            var tax = (int)(currentMoney * taxRate);
            Game1.player.addUnearnedMoney(tax * -1);
            if (difficulty == TrapItemsDifficulty.Nightmare)
            {
                RemoveTaxTrapFromBankAsync().FireAndForget();
            }
        }

        public async Task RemoveTaxTrapFromBankAsync()
        {
            var bankingKey = string.Format(BankHandler.BANKING_TEAM_KEY, _archipelago.GetTeam());
            var currentAmountJoules = await _archipelago.ReadBigIntegerFromDataStorageAsync(Scope.Global, bankingKey);
            if (currentAmountJoules == null || currentAmountJoules <= 0)
            {
                return;
            }

            _archipelago.DivideBigIntegerDataStorage(Scope.Global, bankingKey, 2);
        }

        public void TeleportRandomly()
        {
            var difficulty = _archipelago.SlotData.TrapItemsDifficulty;
            var destination = _difficultyBalancer.TeleportDestinations[difficulty];
            var validMaps = new List<GameLocation>();
            switch (destination)
            {
                case TeleportDestination.None:
                    return;
                case TeleportDestination.Nearby:
                case TeleportDestination.SameMap:
                    validMaps.Add(Game1.player.currentLocation);
                    break;
                case TeleportDestination.SameMapOrHome:
                    validMaps.Add(Game1.getFarm());
                    validMaps.Add(Game1.getLocationFromName("FarmHouse"));
                    if (!Game1.player.currentLocation.Name.Contains("Farm"))
                    {
                        validMaps.Add(Game1.player.currentLocation);
                    }

                    break;
                case TeleportDestination.PelicanTown:
                    validMaps.AddRange(Game1.locations.Where(x => x is not IslandLocation));
                    break;
                case TeleportDestination.Anywhere:
                    validMaps.AddRange(Game1.locations);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            GameLocation chosenLocation = null;
            Vector2? chosenTile = null;
            while (chosenLocation == null || chosenTile == null)
            {
                chosenLocation = validMaps[Game1.random.Next(validMaps.Count)];
                if (destination == TeleportDestination.Nearby)
                {
                    chosenTile = _tileChooser.GetRandomTileInbounds(chosenLocation, Game1.player.TilePoint, 20);
                }
                else
                {
                    chosenTile = _tileChooser.GetRandomTileInbounds(chosenLocation);
                }
            }

            TeleportFarmerTo(chosenLocation.Name, chosenTile.Value);
        }

        private void TeleportFarmerTo(string locationName, Vector2 tile)
        {
            var farmer = Game1.player;
            var multiplayerField = _helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer");
            var multiplayer = multiplayerField.GetValue();
            for (var index = 0; index < 12; ++index)
            {
                multiplayer.broadcastSprites(farmer.currentLocation,
                    new TemporaryAnimatedSprite(354, Game1.random.Next(25, 75), 6, 1,
                        new Vector2(Game1.random.Next((int)farmer.position.X - 256, (int)farmer.position.X + 192),
                            Game1.random.Next((int)farmer.position.Y - 256, (int)farmer.position.Y + 192)), false, Game1.random.NextDouble() < 0.5));
            }

            Game1.currentLocation.playSound("wand");
            Game1.displayFarmer = false;
            farmer.temporarilyInvincible = true;
            farmer.temporaryInvincibilityTimer = -2000;
            farmer.Halt();
            farmer.faceDirection(2);
            farmer.CanMove = false;
            farmer.freezePause = 2000;
            Game1.flashAlpha = 1f;
            DelayedAction.fadeAfterDelay(() => AfterTeleport(farmer, locationName, tile), 1000);
            new Rectangle(farmer.GetBoundingBox().X, farmer.GetBoundingBox().Y, 64, 64).Inflate(192, 192);
            var num = 0;
            for (var x1 = farmer.Tile.X + 8; x1 >= farmer.Tile.X - 8; --x1)
            {
                multiplayer.broadcastSprites(farmer.currentLocation,
                    new TemporaryAnimatedSprite(6, new Vector2(x1, farmer.Tile.Y) * 64f, Color.White, animationInterval: 50f)
                    {
                        layerDepth = 1f,
                        delayBeforeAnimationStart = num * 25,
                        motion = new Vector2(-0.25f, 0.0f),
                    });
                ++num;
            }
        }

        private void AfterTeleport(Farmer farmer, string locationName, Vector2 tile)
        {
            var destination = Utility.Vector2ToPoint(tile);
            Game1.warpFarmer(locationName, destination.X, destination.Y, false);
            Game1.changeMusicTrack("none");
            Game1.fadeToBlackAlpha = 0.99f;
            Game1.screenGlow = false;
            farmer.temporarilyInvincible = false;
            farmer.temporaryInvincibilityTimer = 0;
            Game1.displayFarmer = true;
            farmer.CanMove = true;
        }

        private void SendCrows()
        {
            var crowRate = _difficultyBalancer.CrowAttackRate[_archipelago.SlotData.TrapItemsDifficulty];
            var crowTargets = _difficultyBalancer.CrowValidTargets[_archipelago.SlotData.TrapItemsDifficulty];
            if (crowTargets == CrowTargets.None)
            {
                return;
            }

            if (crowTargets == CrowTargets.Farm)
            {
                var farm = Game1.getFarm();
                SendCrowsForLocation(farm, crowRate);
                return;
            }

            if (crowTargets == CrowTargets.Outside)
            {
                foreach (var gameLocation in Game1.locations)
                {
                    if (!gameLocation.IsOutdoors)
                    {
                        continue;
                    }

                    SendCrowsForLocation(gameLocation, crowRate);
                }
                return;
            }

            foreach (var gameLocation in Game1.locations)
            {
                SendCrowsForLocation(gameLocation, crowRate);
            }
        }

        private static void SendCrowsForLocation(GameLocation map, double crowRate)
        {
            var scarecrowPositions = GetScarecrowPositions(map);
            var crops = GetAllCrops(map);
            map.critters ??= new List<Critter>();
            foreach (var (cropPosition, crop) in crops)
            {
                var roll = Game1.random.NextDouble();
                if (roll > crowRate)
                {
                    continue;
                }

                if (IsCropDefended(map, scarecrowPositions, cropPosition))
                {
                    continue;
                }

                crop.destroyCrop(true);
                map.critters.Add(new Crow((int)crop.Tile.X, (int)crop.Tile.Y));
            }
        }

        private static bool IsCropDefended(GameLocation map, List<Vector2> scarecrowPositions, Vector2 cropPosition)
        {
            var vulnerability = GetCropVulnerability(map, scarecrowPositions, cropPosition);
            for (var i = 0; i < vulnerability; i++)
            {
                if (Game1.random.NextDouble() < TrapDifficultyBalancer.SCARECROW_EFFICIENCY)
                {
                    return true;
                }
            }

            return false;
        }

        private static List<Vector2> GetScarecrowPositions(GameLocation farm)
        {
            var scarecrowPositions = new List<Vector2>();
            foreach (var (position, placedObject) in farm.objects.Pairs)
            {
                if (placedObject.bigCraftable.Value && placedObject.IsScarecrow())
                {
                    scarecrowPositions.Add(position);
                }
            }

            return scarecrowPositions;
        }

        private static IEnumerable<KeyValuePair<Vector2, HoeDirt>> GetAllCrops(GameLocation location)
        {
            foreach (var (cropPosition, cropTile) in location.terrainFeatures.Pairs)
            {
                if (cropTile is not HoeDirt dirt || dirt.crop == null || dirt.crop.currentPhase.Value <= 1)
                {
                    continue;
                }

                yield return new KeyValuePair<Vector2, HoeDirt>(cropPosition, dirt);
            }

            foreach (var (cropPosition, gameObject) in location.Objects.Pairs)
            {
                if (gameObject is not IndoorPot gardenPot || gardenPot.hoeDirt.Value.crop == null || gardenPot.hoeDirt.Value.crop.currentPhase.Value <= 1)
                {
                    continue;
                }

                yield return new KeyValuePair<Vector2, HoeDirt>(cropPosition, gardenPot.hoeDirt.Value);
            }
        }

        private static int GetCropVulnerability(GameLocation farm, List<Vector2> scarecrowPositions, Vector2 cropPosition)
        {
            var numberOfDefendingScarecrows = 0;
            foreach (var scarecrowPosition in scarecrowPositions)
            {
                var radiusForScarecrow = farm.objects[scarecrowPosition].GetRadiusForScarecrow();
                if (Vector2.Distance(scarecrowPosition, cropPosition) < radiusForScarecrow)
                {
                    numberOfDefendingScarecrows++;
                }
            }

            return numberOfDefendingScarecrows;
        }

        private void SpawnMonsters()
        {
            var numberMonsters = _difficultyBalancer.NumberOfMonsters[_archipelago.SlotData.TrapItemsDifficulty];
            for (var i = 0; i < numberMonsters; i++)
            {
                _monsterSpawner.SpawnOneMonster(Game1.player.currentLocation);
            }
        }

        public void ShuffleInventory()
        {
            var targets = _difficultyBalancer.ShuffleTargets[_archipelago.SlotData.TrapItemsDifficulty];
            var rate = _difficultyBalancer.ShuffleRate[_archipelago.SlotData.TrapItemsDifficulty];
            var rateToFriends = _difficultyBalancer.ShuffleRateToFriends[_archipelago.SlotData.TrapItemsDifficulty];
            _inventoryShuffler.ShuffleInventories(targets, rate, rateToFriends);
        }

        private void SendDislikedGiftToEveryone()
        {
            var player = Game1.player;
            var friendshipLoss = _difficultyBalancer.PariahFriendshipLoss[_archipelago.SlotData.TrapItemsDifficulty];
            foreach (var name in player.friendshipData.Keys)
            {
                var npc = Game1.getCharacterFromName(name) ?? Game1.getCharacterFromName<Child>(name, false);
                if (npc == null)
                {
                    continue;
                }

                ++Game1.stats.GiftsGiven;
                player.currentLocation.localSound("give_gift");
                ++player.friendshipData[name].GiftsToday;
                ++player.friendshipData[name].GiftsThisWeek;
                player.friendshipData[name].LastGiftDate = new WorldDate(Game1.Date);
                Game1.player.changeFriendship(friendshipLoss, npc);
            }
        }

        private void PerformDroughtTrap()
        {
            var droughtTargets = _difficultyBalancer.DroughtTargets[_archipelago.SlotData.TrapItemsDifficulty];
            var hoeDirts = GetAllHoeDirt(droughtTargets);
            foreach (var hoeDirt in hoeDirts)
            {
                if (hoeDirt.state.Value == 1)
                {
                    hoeDirt.state.Value = 0;
                }
            }

            if (droughtTargets != DroughtTarget.CropsIncludingWateringCan)
            {
                return;
            }

            foreach (var wateringCan in GetAllWateringCans())
            {
                wateringCan.WaterLeft = 0;
            }
        }

        private IEnumerable<HoeDirt> GetAllHoeDirt(DroughtTarget validTargets)
        {
            if (validTargets == DroughtTarget.None)
            {
                yield break;
            }

            foreach (var gameLocation in Game1.locations)
            {
                if (!gameLocation.IsOutdoors && validTargets < DroughtTarget.CropsIncludingInside)
                {
                    continue;
                }

                foreach (var terrainFeature in gameLocation.terrainFeatures.Values)
                {
                    if (terrainFeature is not HoeDirt groundDirt)
                    {
                        continue;
                    }

                    if (validTargets == DroughtTarget.Soil && groundDirt.crop != null)
                    {
                        continue;
                    }

                    yield return groundDirt;
                }

                foreach (var (tile, gameObject) in gameLocation.Objects.Pairs)
                {
                    if (gameObject is not IndoorPot gardenPot)
                    {
                        continue;
                    }

                    yield return gardenPot.hoeDirt.Value;
                }
            }
        }

        private IEnumerable<WateringCan> GetAllWateringCans()
        {
            foreach (var item in Game1.player.Items)
            {
                if (item is not WateringCan wateringCan)
                {
                    continue;
                }

                yield return wateringCan;
            }


            foreach (var gameLocation in Game1.locations)
            {
                foreach (var (tile, gameObject) in gameLocation.Objects.Pairs)
                {
                    if (gameObject is not Chest chest)
                    {
                        continue;
                    }

                    foreach (var chestItem in chest.Items)
                    {
                        if (chestItem is not WateringCan wateringCan)
                        {
                            continue;
                        }

                        yield return wateringCan;
                    }
                }
            }
        }

        private void SkipTimeForward()
        {
            var timeToSkip = (int)_difficultyBalancer.TimeFliesDurations[_archipelago.SlotData.TrapItemsDifficulty];
            if (timeToSkip > 120)
            {
                MultiSleep.DaysToSkip = (timeToSkip / 120) - 1;
                Game1.timeOfDay = 2800;
                Game1.player.startToPassOut();
                return;
            }

            for (var i = 0; i < timeToSkip; i++)
            {
                Game1.performTenMinuteClockUpdate();
            }
        }

        private void SpawnTemporaryBabies()
        {
            var numberBabies = _difficultyBalancer.NumberOfBabies[_archipelago.SlotData.TrapItemsDifficulty];
            for (var i = 0; i < numberBabies; i++)
            {
                _babyBirther.SpawnTemporaryBaby(i);
            }
        }

        private void PlayMeows()
        {
            var numberOfMeows = _difficultyBalancer.MeowBarkNumber[_archipelago.SlotData.TrapItemsDifficulty];
            PlaySoundsAsync(numberOfMeows, "cat").FireAndForget();
        }

        private void PlayBarks()
        {
            var numberOfMeows = _difficultyBalancer.MeowBarkNumber[_archipelago.SlotData.TrapItemsDifficulty];
            PlaySoundsAsync(numberOfMeows, "dog_bark").FireAndForget();
        }

        private async Task PlaySoundsAsync(int numberOfSounds, string sound)
        {
            for (var i = 0; i < numberOfSounds; i++)
            {
                await Task.Run(() => Thread.Sleep(2000));
                Game1.playSound(sound);
            }
        }

        private void ForceNextMultisleep()
        {
            var daysToSkip = _difficultyBalancer.DepressionTrapDays[_archipelago.SlotData.TrapItemsDifficulty];
            MultiSleep.DaysToSkip = daysToSkip;
        }

        private void UngrowCrops()
        {
            var ungrowthDays = _difficultyBalancer.UngrowthDays[_archipelago.SlotData.TrapItemsDifficulty];
            var hoeDirts = GetAllHoeDirt(DroughtTarget.CropsIncludingInside);
            foreach (var hoeDirt in hoeDirts)
            {
                UngrowCrop(hoeDirt.crop, ungrowthDays);
            }

            var treeUngrowthDays = _difficultyBalancer.TreeUngrowthDays[_archipelago.SlotData.TrapItemsDifficulty];
            var fruitTrees = GetAllFruitTrees();
            foreach (var fruitTree in fruitTrees)
            {
                UngrowFruitTree(fruitTree, treeUngrowthDays);
            }
        }

        private void UngrowCrop(Crop crop, int days)
        {
            if (crop == null)
            {
                return;
            }

            if (crop.fullyGrown.Value)
            {
                crop.fullyGrown.Set(false);
            }

            var dayOfCurrentPhase = crop.dayOfCurrentPhase.Value;
            var currentPhase = crop.currentPhase.Value;
            var daysPerPhase = crop.phaseDays.ToList();

            dayOfCurrentPhase -= days;

            while (dayOfCurrentPhase < 0)
            {
                if (currentPhase <= 0 || !daysPerPhase.Any())
                {
                    break;
                }

                if (currentPhase > daysPerPhase.Count)
                {
                    currentPhase = daysPerPhase.Count;
                }

                currentPhase -= 1;
                var daysInCurrentPhase = daysPerPhase[currentPhase];
                dayOfCurrentPhase += daysInCurrentPhase;
            }

            if (dayOfCurrentPhase < 0)
            {
                dayOfCurrentPhase = 0;
            }

            crop.currentPhase.Set(currentPhase);
            crop.dayOfCurrentPhase.Set(dayOfCurrentPhase);
            // private Vector2 tilePosition;
            var tilePositionField = _helper.Reflection.GetField<Vector2>(crop, "tilePosition");
            crop.updateDrawMath(tilePositionField.GetValue());
        }

        private void ActivateInflation()
        {
            Game1.player.RemoveMail("spring_1_2");
            Game1.player.mailForTomorrow.Add("spring_1_2");
        }

        // public override int salePrice()
        public static bool SalePrice_GetCorrectInflation_Prefix(Object __instance, ref int __result)
        {
            try
            {
                switch (__instance.ParentSheetIndex)
                {
                    case 378:
                        __result = GetInflatedPrice(80);
                        return false; // don't run original logic
                    case 380:
                        __result = GetInflatedPrice(150);
                        return false; // don't run original logic
                    case 382:
                        __result = GetInflatedPrice(120);
                        return false; // don't run original logic
                    case 384:
                        __result = GetInflatedPrice(350);
                        return false; // don't run original logic
                    case 388:
                        __result = GetInflatedPrice(10);
                        return false; // don't run original logic
                    case 390:
                        __result = GetInflatedPrice(20);
                        return false; // don't run original logic
                    default:
                        return true; // run original logic
                }
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(SalePrice_GetCorrectInflation_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static int GetInflatedPrice(int price)
        {
            var inflationRate = _difficultyBalancer.InflationAmount[_archipelago.SlotData.TrapItemsDifficulty];
            var totalInflation = Math.Pow(inflationRate, _archipelago.GetReceivedItemCount("Inflation Trap"));
            return (int)(price * totalInflation);
        }

        private void Explode()
        {
            var explosionRadius = _difficultyBalancer.ExplosionSize[_archipelago.SlotData.TrapItemsDifficulty];

            var location = Game1.player.currentLocation;
            var tile = Game1.player.Tile;
            var x = tile.X * 64;
            var y = tile.Y * 64;
            // protected internal static Multiplayer multiplayer = new Multiplayer();
            var multiplayerField = _helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer");
            var multiplayer = multiplayerField.GetValue();
            var parentSheetIndex = 287;
            if (explosionRadius < 5)
            {
                parentSheetIndex = 286;
            }
            if (explosionRadius > 5)
            {
                parentSheetIndex = 288;
            }

            var randomId = Game1.random.Next();
            location.playSound("thudStep");
            var bombSprite = new TemporaryAnimatedSprite(parentSheetIndex, 100f, 1, 24, tile * 64f, true, false, location, Game1.player)
            {
                shakeIntensity = 0.5f,
                shakeIntensityChange = 1f / 500f,
                extraInfoForEndBehavior = randomId,
                endFunction = location.removeTemporarySpritesWithID,
                bombRadius = explosionRadius,
            };
            multiplayer.broadcastSprites(location, bombSprite);
            multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(598, 1279, 3, 4), 53f, 5, 9, tile * 64f + new Vector2(5f, 3f) * 4f, true, false, (y + 7) / 10000f, 0.0f, Color.Yellow, 4f, 0.0f, 0.0f, 0.0f)
            {
                id = randomId,
            });
            multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(598, 1279, 3, 4), 53f, 5, 9, tile * 64f + new Vector2(5f, 3f) * 4f, true, true, (y + 7) / 10000f, 0.0f, Color.Orange, 4f, 0.0f, 0.0f, 0.0f)
            {
                delayBeforeAnimationStart = 50,
                id = randomId,
            });
            multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(598, 1279, 3, 4), 53f, 5, 9, tile * 64f + new Vector2(5f, 3f) * 4f, true, false, (y + 7) / 10000f, 0.0f, Color.White, 3f, 0.0f, 0.0f, 0.0f)
            {
                delayBeforeAnimationStart = 100,
                id = randomId,
            });
            location.netAudio.StartPlaying("fuse");
        }

        private IEnumerable<FruitTree> GetAllFruitTrees()
        {
            foreach (var gameLocation in Game1.locations)
            {
                foreach (var terrainFeature in gameLocation.terrainFeatures.Values)
                {
                    if (terrainFeature is not FruitTree fruitTree)
                    {
                        continue;
                    }

                    yield return fruitTree;
                }
            }
        }

        private void UngrowFruitTree(FruitTree fruitTree, int days)
        {
            fruitTree.daysUntilMature.Value += days;
        }
    }
}
