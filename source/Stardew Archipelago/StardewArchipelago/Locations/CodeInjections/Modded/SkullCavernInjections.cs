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
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace StardewArchipelago.Locations.CodeInjections.Modded
{
    public static class SkullCavernInjections
    {
        public const string SKULL_CAVERN_ELEVATOR_ITEM = "Progressive Skull Cavern Elevator";
        public const string SKULL_CAVERN_FLOOR_LOCATION = "Skull Cavern: Floor {0}";
        public const string SKULL_KEY = "Skull Key";

        private const int ELEVATOR_STEP = 25;
        private const int DIFFICULTY = 1;


        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static int _realDeepestMineLevel = -1;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        // public MyElevatorMenu(int elevatorStep, double difficulty, int elevatorCostPerStep)
        public static bool MyElevatorMenuConstructor_SkullCavernElevator_Prefix(MineElevatorMenu __instance, ref int elevatorStep, ref double difficulty, ref int elevatorCostPerStep)
        {
            try
            {
                if (!_archipelago.HasReceivedItem(SKULL_KEY))
                {
                    return true; // Don't bother updating anything until then.
                }
                var receivedElevators = _archipelago.GetReceivedItemCount(SKULL_CAVERN_ELEVATOR_ITEM);
                elevatorStep = ELEVATOR_STEP;
                difficulty = DIFFICULTY;

                if (_realDeepestMineLevel == -1)
                {
                    _realDeepestMineLevel = Game1.player.deepestMineLevel;
                }

                if (receivedElevators >= 8 && Game1.player.deepestMineLevel >= 320)
                {
                    return true; //let the player gain these floors on their own since they've "collected" the floors already
                }

                var elevatorMaxLevel = (receivedElevators * ELEVATOR_STEP) + 120;
                Game1.player.deepestMineLevel = elevatorMaxLevel;

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(MyElevatorMenuConstructor_SkullCavernElevator_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public MyElevatorMenu(int elevatorStep, double difficulty, int elevatorCostPerStep)
        public static void MyElevatorMenuConstructor_SkullCavernElevator_Postfix(MineElevatorMenu __instance, int elevatorStep, double difficulty, int elevatorCostPerStep)
        {
            try
            {
                if (_realDeepestMineLevel > -1)
                {
                    Game1.player.deepestMineLevel = _realDeepestMineLevel;
                    _realDeepestMineLevel = -1;
                }

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(MyElevatorMenuConstructor_SkullCavernElevator_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public static void enterMine(int whatLevel)
        public static void EnterMine_SendSkullCavernElevatorCheck_PostFix(int whatLevel)
        {
            try
            {
                var realSkullCavernLevel = whatLevel - 120;
                if (whatLevel < 121 || realSkullCavernLevel % ELEVATOR_STEP != 0)
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

                var locationToCheck = string.Format(SKULL_CAVERN_FLOOR_LOCATION, realSkullCavernLevel);
                _locationChecker.AddCheckedLocation(locationToCheck);
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(EnterMine_SendSkullCavernElevatorCheck_PostFix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
    }
}
