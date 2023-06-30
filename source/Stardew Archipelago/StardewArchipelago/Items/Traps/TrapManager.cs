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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Netcode;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Extensions;
using StardewArchipelago.Items.Mail;
using StardewArchipelago.Locations.CodeInjections;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

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

        private readonly IModHelper _helper;
        private readonly ArchipelagoClient _archipelago;
        private readonly TrapDifficultyBalancer _difficultyBalancer;
        private readonly TileChooser _tileChooser;
        private readonly MonsterSpawner _monsterSpawner;
        private readonly InventoryShuffler _inventoryShuffler;
        private Dictionary<string, Action> _traps;

        public TrapManager(IModHelper helper, ArchipelagoClient archipelago, TileChooser tileChooser)
        {
            _helper = helper;
            _archipelago = archipelago;
            _difficultyBalancer = new TrapDifficultyBalancer();
            _tileChooser = tileChooser;
            _monsterSpawner = new MonsterSpawner(_tileChooser);
            _inventoryShuffler = new InventoryShuffler();
            _traps = new Dictionary<string, Action>();
            RegisterTraps();
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
            if (Game1.player.currentLocation is FarmHouse or IslandFarmHouse)
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
            _traps.Add(DEBRIS, CreateDebris);
            _traps.Add(SHUFFLE, ShuffleInventory);
            // _traps.Add(WINTER, );
            _traps.Add(PARIAH, SendDislikedGiftToEveryone);
            _traps.Add(DROUGHT, PerformDroughtTrap);

            foreach (var trapName in _traps.Keys.ToArray())
            {
                var differentSpacedTrapName = trapName.Replace(" ", "_");
                if (differentSpacedTrapName != trapName)
                {
                    _traps.Add(differentSpacedTrapName, _traps[trapName]);
                }
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

        private void AddJinxedDebuff()
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
            var debuff = new Buff((int)whichBuff);
            debuff.millisecondsDuration = (int)duration;
            debuff.totalMillisecondsDuration = (int)duration;
            Game1.buffsDisplay.addOtherBuff(debuff);
        }

        private void ChargeTaxes()
        {
            var taxRate = _difficultyBalancer.TaxRates[_archipelago.SlotData.TrapItemsDifficulty];
            var player = Game1.player;
            var currentMoney = player.Money;
            var tax = (int)(currentMoney * taxRate);
            Game1.player.addUnearnedMoney(tax * -1);
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
                    chosenTile = _tileChooser.GetRandomTileInbounds(chosenLocation, Game1.player.getTileLocationPoint(), 20);
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
            for (int index = 0; index < 12; ++index)
            {
                multiplayer.broadcastSprites(farmer.currentLocation, new TemporaryAnimatedSprite(354, Game1.random.Next(25, 75), 6, 1, new Vector2(Game1.random.Next((int)farmer.position.X - 256, (int)farmer.position.X + 192), Game1.random.Next((int)farmer.position.Y - 256, (int)farmer.position.Y + 192)), false, Game1.random.NextDouble() < 0.5));
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
            for (var x1 = farmer.getTileX() + 8; x1 >= farmer.getTileX() - 8; --x1)
            {
                multiplayer.broadcastSprites(farmer.currentLocation, new TemporaryAnimatedSprite(6, new Vector2(x1, farmer.getTileY()) * 64f, Color.White, animationInterval: 50f)
                {
                    layerDepth = 1f,
                    delayBeforeAnimationStart = num * 25,
                    motion = new Vector2(-0.25f, 0.0f)
                });
                ++num;
            }
        }

        private void AfterTeleport(Farmer farmer, string locationName, Vector2 tile)
        {
            var destination = Utility.Vector2ToPoint(tile);
            Game1.warpFarmer(locationName, destination.X, destination.Y, false);
            if (!Game1.isStartingToGetDarkOut() && !Game1.isRaining)
                Game1.playMorningSong();
            else
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

            var farm = Game1.getFarm();
            SendCrowsForLocation(farm, crowRate);
            if (crowTargets == CrowTargets.Farm)
            {
                return;
            }

            var islandSouth = Game1.getLocationFromName("IslandSouth");
            SendCrowsForLocation(islandSouth, crowRate);

            if (crowTargets == CrowTargets.Island)
            {
                return;
            }

            var greenHouse = Game1.getLocationFromName("Greenhouse");
            SendCrowsForLocation(greenHouse, crowRate);
        }

        private static void SendCrowsForLocation(GameLocation map, double crowRate)
        {
            var scarecrowPositions = GetScarecrowPositions(map);
            var vulnerableCrops = GetAllVulnerableCrops(map, scarecrowPositions);
            var numberCrowsToSend = vulnerableCrops.Count * crowRate;
            map.critters ??= new List<Critter>();
            for (var index1 = 0; index1 < numberCrowsToSend; ++index1)
            {
                var chosenIndex = Game1.random.Next(vulnerableCrops.Count);
                var cropToEat = vulnerableCrops[chosenIndex];
                vulnerableCrops.RemoveAt(chosenIndex);
                cropToEat.destroyCrop(cropToEat.currentTileLocation, true, map);
                map.critters.Add(new Crow((int)cropToEat.currentTileLocation.X, (int)cropToEat.currentTileLocation.Y));
            }
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

        private static List<HoeDirt> GetAllVulnerableCrops(GameLocation farm, List<Vector2> scarecrowPositions)
        {
            var vulnerableCrops = new List<HoeDirt>();
            foreach (var (cropPosition, cropTile) in farm.terrainFeatures.Pairs)
            {
                if (cropTile is not HoeDirt dirt || dirt.crop == null || dirt.crop.currentPhase.Value <= 1)
                {
                    continue;
                }

                var isVulnerable = IsCropVulnerable(farm, scarecrowPositions, cropPosition);
                if (isVulnerable)
                {
                    vulnerableCrops.Add(dirt);
                }
            }

            return vulnerableCrops;
        }

        private static bool IsCropVulnerable(GameLocation farm, List<Vector2> scarecrowPositions, Vector2 cropPosition)
        {
            foreach (var scarecrowPosition in scarecrowPositions)
            {
                var radiusForScarecrow = farm.objects[scarecrowPosition].GetRadiusForScarecrow();
                if (Vector2.Distance(scarecrowPosition, cropPosition) < radiusForScarecrow)
                {
                    return false;
                }
            }

            return true;
        }

        private void SpawnMonsters()
        {
            var numberMonsters = _difficultyBalancer.NumberOfMonsters[_archipelago.SlotData.TrapItemsDifficulty];
            for (var i = 0; i < numberMonsters; i++)
            {
                _monsterSpawner.SpawnOneMonster(Game1.player.currentLocation);
            }
        }

        private void CreateDebris()
        {
            var farm = Game1.getFarm();
            var currentLocation = Game1.player.currentLocation;
            var locations = new List<GameLocation> { farm };
            if (currentLocation != farm)
            {
                locations.Add(currentLocation);
            }
            
            var amountOfDebris = _difficultyBalancer.AmountOfDebris[_archipelago.SlotData.TrapItemsDifficulty];
            var amountOfDebrisPerLocation = amountOfDebris / locations.Count;
            foreach (var gameLocation in locations)
            {
                gameLocation.spawnWeedsAndStones(amountOfDebrisPerLocation);
            }
        }

        private void ShuffleInventory()
        {
            var targets = _difficultyBalancer.ShuffleInventoryTargets[_archipelago.SlotData.TrapItemsDifficulty];
            _inventoryShuffler.ShuffleInventories(targets);
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

                    foreach (var chestItem in chest.items)
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
    }
}
