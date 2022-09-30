/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using StardewRoguelike.VirtualProperties;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using System.Collections.Generic;
using System;
using StardewRoguelike.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using System.Linq;
using xTile.Dimensions;
using StardewValley.Monsters;
using xTile.Tiles;
using StardewRoguelike.Netcode;
using Force.DeepCloner;
using StardewRoguelike.Enchantments;
using System.Reflection;
using StardewRoguelike.Patches;

namespace StardewRoguelike
{
    public static class Roguelike
    {
        public static int FloorRngSeed { get; set; } = Guid.NewGuid().GetHashCode();

        public static readonly string SaveFile = "Roguelike_311053312";

        public static readonly List<string> ValidMineMaps = new() {
            "2", "3", "4", "5", "6", "7",
            "8", "9", "11", "13", "15",
            "21", "23", "25", "26", "27",
            "custom-1", "custom-2", "custom-3",
            "custom-4", "custom-5", "custom-6",
            "custom-7"
        };

        public static readonly List<string> MapsWithWater = new()
        {
            "custom-1", "custom-2", "custom-3"
        };

        public static Random FloorRng { get; set; } = new(FloorRngSeed);

        public static readonly List<string> SeenMineMaps = new();

        public static readonly List<int> ScalingOrder = new() { 6, 12, 18, 24, 30, 36, 42, 48 };
        public static readonly int DangerousThreshold = 24;

        public static readonly List<int> FloorsIncreaseGoldMax = new() { 6 };
        public static readonly List<int> FloorsIncreaseGoldMin = new() { 24 };

        public static readonly int StartingGold = 100;

        public static readonly int StartingHP = 50;
        public static readonly int MaxHP = 150;

        public static int TrueMaxHP = Game1.player.maxHealth;
        public static int MilksBought = 0;

        public static bool HardMode
        {
            get => Game1.player.team.get_FarmerTeamHardMode().Value;
            set => Game1.player.team.get_FarmerTeamHardMode().Value = value;
        }

        public static readonly int MinimumMonstersPerFloor = 5;
        public static readonly int MaximumMonstersPerFloorPreLoop = 15;
        public static readonly int MaximumMonstersPerFloorPostLoop = 30;

        public static int GoldDropMax = 2;
        public static int GoldDropMin = 1;

        public static int CurrentLevel = 0;

        private static bool DidHardModeDowngrade = false;

        public static void AdjustMonster(MineShaft mine, ref Monster monster)
        {
            if (HardMode)
                monster.DamageToFarmer += (int)Math.Round(monster.DamageToFarmer * 0.25f);

            Curse.AdjustMonster(ref monster);
        }

        public static void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Curse.HasCurse(CurseType.GlassCannon))
                Game1.player.maxHealth = TrueMaxHP / 2;
            else
                Game1.player.maxHealth = TrueMaxHP;

            if (Game1.player.health > Game1.player.maxHealth)
                Game1.player.health = Game1.player.maxHealth;

            if (e.IsOneSecond && FarmerTakeDamagePatch.ShellCooldownSeconds > 0 && Game1.shouldTimePass())
                FarmerTakeDamagePatch.ShellCooldownSeconds--;

            if (HardMode && !DidHardModeDowngrade && Game1.player.hasItemInInventory(194, 1))
            {
                Game1.player.removeFirstOfThisItemFromInventory(194);
                DelayedAction.playSoundAfterDelay("clank", 250);
                DidHardModeDowngrade = true;
            }
            else if (!HardMode && DidHardModeDowngrade)
            {
                Game1.player.addItemToInventory(new StardewValley.Object(194, 1));
                DelayedAction.playSoundAfterDelay("clank", 250);
                DidHardModeDowngrade = false;
            }

            if (Game1.player.currentLocation is MineShaft)
            {
                MineShaft mine = Game1.player.currentLocation as MineShaft;
                var localChests = mine.get_MineShaftNetChests();
                foreach (NetChest chest in localChests)
                    chest.Spawn(mine);
            }

            int buffId = 88999;
            Buff buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == buffId);
            if (buff == null)
            {
                Game1.buffsDisplay.addOtherBuff(
                    buff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, speed: 1, 0, 0, minutesDuration: 1, source: "Roguelike", displaySource: "Adrenaline") { which = buffId }
                );
            }
            buff.millisecondsDuration = 50;

            if (Game1.player.Stamina < 0 && Game1.player.health > 0)
            {
                Game1.player.health = 0;
                Game1.playSound("ow");
            }

            if (Game1.killScreen)
                HandleDeath();

            if (ModEntry.Stats.StartTime is null && MineShaft.activeMines.Count > 0)
            {
                MineShaft firstMine = null;
                foreach (MineShaft mine in MineShaft.activeMines)
                {
                    if (GetLevelFromMineshaft(mine) == 1)
                    {
                        firstMine = mine;
                        break;
                    }
                }

                if (firstMine is not null)
                {
                    long firstEntryTime = firstMine.get_MineShaftEntryTime().Value;
                    DateTime unix = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    ModEntry.Stats.StartTime = unix.AddSeconds(firstEntryTime);
                }
            }

            if (ModEntry.Stats.DinoKillEndTime is null && e.IsMultipleOf(15))
            {
                bool allPlayersPassed = true;
                foreach (Farmer farmer in Game1.getOnlineFarmers())
                {
                    if (farmer.get_FarmerCurrentLevel().Value < ScalingOrder[^1])
                    {
                        allPlayersPassed = false;
                        break;
                    }
                }

                if (allPlayersPassed)
                {
                    ModEntry.Stats.DinoKillEndTime = DateTime.UtcNow;
                    StatsMenu.Show();
                }
            }
        }

        public static void PlayerWarped(object sender, WarpedEventArgs e)
        {
            if (e.Player != Game1.player)
                return;

            if (e.NewLocation is Mine)
            {
                Game1.player.get_FarmerCurrentLevel().Value = 0;
                PopulateQiDialogue(e.NewLocation);
            }
            else if (e.NewLocation is MineShaft mine)
            {
                int level = GetLevelFromMineshaft(mine);
                Game1.player.get_FarmerCurrentLevel().Value = level;

                if (Merchant.IsMerchantFloor(level) && !Merchant.ShouldSpawnGil(level))
                    Merchant.DespawnGil(mine);
            }
        }

        public static void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            ModEntry.ModMonitor.Log($"Initialized floor generation with seed {FloorRngSeed}", LogLevel.Debug);

            Game1.options.screenFlash = false;
            Game1.options.zoomButtons = true;
            ModEntry.DisableUpload = false;

            List<IModInfo> invalidMods = ModEntry.GetInvalidMods();
            if (invalidMods.Count > 0)
                ModEntry.ShouldShowModDisclaimer = true;

            WarpLocalPlayerToStart();
            Game1.player.team.useSeparateWallets.Value = true;

            GameLocation mine = Game1.getLocationFromName("Mine");
            NPC qi = mine.getCharacterFromName("Mister Qi");
            if (qi is null)
            {
                qi = Game1.getCharacterFromName("Mister Qi");
                qi = qi.ShallowClone();
            }

            qi.showTextAboveHead("Hey there!", duration: 15000);

            if (Context.IsMainPlayer)
            {
                HardMode = false;
                ModEntry.Stats.HardMode = false;

                qi.setTileLocation(new(17, 6));
                qi.faceDirection(2);
                mine.addCharacter(qi);

                Game1.player.Name = "";
                Game1.player.favoriteThing.Value = "";
            }

            if (!ModEntry.Config.SkipCharacterCreation)
                Game1.activeClickableMenu = new CharacterCustomization(CharacterCustomization.Source.NewFarmhand);
            else if (!ModEntry.Config.SkipModDisclaimer && ModEntry.ShouldShowModDisclaimer)
            {
                Game1.activeClickableMenu = new DisclaimerMenu();
                ModEntry.ShouldShowModDisclaimer = false;
            }

            ResetLocalGameState();
            ResetLocalPlayer();
        }

        public static void ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            if (CurrentLevel != 0)
            {
                CurrentLevel--;
                ModEntry.Stats.FloorsDescended--;
            }
        }

        public static void TimeChanged(object sender, TimeChangedEventArgs e)
        {
            if (e.NewTime >= 2300)
                Game1.timeOfDay = 600;
        }

        public static void NextFloor()
        {
            if (CurrentLevel == 0)
                ModEntry.Stats.Reset();

            CurrentLevel++;
            ModEntry.Stats.FloorsDescended++;
            Merchant.CurrentShop = null;
            Perks.CurrentMenu = null;

            if (FloorsIncreaseGoldMax.Contains(CurrentLevel))
                GoldDropMax++;
            else if (FloorsIncreaseGoldMin.Contains(CurrentLevel))
                GoldDropMin++;

            if (Context.IsMainPlayer)
                ClearInactiveMines();
        }

        // This method is a bandaid until SMAPI merges my pull request
        public static void ClearInactiveMines()
        {
            int instancesToKeep = 10;

            if (MineShaft.activeMines.Count < instancesToKeep)
                return;

            if (!Context.IsMultiplayer)
            {
                LocalizedContentManager mapContent = (LocalizedContentManager)MineShaft.activeMines[0].GetType().GetField("mapContent", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(MineShaft.activeMines[0]);
                mapContent.Dispose();
                MineShaft.activeMines.RemoveAt(0);
                return;
            }

            int playersAccountedFor = 0;
            int playersToAccountFor = Game1.getOnlineFarmers().Count;
            int floorsSinceLastPlayer = 0;
            int amountToRemove = 0;
            int merchantInterval = ScalingOrder[1] - ScalingOrder[0];

            for (int i = MineShaft.activeMines.Count - 1; i == 0; i--)
            {
                bool allPlayersFound = playersAccountedFor == playersToAccountFor;
                if (allPlayersFound && floorsSinceLastPlayer < merchantInterval)
                {
                    floorsSinceLastPlayer++;
                    continue;
                }
                else if (allPlayersFound && floorsSinceLastPlayer >= merchantInterval)
                {
                    amountToRemove++;
                    continue;
                }

                MineShaft mine = MineShaft.activeMines[i];
                playersAccountedFor += mine.farmers.Count;
            }

            while (amountToRemove > 0 && MineShaft.activeMines.Count >= instancesToKeep)
            {
                LocalizedContentManager mapContent = (LocalizedContentManager)MineShaft.activeMines[0].GetType().GetField("mapContent", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(MineShaft.activeMines[0]);
                mapContent.Dispose();
                MineShaft.activeMines.RemoveAt(0);
            }
        }

        public static string GetRandomTrack(List<string> tracks)
        {
            return tracks[Game1.random.Next(tracks.Count)];
        }

        public static int GetFloorDepth(MineShaft mine, int floor)
        {
            int level = GetLevelFromMineshaft(mine);

            if (BossFloor.IsBossFloor(level))
                return floor;

            int result;
            if (level < ScalingOrder[0])
                result = 20;
            else if (level < ScalingOrder[1])
                result = 60;
            else if (level < ScalingOrder[2])
                result = 100;
            else if (level < ScalingOrder[3])
                result = 121;
            else if (level < ScalingOrder[4])
                result = 20;
            else if (level < ScalingOrder[5])
                result = 60;
            else if (level < ScalingOrder[6])
                result = 100;
            else if (level < ScalingOrder[7])
                result = 179;
            else
                result = 179;

            if (mine.get_MineShaftIsDarkArea())
                result += 10;

            return result;
        }

        public static string GetMapPath(MineShaft mine, bool avoidWater = false)
        {
            if (Merchant.IsMerchantFloor(mine))
                return Merchant.GetMapPath(mine);
            else if (BossFloor.IsBossFloor(mine))
                return BossFloor.GetMapPath(mine);
            else if (ForgeFloor.IsForgeFloor(mine))
                return ForgeFloor.ForgeFloorMapPath;
            else if (ChestFloor.IsChestFloor(mine))
                return ChestFloor.ChestFloorMapPath;
            else if (ChallengeFloor.IsChallengeFloor(mine))
                return ChallengeFloor.GetMapPath(mine);

            var validOptions = ValidMineMaps;

            if (avoidWater)
                validOptions.RemoveAll(floor => MapsWithWater.Contains(floor));

            if (SeenMineMaps.Count >= validOptions.Count)
                SeenMineMaps.Clear();

            string chosenLayout;
            do
            {
                chosenLayout = validOptions[FloorRng.Next(validOptions.Count)];
            } while (SeenMineMaps.Contains(chosenLayout));
            SeenMineMaps.Add(chosenLayout);

            return chosenLayout;
        }

        public static void AddDefaultItemsToInventory()
        {
            Pickaxe pick = new()
            {
                UpgradeLevel = 4
            };
            Game1.player.addItemToInventory(pick);                // Copper Pickaxe
            Game1.player.addItemToInventory(new MeleeWeapon(0));  // Rusty Sword

            int eggStack = HardMode ? 2 : 3;
            Game1.player.addItemToInventory(new StardewValley.Object(194, eggStack));  // Fried Egg
        }

        public static void HandleDeath()
        {
            Game1.killScreen = false;
            Game1.player.exhausted.Value = false;

            if (Context.IsMultiplayer)
                HandleMultiplayerDeath();
            else
                HandleLocalDeath();
        }

        public static void HandleLocalDeath()
        {
            Game1.showGlobalMessage($"You survived {CurrentLevel - 1} floors!");

            ModEntry.Invincible = true;
            Game1.player.temporarilyInvincible = true;
            Game1.player.maxHealth = StartingHP;
            Game1.player.health = StartingHP;
            Game1.player.Stamina = Game1.player.MaxStamina;

            DelayedAction.functionAfterDelay(GameOver, 7000);
        }

        public static void HandleMultiplayerDeath()
        {
            ModEntry.Invincible = true;
            Game1.player.health = Game1.player.maxHealth;

            DelayedAction.functionAfterDelay(() =>
            {
                Game1.screenGlow = false;
                Game1.player.temporarilyInvincible = false;

                List<Farmer> aliveFarmers = SpectatorMode.GetAliveFarmers();
                aliveFarmers.RemoveAll(f => f == Game1.player);
                if (aliveFarmers.Count == 0)
                {
                    // Everyone is dead :(
                    ModEntry.MultiplayerHelper.SendMessage(
                        "GameOver",
                        "GameOver"
                    );
                    GameOver();
                }
                else
                {
                    // The run lives!
                    SpectatorMode.EnterSpectatorMode();
                    Game1.showGlobalMessage($"You will be revived at the next merchant.");
                }
            }, 7000);

        }

        public static void GameOver()
        {
            if (Game1.player.get_FarmerIsSpectating().Value)
                SpectatorMode.ExitSpectatorMode();

            Game1.showGlobalMessage($"You survived {ModEntry.Stats.FloorsDescended} floors!");

            WarpLocalPlayerToStart();
            Game1.player.temporarilyInvincible = false;
            CurrentLevel = 0;
            Game1.screenGlow = false;
            ModEntry.Stats.EndTime = DateTime.UtcNow;

            ResetLocalGameState();
            ResetLocalPlayer();

            StatsMenu.Show();
        }

        public static void ResetLocalGameState()
        {
            CurrentLevel = 0;
            GoldDropMax = 2;
            GoldDropMin = 1;

            Game1.flushLocationLookup();
            MineShaft.clearActiveMines();

            if (Context.IsMainPlayer)
                SeenMineMaps.Clear();
        }

        public static void ResetLocalPlayer()
        {
            Curse.RemoveAllCurses();
            Perks.RemoveAllPerks();

            ModEntry.Invincible = false;

            Game1.player.get_FarmerIsSpectating().Value = false;

            Game1.player.health = StartingHP;
            Game1.player.maxHealth = StartingHP;
            TrueMaxHP = StartingHP;

            Game1.player.Stamina = Game1.player.MaxStamina;
            Game1.player.Money = StartingGold;

            Game1.player.hasSkullKey = true;
            Game1.player.MagneticRadius = 256;

            Game1.player.ClearBuffs();

            // Remove items
            if (Game1.player.leftRing.Value is not null)
            {
                Ring ring = Game1.player.leftRing.Value;
                ring.onUnequip(Game1.player, Game1.player.currentLocation);
            }

            if (Game1.player.rightRing.Value is not null)
            {
                Ring ring = Game1.player.rightRing.Value;
                ring.onUnequip(Game1.player, Game1.player.currentLocation);
            }

            if (Game1.player.boots.Value is not null)
            {
                Boots boots = Game1.player.boots.Value;
                boots.onUnequip();
            }

            Game1.player.leftRing.Set(null);
            Game1.player.rightRing.Set(null);
            Game1.player.boots.Set(null);
            Game1.player.hat.Value = null;
            Game1.player.CursorSlotItem = null;

            Game1.player.enchantments.Clear();
            Game1.player.enchantments.Add(new WeaponStatTrack());

            Game1.player.hasUsedDailyRevive.Value = false;
            MilksBought = 0;

            Game1.player.craftingRecipes.Clear();
            Game1.player.craftingRecipes.Add("Wood Fence", 0);

            for (int i = 0; i < Game1.player.Items.Count; i++)
                Game1.player.Items[i] = null;

            AddDefaultItemsToInventory();
        }

        public static void WarpLocalPlayerToStart()
        {
            Game1.warpFarmer("Mine", 17, 15, 2);
        }

        public static void MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.OldMenu is not DialogueBox dialogue || (dialogue.characterDialogue is not null && dialogue.characterDialogue.speaker is null))
                return;

            if (Game1.currentLocation is MineShaft mine && Merchant.IsMerchantFloor(mine))
                Merchant.PopulateQiDialogue(mine);
            else
                PopulateQiDialogue(Game1.currentLocation);
        }

        public static void PopulateQiDialogue(GameLocation lobby)
        {
            NPC qi = lobby.getCharacterFromName("Mister Qi");
            if (qi is null)
                return;

            qi.CurrentDialogue.Clear();

            string perkKey = string.Join('+', Game1.options.journalButton);
            qi.setNewDialogue(
                "Welcome to The Abyss! This will be your final challenge before you're " +
                "ready to reach the summit.#$b#You must defeat all the bosses that lie beneath. Killing enemies will " +
                "reward you with gold that you can use as currency at the shops.#$b#" +
                $"Below, you will also find vendors for perks and curses. View your active perks by pressing {perkKey}.#$b#" +
                "Your tracking chip will keep note of all your statistics. You can view them by typing /stats. " +
                "You can reset your journey at any time with the /reset command.#$b#" +
                "At any time, if you get stuck with nowhere to go, use /stuck for a helping hand.#$b#" +
                "If you're up for it, check out that statue over there for a more difficult time.#$b#" +
                "Good luck, kid."
            );
        }

        public static bool PerformAction(GameLocation location, string action, Farmer who, Location tileLocation)
        {
            if (action == "HardMode" && Context.IsMainPlayer)
            {
                var responses = location.createYesNoResponses();
                if (HardMode)
                    location.createQuestionDialogue("Exit hard mode?", responses, "hardMode");
                else
                    location.createQuestionDialogue("Enter hard mode?", responses, "hardMode");
                return true;
            }

            return false;
        }

        public static bool AnswerDialogueAction(GameLocation mine, string questionAndAnswer, string[] questionParams)
        {
            if (questionAndAnswer != "hardMode_Yes")
                return false;

            if (HardMode)
            {
                HardMode = false;
                Game1.drawObjectDialogue("Couldn't handle the heat?");
            }
            else
            {
                HardMode = true;
                Game1.drawObjectDialogue("Your journey has begun.");
            }

            ModEntry.Stats.Reset();
            ModEntry.Stats.HardMode = HardMode;

            return true;
        }

        public static int GetHighestMineShaftLevel()
        {
            int highestFloor = 0;
            foreach (MineShaft mine in MineShaft.activeMines)
            {
                int floor = GetLevelFromMineshaft(mine);
                if (floor > highestFloor)
                    highestFloor = floor;
            }

            return highestFloor;
        }

        public static int GetLowestMineShaftLevel()
        {
            int lowestFloor = GetHighestMineShaftLevel();
            foreach (MineShaft mine in MineShaft.activeMines)
            {
                int floor = GetLevelFromMineshaft(mine);
                if (floor < lowestFloor)
                    lowestFloor = floor;
            }

            return lowestFloor;
        }

        public static int GetLevelFromMineshaft(MineShaft mine)
        {
            return mine.get_MineShaftLevel().Value;
        }

        public static (int, int) GetBarrelDrops(MineShaft mine)
        {
            int itemId;
            int quantity = 1;

            double roll = Game1.random.NextDouble();

            if (Perks.HasPerk(Perks.PerkType.BarrelEnthusiast))
                roll = Math.Min(Game1.random.NextDouble(), roll);

            if (roll <= 0.0001)
                itemId = 74;
            else if (roll <= 0.005)
                itemId = 72;
            else if (roll <= 0.0125)
                itemId = 60;
            else if (roll <= 0.02)
                itemId = 64;
            else if (roll <= 0.04)
                itemId = 62;
            else if (roll <= 0.07)
                itemId = 68;
            else if (roll <= 0.1)
                itemId = 66;
            else if (roll <= 0.2)
                itemId = 78;
            else if (roll <= 0.8)
            {
                itemId = 384;
                quantity = Game1.random.Next(GoldDropMin, GoldDropMax + 1);
            }
            else
            {
                itemId = 0;
                quantity = 0;
            }

            // 20% nothing
            // 60% gold
            // 10% cave carrot 1-2
            // 3% amethyst 1
            // 3% topaz 1
            // 2% aquamarine 1
            // 0.75% ruby 1
            // 0.75% emerald 1
            // 0.49% diamond 1
            // 0.01% prizzy 1

            return (itemId, quantity);
        }

        /// <summary>
        /// Finds the index for a tile in a map with many tilesheets.
        /// </summary>
        /// <param name="map">The map to look through</param>
        /// <param name="tileSheetId">The ID (string name) of the tilesheet</param>
        /// <param name="tileId">The tile ID in the passed tilesheet</param>
        /// <param name="doSum">Whether or not to return the tile ID as the sum of all previous or standalone</param>
        /// <returns>tilesheet index, tile ID</returns>
        /// <exception cref="Exception">The tilesheet passed could not be found.</exception>
        public static (int, int) GetTileIndexForMap(xTile.Map map, string tileSheetId, int tileId, bool doSum = false)
        {
            int idSum = 0;
            for (int i = 0; i < map.TileSheets.Count; i++)
            {
                TileSheet tileSheet = map.TileSheets[i];
                if (tileSheet.Id == tileSheetId)
                    return (i, doSum ? tileId + idSum : tileId);

                idSum += tileSheet.TileCount - 1;
            }

            throw new Exception("Could not find tile sheet with id " + tileSheetId);
        }
    }
}
