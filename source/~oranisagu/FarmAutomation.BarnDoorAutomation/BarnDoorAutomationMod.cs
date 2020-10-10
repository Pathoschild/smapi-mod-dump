/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/oranisagu/SDV-FarmAutomation
**
*************************************************/

using FarmAutomation.Common;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;

namespace FarmAutomation.BarnDoorAutomation
{
    /// <summary>
    /// a mod which opens and closes barn doors for animals based on game time, season and weather.
    /// </summary>
    public class BarnDoorAutomationMod : Mod
    {
        private bool _gameLoaded;

        private readonly BarnDoorAutomationConfiguration _config;

        /// <summary>
        /// if true, opening doors will be skipped until the next day. this is used to keep cpu time as low as possible
        /// </summary>
        bool IgnoreOpeningToday { get; set; }

        /// <summary>
        /// if true, closing doors will be skipped until the next day. this is used to keep cpu time as low as possible
        /// </summary>
        bool IgnoreClosingToday { get; set; }

        /// <summary>
        /// default constructor
        /// </summary>
        public BarnDoorAutomationMod()
        {
            Log.Info($"Initalizing {nameof(BarnDoorAutomationMod)}");
            _config = ConfigurationBase.LoadConfiguration<BarnDoorAutomationConfiguration>();
        }

        /// <summary>
        /// entry point for mods. events are subscribed here
        /// </summary>
        /// <param name="objects"></param>
        public override void Entry(params object[] objects)
        {
            base.Entry(objects);
            GameEvents.GameLoaded += (s, e) => { _gameLoaded = true; };
            TimeEvents.DayOfMonthChanged += (s, e) => { ResetDoorStatesOnNewDay(); };
            TimeEvents.TimeOfDayChanged += (s, e) => { if (_gameLoaded && _config.EnableMod) ProcessIfTimeAndWeatherFits(); };
        }

        /// <summary>
        /// reset the helper variables which skip processing
        /// </summary>
        private void ResetDoorStatesOnNewDay()
        {
            IgnoreOpeningToday = false;
            IgnoreClosingToday = false;
        }

        /// <summary>
        /// starts the door processing when all circumstances are as expected
        /// </summary>
        private void ProcessIfTimeAndWeatherFits()
        {
            if (!Game1.hasLoadedGame)
            {
                return;
            }
            // ignore days when the doors should stay closed
            if (!IgnoreOpeningToday && WillAnimalsStayInside())
            {
                Log.Debug("It's either winter or unpleasant weather today, the animals will stay inside. Animal doors won't be opened.");
                IgnoreOpeningToday = true;
            }

            if (!IgnoreClosingToday && Game1.timeOfDay >= _config.CloseDoorsAfter)
            {
                SetAllDoors(DoorState.Closed);
                IgnoreClosingToday = true;
            }
            else if (!IgnoreOpeningToday && Game1.timeOfDay >= _config.OpenDoorsAfter)
            {
                if (SkipSpringDay())
                {
                    IgnoreOpeningToday = true;
                    Log.Debug($"Skipping door opening for first {_config.FirstDayInSpringToOpen} days in spring");
                    return;
                }
                SetAllDoors(DoorState.Open);
                IgnoreOpeningToday = true;
            }

        }

        /// <summary>
        /// the configuration allows you to wait a few days in spring for your grass starters to kick in before letting your animals out
        /// </summary>
        /// <returns>whether opening the doors should be skipped on the current day</returns>
        private bool SkipSpringDay()
        {
            return Game1.IsSpring && Game1.dayOfMonth < _config.FirstDayInSpringToOpen;
        }

        /// <summary>
        /// check if the animals will stay inside
        /// </summary>
        /// <returns>true if the animals won't leave their buildings today</returns>
        private static bool WillAnimalsStayInside()
        {
            return Game1.IsWinter | Game1.isRaining | Game1.isLightning;
        }

        /// <summary>
        /// iterate over all buildings and set their doors to the supplied state
        /// </summary>
        /// <param name="shouldBeOpen">true if the doors should be open, false if they should be closed</param>
        private void SetAllDoors(DoorState shouldBeOpen)
        {
            var farm = Game1.getFarm();

            foreach (var building in farm.buildings)
            {
                // skip buildings still in construction
                if (building.daysOfConstructionLeft > 0)
                {
                    Log.Debug($"Skipping {building.buildingType} because it's in construction");
                    return;
                }

                // in order to support future (modded) buildings, check if the animal door exists instead of simply checking if its a barn or coop
                if (building.animalDoor.X > 0 && building.animalDoor.Y > 0)
                {
                    SetDoor(building, shouldBeOpen);
                }
            }
        }

        /// <summary>
        /// open or close the door on a building
        /// </summary>
        /// <param name="building">the building with an animal door</param>
        /// <param name="desiredDoorState">true if the door should be opened, false if it should be closed</param>
        private void SetDoor(Building building, DoorState desiredDoorState)
        {
            if (!IsDoorInDesiredState(building, desiredDoorState))
            {
                var vector = new Vector2(
                    building.animalDoor.X + building.tileX,
                    building.animalDoor.Y + building.tileY
                );
                Log.Debug($"Setting door to {desiredDoorState} for building {building.buildingType}");
                building.doAction(vector, Game1.player);
            }
        }

        /// <summary>
        /// check if the animal door is in the desired state
        /// </summary>
        /// <param name="building">the building on which the door is located</param>
        /// <param name="state">the desired state for the door</param>
        /// <returns></returns>
        private bool IsDoorInDesiredState(Building building, DoorState state)
        {
            return state == DoorState.Open ? building.animalDoorOpen : !building.animalDoorOpen;
        }

    }
}
