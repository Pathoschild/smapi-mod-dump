/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
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
using StardewRoguelike.Extensions;
using Microsoft.Xna.Framework;
using StardewValley.Minigames;
using StardewRoguelike.HatQuests;
using StardewModdingAPI.Utilities;

namespace StardewRoguelike
{
    public static class Roguelike
    {
        public static int FloorRngSeed { get; set; } = Guid.NewGuid().GetHashCode();

        public static bool RerollRandomEveryRun { get; set; } = true;

        public static Random FloorRng { get; set; } = new(FloorRngSeed);

        public static readonly List<string> SeenMineMaps = new();

        public static readonly int StartingGold = 100;

        public static readonly int StartingHP = 50;
        public static readonly int MaxHP = 150;

        private static readonly PerScreen<int> ScreenTrueMaxHP = new(() => Game1.player.maxHealth);
        private static readonly PerScreen<int> ScreenMilksBought = new(() => 0);

        public static int TrueMaxHP
        {
            get => ScreenTrueMaxHP.Value;
            set => ScreenTrueMaxHP.Value = value;
        }

        public static int MilksBought
        {
            get => ScreenMilksBought.Value;
            set => ScreenMilksBought.Value = value;
        }

        public static bool HardMode
        {
            get => Game1.player.team.get_FarmerTeamHardMode().Value;
            set => Game1.player.team.get_FarmerTeamHardMode().Value = value;
        }

        private static readonly PerScreen<int> ScreenGoldDropMax = new(() => 2);
        private static readonly PerScreen<int> ScreenGoldDropMin = new(() => 1);
        private static readonly PerScreen<int> ScreenCurrentLevel = new(() => 0);
        private static readonly PerScreen<bool> ScreenDidHardModeDowngrade = new(() => false);
        private static readonly PerScreen<IMinigame?> ScreenActiveMinigame = new(() => null);
        private static readonly PerScreen<int> ScreenFloorTickCounter = new(() => 0);

        public static int GoldDropMax
        {
            get => ScreenGoldDropMax.Value;
            set => ScreenGoldDropMax.Value = value;
        }

        public static int GoldDropMin
        {
            get => ScreenGoldDropMin.Value;
            set => ScreenGoldDropMin.Value = value;
        }

        public static int CurrentLevel
        {
            get => ScreenCurrentLevel.Value;
            set => ScreenCurrentLevel.Value = value;
        }

        private static bool DidHardModeDowngrade
        {
            get => ScreenDidHardModeDowngrade.Value;
            set => ScreenDidHardModeDowngrade.Value = value;
        }

        private static IMinigame? ActiveMinigame
        {
            get => ScreenActiveMinigame.Value;
            set => ScreenActiveMinigame.Value = value;
        }

        public static int FloorTickCounter
        {
            get => ScreenFloorTickCounter.Value;
            set => ScreenFloorTickCounter.Value = value;
        }

        public static void AdjustMonster(MineShaft mine, ref Monster monster)
        {
            int level = GetLevelFromMineshaft(mine);

            if (level >= Constants.ScalingOrder[^1])
            {
                int levelsPostLoop = level - Constants.ScalingOrder[^1];
                int postLoopHealth = Math.Min((int)(Game1.random.Next(450, 550) * (1 + (levelsPostLoop / 48f))), 1000);
                monster.MaxHealth = Math.Max(monster.MaxHealth, postLoopHealth);
                monster.Health = monster.MaxHealth;
                monster.DamageToFarmer = Math.Max(monster.DamageToFarmer, (int)(25 * BossFloor.GetLevelDifficulty(level)));
            }

            if (HardMode)
                monster.DamageToFarmer += (int)Math.Round(monster.DamageToFarmer * 0.25f);

            Curse.AdjustMonster(ref monster);
        }

        public static void UpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Game1.currentMinigame is not null && ActiveMinigame is null)
                ActiveMinigame = Game1.currentMinigame;
            else if (Game1.currentMinigame is null && ActiveMinigame is not null)
            {
                Minigames.MinigameClosed(ActiveMinigame);
                ActiveMinigame = null;
            }

            bool hasFiberHat = HatQuest.HasHat(40);
            int trueMax = TrueMaxHP + (hasFiberHat ? 25 : 0);
            if (Curse.HasCurse(CurseType.GlassCannon))
                Game1.player.maxHealth = trueMax / 2;
            else
                Game1.player.maxHealth = trueMax;

            if (Game1.player.health > Game1.player.maxHealth)
                Game1.player.health = Game1.player.maxHealth;

            if (e.IsOneSecond && FarmerTakeDamagePatch.ShellCooldownSeconds > 0 && Game1.shouldTimePass())
                FarmerTakeDamagePatch.ShellCooldownSeconds--;

            HatQuest? quest = Game1.player.get_FarmerActiveHatQuest();
            if (quest is not null && quest.IsComplete())
            {
                quest.GiveHat();
                Game1.player.set_FarmerActiveHatQuest(null);
            }

            if (HardMode && !DidHardModeDowngrade && Game1.player.hasItemInInventory(194, 1))
            {
                Game1.player.removeFirstOfThisItemFromInventory(194);
                DelayedAction.playSoundAfterDelay("clank", 250);
                DidHardModeDowngrade = true;
            }
            else if (!HardMode && DidHardModeDowngrade)
            {
                Game1.player.addItemToInventory(new SObject(194, 1));
                DelayedAction.playSoundAfterDelay("clank", 250);
                DidHardModeDowngrade = false;
            }

            if (Game1.player.currentLocation is MineShaft mine)
            {
                FloorTickCounter++;

                var localChests = mine.get_MineShaftNetChests();
                foreach (NetChest chest in localChests)
                    chest.Spawn(mine);
            }

            int buffId = 88999;
            Buff? buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == buffId);
            if (buff is null)
            {
                Game1.buffsDisplay.addOtherBuff(
                    buff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, speed: 1, 0, 0, minutesDuration: 1, source: "Roguelike", displaySource: I18n.Roguelike_Adrenaline()) { which = buffId }
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

            if (ModEntry.ActiveStats.StartTime is null && MineShaft.activeMines.Count > 0)
            {
                MineShaft? firstMine = null;
                foreach (MineShaft m in MineShaft.activeMines)
                {
                    if (GetLevelFromMineshaft(m) == 1)
                    {
                        firstMine = m;
                        break;
                    }
                }

                if (firstMine is not null)
                {
                    long firstEntryTime = firstMine.get_MineShaftEntryTime().Value;
                    DateTime unix = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    ModEntry.ActiveStats.StartTime = unix.AddSeconds(firstEntryTime);
                }
            }

            if (ModEntry.ActiveStats.DinoKillEndTime is null && e.IsMultipleOf(15))
            {
                bool allPlayersPassed = true;
                foreach (Farmer farmer in Game1.getOnlineFarmers())
                {
                    if (farmer.get_FarmerCurrentLevel().Value < Constants.ScalingOrder[^1])
                    {
                        allPlayersPassed = false;
                        break;
                    }
                }

                if (allPlayersPassed)
                {
                    ModEntry.ActiveStats.DinoKillEndTime = DateTime.UtcNow;
                    StatsMenu.Show();
                }
            }
        }

        public static void ButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || !ModEntry.Config.AutomaticallyFaceMouse || Game1.player.CurrentTool is not MeleeWeapon weapon)
                return;

            bool isDagger = weapon.type.Value == MeleeWeapon.dagger;
            if (SButtonExtensions.IsUseToolButton(e.Button) || (SButtonExtensions.IsActionButton(e.Button) && isDagger))
            {
                var playerPos = Game1.player.GetBoundingBox().Center;

                float mouseX = e.Cursor.AbsolutePixels.X - playerPos.X;
                float mouseY = e.Cursor.AbsolutePixels.Y - playerPos.Y;

                if (Math.Abs(mouseX) > Math.Abs(mouseY))
                {
                    if (mouseX > 0f)
                        Game1.player.FacingDirection = Game1.right;
                    else
                        Game1.player.FacingDirection = Game1.left;
                }
                else
                {
                    if (mouseY > 0f)
                        Game1.player.FacingDirection = Game1.down;
                    else
                        Game1.player.FacingDirection = Game1.up;
                }
            }
        }

        public static void PlayerWarped(object? sender, WarpedEventArgs e)
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

                if (Merchant.IsMerchantFloor(level))
                    Merchant.SetupForLocalPlayer(mine);
            }
        }

        public static void SaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            Game1.options.screenFlash = false;
            Game1.options.zoomButtons = true;
            Game1.player.FishingLevel = 10;

            var loadedOptions = ModEntry.DataHelper.ReadGlobalData<Options>("RoguelikeGameOptions");
            if (loadedOptions is not null)
                Game1.options = loadedOptions;

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

            qi.showTextAboveHead(I18n.Lobby_QiIntroduction(), duration: 15000);

            if (Context.IsMainPlayer)
            {
                HardMode = false;
                ModEntry.ActiveStats.HardMode = false;

                qi.setTileLocation(new(17, 6));
                qi.faceDirection(2);
                mine.addCharacter(qi);

                Vector2 computerTile = new(19, 7);
                mine.Objects[computerTile] = new SObject(computerTile, 239);

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

        /// <summary>
        /// Event handler for when the player returns to title.
        /// Used for multiplayer, so players can't keep going up by
        /// disconnecting and reconnecting.
        /// </summary>
        /// <param name="sender">always null in SMAPI</param>
        /// <param name="e">The event's arguments</param>
        public static void ReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
        {
            if (CurrentLevel != 0)
                CurrentLevel--;
        }

        /// <summary>
        /// Event handler for when the in-game time changes.
        /// After 11pm, reset the time to 6am so the player never sleeps
        /// </summary>
        /// <param name="sender">always null in SMAPI</param>
        /// <param name="e">The event's arguments</param>
        public static void TimeChanged(object? sender, TimeChangedEventArgs e)
        {
            if (e.NewTime >= 2300)
                Game1.timeOfDay = 600;
        }

        /// <summary>
        /// Method called when the player goes down a ladder.
        /// Performs some resets and increments the level counter
        /// </summary>
        public static void NextFloor()
        {
            FloorTickCounter = 0;
            if (CurrentLevel == 0)
            {
                ModEntry.ActiveStats.Reset();

                FloorRng = new(FloorRngSeed);
                SeenMineMaps.Clear();
                ChallengeFloor.History.Clear();
                MineShaft.clearActiveMines();
            }

            Game1.player.get_FarmerWasDamagedOnThisLevel().Value = false;

            CurrentLevel++;
            ModEntry.ActiveStats.FloorsDescended = GetHighestMineShaftLevel();
            Merchant.CurrentShop = null;
            Perks.CurrentMenu = null;
            ForgeFloor.CurrentForge = null;

            if (Constants.FloorsIncreaseGoldMax.Contains(CurrentLevel))
                GoldDropMax++;
            else if (Constants.FloorsIncreaseGoldMin.Contains(CurrentLevel))
                GoldDropMin++;

            if (Context.IsMainPlayer)
                ClearInactiveMines();
        }

        /// <summary>
        /// Removes mines from memory if they are no longer needed
        /// </summary>
        /// <remarks>
        /// This method is a bandaid until SMAPI merges my pull request
        /// </remarks>
        public static void ClearInactiveMines()
        {
            int instancesToKeep = 10;

            if (MineShaft.activeMines.Count < instancesToKeep)
                return;

            if (!Context.IsMultiplayer)
            {
                LocalizedContentManager? mapContent = (LocalizedContentManager?)MineShaft.activeMines[0].GetType().GetField("mapContent", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(MineShaft.activeMines[0]);
                mapContent?.Dispose();
                MineShaft.activeMines.RemoveAt(0);
                return;
            }

            int playersAccountedFor = 0;
            int playersToAccountFor = Game1.getOnlineFarmers().Count;
            int floorsSinceLastPlayer = 0;
            int amountToRemove = 0;
            int merchantInterval = Constants.ScalingOrder[1] - Constants.ScalingOrder[0];

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
                LocalizedContentManager? mapContent = (LocalizedContentManager?)MineShaft.activeMines[0].GetType().GetField("mapContent", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(MineShaft.activeMines[0]);
                mapContent?.Dispose();
                MineShaft.activeMines.RemoveAt(0);
            }
        }

        public static string GetRandomTrack(List<string> tracks)
        {
            return tracks[Game1.random.Next(tracks.Count)];
        }

        /// <summary>
        /// Retrieves the song artist and song name for a given track.
        /// </summary>
        /// <param name="track">The technical name of the music track</param>
        /// <returns></returns>
        public static string GetMusicCredits(string track)
        {
            return track switch
            {
                "gelus_defensor" => "Gelus Defensor - Therm",
                "photophobia" => "Photophobia - Therm",
                "jelly_junktion" => "Jelly Junktion - Therm",
                "hold_your_ground" => "Hold Your Ground - Therm",
                "ceaseless_and_incessant" => "Ceaseless and Incessant - Therm",
                "circus_freak" => "Circus Freak - Therm",
                "invoke_the_ancient" => "Invoke the Ancient - Therm",
                "bee_boss" => "Bee Boss - ConcernedApe",
                _ => ""
            };
        }

        /// <summary>
        /// Retrieves the depth (monsters to spawn, music, etc) for a given Roguelike level.
        /// </summary>
        /// <param name="mine">The mine to retrieve depth for</param>
        /// <param name="floor">The roguelike level</param>
        /// <returns></returns>
        public static int GetFloorDepth(MineShaft mine, int floor)
        {
            int level = GetLevelFromMineshaft(mine);

            if (BossFloor.IsBossFloor(level))
                return floor;

            level %= Constants.ScalingOrder[^1];

            int result;
            if (level < Constants.ScalingOrder[0])
                result = 20;
            else if (level < Constants.ScalingOrder[1])
                result = 60;
            else if (level < Constants.ScalingOrder[2])
                result = 100;
            else if (level < Constants.ScalingOrder[3])
                result = 121;
            else if (level < Constants.ScalingOrder[4])
                result = 20;
            else if (level < Constants.ScalingOrder[5])
                result = 60;
            else if (level < Constants.ScalingOrder[6])
                result = 100;
            else if (level < Constants.ScalingOrder[7])
                result = 179;
            else
                result = 179;

            if (mine.get_MineShaftIsDarkArea())
                result += 10;

            return result;
        }

        /// <summary>
        /// Retrieves the map to load for a specific MineShaft. Optionally avoiding
        /// picking maps that contain water due to issues with dark floors not having
        /// water textures.
        /// </summary>
        /// <param name="mine">The mine to retrieve the map for</param>
        /// <param name="avoidWater">Whether or not to avoid picking maps with water</param>
        /// <returns></returns>
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

            var validOptions = Constants.ValidMineMaps;

            if (avoidWater)
                validOptions.RemoveAll(Constants.MapsWithWater.Contains);

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

        /// <summary>
        /// Adds the default starting items to the player's inventory.
        /// </summary>
        public static void AddDefaultItemsToInventory()
        {
            Pickaxe pick = new()
            {
                UpgradeLevel = 4
            };
            Game1.player.addItemToInventory(pick);                // Copper Pickaxe
            Game1.player.addItemToInventory(new MeleeWeapon(0));  // Rusty Sword

            int eggStack = HardMode ? 2 : 3;
            Game1.player.addItemToInventory(new SObject(194, eggStack));  // Fried Egg
        }

        /// <summary>
        /// Handles death when it occurs. Respective functions will be called
        /// depending on if the game is singleplayer or multiplayer.
        /// </summary>
        public static void HandleDeath()
        {
            Game1.killScreen = false;
            Game1.player.exhausted.Value = false;

            if (Context.IsMultiplayer)
                HandleMultiplayerDeath();
            else
                HandleLocalDeath();
        }

        /// <summary>
        /// Handles death when the game is in singleplayer.
        /// </summary>
        public static void HandleLocalDeath()
        {
            ModEntry.Invincible = true;
            Game1.player.temporarilyInvincible = true;
            Game1.player.maxHealth = StartingHP;
            Game1.player.health = StartingHP;
            Game1.player.Stamina = Game1.player.MaxStamina;

            DelayedAction.functionAfterDelay(GameOver, 7000);
        }

        /// <summary>
        /// Handles death when the game is in multiplayer.
        ///
        /// Puts the player in a "ghost" state where they can still move around.
        /// If all players are dead, the game ends.
        /// </summary>
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
                    Game1.showGlobalMessage(I18n.Roguelike_WillBeRevived());
                }
            }, 7000);

        }

        /// <summary>
        /// Performs the game end mechanics.
        ///
        /// Shows a game over message and resets the game for the next play.
        /// </summary>
        public static void GameOver()
        {
            if (Game1.player.get_FarmerIsSpectating().Value)
                SpectatorMode.ExitSpectatorMode();

            Game1.showGlobalMessage(I18n.Roguelike_YouSurvived(floors: ModEntry.ActiveStats.FloorsDescended));

            WarpLocalPlayerToStart();
            Game1.player.temporarilyInvincible = false;
            CurrentLevel = 0;
            Game1.screenGlow = false;
            ModEntry.ActiveStats.Multiplayer = Context.IsMultiplayer;
            ModEntry.ActiveStats.PlayerCount = Game1.getOnlineFarmers().Count;
            ModEntry.ActiveStats.EndTime = DateTime.UtcNow;
            ModEntry.ActiveStats.Patch = ModEntry.CurrentVersion;
            ModEntry.ActiveStats.Seed = FloorRngSeed.ToString();

            ModPersistentData? globalData = ModEntry.Instance.Helper.Data.ReadGlobalData<ModPersistentData>("roguelike-persistent-data");
            globalData ??= new ModPersistentData();

            if (ModEntry.ActiveStats.DinoKillEndTime is not null)
                globalData.UnlockedBossArena = true;
            globalData.RunHistory.Add(ModEntry.ActiveStats);

            ModEntry.Instance.Helper.Data.WriteGlobalData("roguelike-persistent-data", globalData);

            ResetLocalGameState();
            ResetLocalPlayer();

            StatsMenu.Show();
        }

        /// <summary>
        /// Resets the game's state for a new play.
        /// </summary>
        public static void ResetLocalGameState()
        {
            CurrentLevel = 0;
            GoldDropMax = 2;
            GoldDropMin = 1;

            Game1.flushLocationLookup();
            MineShaft.clearActiveMines();

            if (Context.IsMainPlayer && RerollRandomEveryRun)
                FloorRngSeed = Guid.NewGuid().GetHashCode();
        }

        /// <summary>
        /// Resets the player's state for a new play.
        /// </summary>
        public static void ResetLocalPlayer()
        {
            Curse.RemoveAllCurses();
            Perks.RemoveAllPerks();

            Game1.player.set_FarmerActiveHatQuest(null);

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

            Game1.player.resilience = 0;

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

            Game1.player.MaxItems = 12;
            while (Game1.player.Items.Count > Game1.player.MaxItems)
                Game1.player.Items.RemoveAt(Game1.player.Items.Count - 1);

            AddDefaultItemsToInventory();
        }

        /// <summary>
        /// Warps the local player to the start of the game.
        /// </summary>
        public static void WarpLocalPlayerToStart()
        {
            Game1.warpFarmer("Mine", 17, 15, 2);
        }

        /// <summary>
        /// Event handler for when the game's menu changes.
        ///
        /// This handler refreshes the dialogue for Mister Qi.
        /// </summary>
        /// <param name="e">The event's arguments</param>
        public static void MenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if (e.OldMenu is not DialogueBox dialogue || (dialogue.characterDialogue is not null && dialogue.characterDialogue.speaker is null))
                return;

            if (Game1.currentLocation is MineShaft mine && Merchant.IsMerchantFloor(mine))
                Merchant.PopulateQiDialogue(mine);
            else
                PopulateQiDialogue(Game1.currentLocation);
        }

        /// <summary>
        /// Populate Mister Qi's dialogue.
        /// </summary>
        /// <param name="lobby">The map Mister Qi is in</param>
        public static void PopulateQiDialogue(GameLocation lobby)
        {
            NPC qi = lobby.getCharacterFromName("Mister Qi");
            if (qi is null)
                return;

            qi.CurrentDialogue.Clear();

            string perkKey = string.Join('+', Game1.options.journalButton);
            qi.setNewDialogue(I18n.Lobby_QiDialogue(perkKey: perkKey));
        }

        /// <summary>
        /// Event handler for when the player attempts to perform an action (right click)
        /// </summary>
        /// <param name="location">The location of the attempt</param>
        /// <param name="action">The attempted action</param>
        /// <param name="who">Which player attempted</param>
        /// <param name="tileLocation">The tile location of the attempt</param>
        /// <returns>If the action was a success</returns>
        public static bool PerformAction(GameLocation location, string action, Farmer who, Location tileLocation)
        {
            if (action == "HardMode" && Context.IsMainPlayer)
            {
                var responses = location.createYesNoResponses();
                if (HardMode)
                    location.createQuestionDialogue(I18n.Lobby_ExitHardMode(), responses, "hardMode");
                else
                    location.createQuestionDialogue(I18n.Lobby_EnterHardMode(), responses, "hardMode");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Event handler for when the players answers a dialogue pop-up.
        /// </summary>
        /// <param name="mine">The location of the player</param>
        /// <param name="questionAndAnswer">The question that was answered</param>
        /// <param name="questionParams">Optional parameters for the question</param>
        /// <returns>If the answer was a success</returns>
        public static bool AnswerDialogueAction(GameLocation mine, string questionAndAnswer, string[] questionParams)
        {
            if (questionAndAnswer != "hardMode_Yes")
                return false;

            if (HardMode)
            {
                HardMode = false;
                Game1.drawObjectDialogue(I18n.Lobby_DidExitHardMode());
            }
            else
            {
                HardMode = true;
                Game1.drawObjectDialogue(I18n.Lobby_DidEnterHardMode());
            }

            ModEntry.ActiveStats.Reset();
            ModEntry.ActiveStats.HardMode = HardMode;

            return true;
        }

        /// <summary>
        /// Finds the highest roguelike level achieved in the current run
        /// </summary>
        /// <returns>The highest level reached</returns>
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

        /// <summary>
        /// Finds the lowest roguelike level achieved in the current run
        /// </summary>
        /// <remarks>
        /// The lowest level stored in memory
        /// </remarks>
        /// <returns>The lowest level reached</returns>
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

        /// <summary>
        /// Method called when a user successfully fishes on a level.
        /// Determines the result of the fishing.
        /// </summary>
        /// <param name="mine">Where the fishing happened</param>
        /// <param name="who">Who did the fishing</param>
        /// <returns>An item to give the player</returns>
        public static SObject? GetFish(MineShaft mine, Farmer who)
        {
            double roll = Game1.random.NextDouble();
            double qualityRoll = Game1.random.NextDouble();

            int itemId;
            int quality;
            if (roll <= 0.4 && !HatQuest.HasBuffFor(HatQuestType.FISHING_HAT))
            {
                // trash
                itemId = Game1.random.Next(167, 173);
                quality = 0;
            }
            else if (roll <= 0.65)
            {
                // fish
                itemId = MerchantFloor.PickNFromList(Constants.PossibleFish, 1).First();

                if (qualityRoll <= 0.05)
                    quality = 3;
                else if (qualityRoll <= 0.35)
                    quality = 2;
                else if (qualityRoll <= 0.65)
                    quality = 1;
                else
                    quality = 0;

                if (Game1.player.get_FarmerActiveHatQuest() is not null)
                    Game1.player.get_FarmerActiveHatQuest()!.FishCaught++;
            }
            else if (roll <= 0.9)
            {
                int toSpawn = Game1.random.Next(1, 3);
                if (Curse.AnyFarmerHasCurse(CurseType.MoreEnemiesLessHealth))
                    toSpawn++;

                mine.SpawnMonsters(toSpawn);
                Game1.chatBox.addMessage("Monsters emerge from the depths...", Color.Gold);

                return null;
            }
            else
            {
                // gems
                if (qualityRoll <= 0.05)
                    itemId = 74;
                else if (qualityRoll <= 0.25)
                    itemId = 72;
                else if (qualityRoll <= 0.55)
                    itemId = 64;
                else if (qualityRoll <= 0.85)
                    itemId = 60;
                else
                    itemId = 68;

                quality = 0;
            }


            return new SObject(itemId, 1, quality: quality);
        }

        /// <summary>
        /// Method that determines the drops of a broken barrel/crate
        /// </summary>
        /// <param name="mine">Where the barrel/crate was broken</param>
        /// <returns>Tuple of (item id, quantity) for barrel drops</returns>
        public static (int, int) GetBarrelDrops(MineShaft mine)
        {
            int itemId;
            int quantity = 1;

            double roll = Game1.random.NextDouble();

            if (Perks.HasPerk(Perks.PerkType.BarrelEnthusiast))
                roll = Math.Min(Game1.random.NextDouble(), roll);

            if (HatQuest.HasBuffFor(HatQuestType.GARBAGE_HAT))
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
            else if (roll <= 0.14)
                itemId = 1000;
            else if (roll <= 0.17)
                itemId = 66;
            else if (roll <= 0.25)
                itemId = 78;
            else if (roll <= 0.85)
            {
                itemId = 384;
                quantity = Game1.random.Next(GoldDropMin, GoldDropMax + 1);
            }
            else
            {
                itemId = 0;
                quantity = 0;
            }

            // 15% nothing
            // 60% gold
            // 8% cave carrot 1-2
            // 7% fishing rod
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
