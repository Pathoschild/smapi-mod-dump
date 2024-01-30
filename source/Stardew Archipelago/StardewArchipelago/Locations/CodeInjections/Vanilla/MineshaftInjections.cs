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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Items.Unlocks;
using StardewArchipelago.Textures;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public static class MineshaftInjections
    {
        private const string TREASURE_LOCATION = "The Mines Floor {0} Treasure";
        private const string ELEVATOR_LOCATION = "Floor {0} Elevator";

        private static IMonitor _monitor;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static Texture2D _miniArchipelagoIcon;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _archipelago = archipelago;
            _locationChecker = locationChecker;

            var desiredTextureName = ArchipelagoTextures.COLOR;
            _miniArchipelagoIcon = ArchipelagoTextures.GetColoredLogo(modHelper, 12, desiredTextureName);
        }

        public static bool CheckForAction_MineshaftChest_Prefix(Chest __instance, Farmer who, bool justCheckingForActivity, ref bool __result)
        {
            try
            {
                if (justCheckingForActivity || __instance.giftbox.Value || __instance.playerChest.Value || Game1.mine == null || Game1.mine.mineLevel > 120)
                {
                    return true; // run original logic
                }

                if (__instance.items.Count <= 0)
                {
                    return true; // run original logic
                }

                who.currentLocation.playSound("openChest");
                if (__instance.synchronized.Value)
                    __instance.GetMutex().RequestLock(() => __instance.openChestEvent.Fire());
                else
                    __instance.performOpenChest();

                Game1.mine.chestConsumed();
                var obj = __instance.items[0];
                __instance.items[0] = null;
                __instance.items.RemoveAt(0);
                __result = true;
                
                _locationChecker.AddCheckedLocation(string.Format(TREASURE_LOCATION, Game1.mine.mineLevel));

                return false; // don't run original logic

            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckForAction_MineshaftChest_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static bool AddLevelChests_Level120_Prefix(MineShaft __instance)
        {
            try
            {
                if (__instance.mineLevel != 120 || Game1.player.chestConsumedMineLevels.ContainsKey(120))
                {
                    return true; // run original logic
                }

                Game1.player.completeQuest(18);
                Game1.getSteamAchievement("Achievement_TheBottom");
                var chestPosition = new Vector2(9f, 9f);
                var items = new List<Item>();
                items.Add(new MeleeWeapon(8));
                __instance.overlayObjects[chestPosition] = new Chest(0, items, chestPosition)
                {
                    Tint = Color.Pink,
                };

                return false; // don't run original logic

            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AddLevelChests_Level120_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public static void enterMine(int whatLevel)
        public static void EnterMine_SendElevatorCheck_PostFix(int whatLevel)
        {
            try
            {
                if (whatLevel < 5 || whatLevel > 120 || whatLevel % 5 != 0)
                {
                    return;
                }

                var progression = _archipelago.SlotData.ElevatorProgression;
                var currentMineshaft = Game1.player.currentLocation as MineShaft;
                var currentMineLevel = currentMineshaft?.mineLevel ?? 0;
                if (progression == ElevatorProgression.ProgressiveFromPreviousFloor && currentMineLevel != whatLevel - 1)
                {
                    return;
                }

                _locationChecker.AddCheckedLocation(string.Format(ELEVATOR_LOCATION, whatLevel));
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(EnterMine_SendElevatorCheck_PostFix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        public static bool PerformAction_LoadElevatorMenu_Prefix(GameLocation __instance, string action, Farmer who,
            Location tileLocation, ref bool __result)
        {
            try
            {
                if (action == null || !who.IsLocalPlayer || action.Split(' ')[0] != "MineElevator")
                {
                    return true; // run original logic
                }

                CreateElevatorMenuIfUnlocked();
                __result = true;
                return false; // don't run original logic

            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(PerformAction_LoadElevatorMenu_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static bool CheckAction_LoadElevatorMenu_Prefix(MineShaft __instance, Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who, ref bool __result)
        {
            try
            {
                var tile = __instance.map.GetLayer("Buildings").PickTile(new Location(tileLocation.X * 64, tileLocation.Y * 64), viewport.Size);

                if (tile == null || !who.IsLocalPlayer || tile.TileIndex != 112 || __instance.mineLevel > 120)
                {
                    return true; // run original logic
                }

                CreateElevatorMenuIfUnlocked();
                __result = true;
                return false; // don't run original logic

            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckAction_LoadElevatorMenu_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void CreateElevatorMenuIfUnlocked()
        {
            var numberOfMineElevatorReceived = _archipelago.GetReceivedItemCount(VanillaUnlockManager.PROGRESSIVE_MINE_ELEVATOR);
            var mineLevelUnlocked = numberOfMineElevatorReceived * 5;
            mineLevelUnlocked = Math.Min(120, Math.Max(0, mineLevelUnlocked));

            if (mineLevelUnlocked < 5)
            {
                Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Mines_MineElevator_NotWorking")));
            }
            else
            {
                var previousMaxLevel = MineShaft.lowestLevelReached;
                MineShaft.lowestLevelReached = mineLevelUnlocked;
                Game1.activeClickableMenu = new MineElevatorMenu();
                MineShaft.lowestLevelReached = previousMaxLevel;
            }
        }

        // public override void draw(SpriteBatch b)
        public static void Draw_AddArchipelagoIndicators_Postfix(MineElevatorMenu __instance, SpriteBatch b)
        {
            try
            {
                foreach (var button in __instance.elevators)
                {
                    var floor = Convert.ToInt32(button.name);
                    var elevatorLocation = string.Format(ELEVATOR_LOCATION, floor);
                    var treasureLocation = string.Format(TREASURE_LOCATION, floor);
                    var checkRemainingOnThatFloor = _locationChecker.IsLocationMissing(elevatorLocation) || _locationChecker.IsLocationMissing(treasureLocation);

                    if (!checkRemainingOnThatFloor)
                    {
                        continue;
                    }

                    var buttonLocation = new Vector2(button.bounds.X, button.bounds.Y);
                    var position = buttonLocation + new Vector2(12f, 12f);
                    var sourceRectangle = new Rectangle(0, 0, 12, 12);
                    var color = Color.White;
                    var origin = new Vector2(8f, 8f);
                    b.Draw(_miniArchipelagoIcon, position, sourceRectangle, color, 0.0f, origin, 1f, SpriteEffects.None, 0.86f);
                }
                
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Draw_AddArchipelagoIndicators_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
    }
}
