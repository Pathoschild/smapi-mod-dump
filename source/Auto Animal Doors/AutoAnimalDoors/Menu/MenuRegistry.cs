/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/taggartaa/AutoAnimalDoors
**
*************************************************/

using AutoAnimalDoors.Config;
using AutoAnimalDoors.StardewValleyWrapper;
using GenericModConfigMenu;
using StardewModdingAPI;
using System;
using System.Collections.Generic;

namespace AutoAnimalDoors.Menu
{
    public class MenuRegistry
    {
        private static readonly int MIN_TIMEPICKER_HOUR = 6; // 6am when you get up
        private static readonly int MAX_TIMEPICKER_HOUR = 26; // 2am the next day when you pass out
        private static readonly int HOUR_DIVISOR = 100;
        private static readonly int MINUTE_INCREMENT = 10;
        private static readonly int MIN_TIMEPICKER_VALUE = 0;

        private static int MINUTE_INCREMENTS_PER_HOUR
        {
            get { return 60 / MINUTE_INCREMENT; }
        }
        private static int MAX_TIMEPICKER_VALUE
        {
            get { return MIN_TIMEPICKER_VALUE + (MAX_TIMEPICKER_HOUR - MIN_TIMEPICKER_HOUR) * MINUTE_INCREMENTS_PER_HOUR; }
        }

        public event EventHandler<bool> AutoOpenedEnabledChanged;

        private readonly SortedList<int, String> animalBuildingLevelOptions = new()
        {
            { 1, "Normal" },
            { 2, "Big" },
            { 3, "Deluxe" },
            { int.MaxValue, "Disabled" }
        };

        private string[] AnimalBuildingLevelNames
        {
            get
            {
                string[] names = new string[animalBuildingLevelOptions.Count];
                animalBuildingLevelOptions.Values.CopyTo(names, 0);
                return names;
            }
        }

        private IModHelper Helper { get; set; }

        public MenuRegistry(IModHelper helper)
        {
            Helper = helper;
        }

        public void InitializeMenu(IManifest manifest, ModConfig config)
        {
            IGenericModConfigMenuApi api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api != null)
            {
                Logger.Instance.Log("Generic Mod Config detected, initializing menu");

                api.Register(manifest, () => config = new ModConfig(), () => Helper.WriteConfig<ModConfig>(config));

                api.AddSectionTitle(manifest, () => "Auto Animal Doors");
                api.AddBoolOption(mod: manifest,
                    name: () => "Auto Open Enabled",
                    getValue: () => config.AutoOpenEnabled,
                    setValue: (bool autoOpenEnabled) =>
                    {
                        config.AutoOpenEnabled = autoOpenEnabled;
                        AutoOpenedEnabledChanged?.Invoke(this, autoOpenEnabled);
                    });

                api.AddNumberOption(mod: manifest,
                    name: () =>
                    {
                        string alreadyOpened = ModEntry.HasDoorsOpenedToday ? "(Already Opened Today)" : "";
                        return "Open Time " + alreadyOpened;
                    },
                    tooltip: () => "The time animal doors are scheduled to open.",
                    getValue: () => GetTimePickerValueFromTime(config.AnimalDoorOpenTime),
                    setValue: (int newTime) => config.AnimalDoorOpenTime = GetTimeFromTimePickerValue(newTime),
                    min: MIN_TIMEPICKER_VALUE,
                    max: MAX_TIMEPICKER_VALUE,
                    formatValue: (value) => GetTimePickerStringFromTimePickerValue(value));


                api.AddNumberOption(mod: manifest,
                    name: () =>
                    {
                        string alreadyClosed = ModEntry.HasDoorsClosedToday ? "(Already Closed Today)" : "";
                        return "Close Time " + alreadyClosed;
                    },
                    tooltip: () => "The time animal doors are scheduled to close.",
                    getValue: () => GetTimePickerValueFromTime(config.AnimalDoorCloseTime),
                    setValue: (int newTime) => config.AnimalDoorCloseTime = GetTimeFromTimePickerValue(newTime),
                    min: MIN_TIMEPICKER_VALUE,
                    max: MAX_TIMEPICKER_VALUE,
                    formatValue: (value) => GetTimePickerStringFromTimePickerValue(value));

                api.AddBoolOption(mod: manifest,
                    name: () => "Other Mods Enabled",
                    tooltip: () => "Enables or disables the auto opening of animal doors from other mods (I can't control door sounds/animation or test every mod).",
                    getValue: () => config.UnrecognizedAnimalBuildingsEnabled,
                    setValue: (bool unrecognizedAnimalBulidingsEnabled) => config.UnrecognizedAnimalBuildingsEnabled = unrecognizedAnimalBulidingsEnabled);

                api.AddTextOption(mod: manifest,
                    name: () => "Coop Required Upgrade Level",
                    tooltip: () => "The coop upgrade level required for auto open/close.",
                    getValue: () => GetAnimalBuildingUpgradeLevelName(config.CoopRequiredUpgradeLevel),
                    setValue: (string newLevel) => config.CoopRequiredUpgradeLevel = GetAnimalBuildingUpgradeLevel(newLevel),
                    allowedValues: AnimalBuildingLevelNames);

                api.AddTextOption(mod: manifest,
                    name: () => "Barn Required Upgrade Level",
                    tooltip: () => "The barn upgrade level required for auto open/close.",
                    getValue: () => GetAnimalBuildingUpgradeLevelName(config.BarnRequiredUpgradeLevel),
                    setValue: (string newLevel) => config.BarnRequiredUpgradeLevel = GetAnimalBuildingUpgradeLevel(newLevel),
                    allowedValues: AnimalBuildingLevelNames);

                api.AddTextOption(mod: manifest,
                    name: () => "Door Sound Setting",
                    tooltip: () => "When to play/not play the door sound when doors are opened and closed.",
                    getValue: () => config.DoorSoundSetting.Name(),
                    setValue: (string doorSoundSettingName) => config.DoorSoundSetting = DoorSoundSettingUtils.FromName(doorSoundSettingName),
                    allowedValues: DoorSoundSettingUtils.Names);


                api.AddBoolOption(mod: manifest,
                    name: () => "Open Doors When Raining",
                    tooltip: () => "Enables or disables opening doors when raining/lightning.",
                    getValue: () => config.OpenDoorsWhenRaining,
                    setValue: (bool autoOpenEnabled) => config.OpenDoorsWhenRaining = autoOpenEnabled);

                api.AddBoolOption(mod: manifest,
                    name: () => "Open Doors During Winter",
                    tooltip: () => "Enables or disables opening doors during winter.",
                    getValue: () => config.OpenDoorsDuringWinter,
                    setValue: (bool autoOpenEnabled) => config.OpenDoorsDuringWinter = autoOpenEnabled);
            }
        }

        private static int GetTimePickerValueFromTime(int time)
        {
            int hour = (time / HOUR_DIVISOR) - MIN_TIMEPICKER_HOUR;
            int min = (time % HOUR_DIVISOR) / MINUTE_INCREMENT;
            return hour * MINUTE_INCREMENTS_PER_HOUR + min;
        }

        private static int GetTimeFromTimePickerValue(int timePickerValue)
        {
            int hour = (timePickerValue / MINUTE_INCREMENTS_PER_HOUR) + MIN_TIMEPICKER_HOUR;
            int min = timePickerValue % MINUTE_INCREMENTS_PER_HOUR; 
            return hour * HOUR_DIVISOR + min * MINUTE_INCREMENT;
        }

        private string GetAnimalBuildingUpgradeLevelName(int level)
        {
            if (animalBuildingLevelOptions.TryGetValue(level, out string upgradeLevelName))
            {
                return upgradeLevelName;
            }

            return animalBuildingLevelOptions.Values[0];
        }

        private int GetAnimalBuildingUpgradeLevel(string name)
        {
            int index = animalBuildingLevelOptions.IndexOfValue(name);
            if (index < 0)
            {
                index = 0;
            }
            return animalBuildingLevelOptions.Keys[index];
        }

        private string GetTimePickerStringFromTimePickerValue(int timePickerValue)
        {
            int time = GetTimeFromTimePickerValue(timePickerValue);
            int hour = time / HOUR_DIVISOR;
            int min = time % HOUR_DIVISOR;
            bool isAM = hour % 24 < 12;
            int hourLabel = hour % 12;
            if (hourLabel == 0)
            {
                hourLabel = 12;
            }
            return string.Format("{0}:{1:D2} {2}", hourLabel, min, isAM? "AM" : "PM");
        }
    }
}
