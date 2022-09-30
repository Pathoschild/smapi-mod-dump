/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/taggartaa/AutoAnimalDoors
**
*************************************************/

using System;
using System.Collections.Generic;
using AutoAnimalDoors.Config;
using AutoAnimalDoors.StardewValleyWrapper;
using GenericModConfigMenu;
using StardewModdingAPI;

namespace AutoAnimalDoors.Menu
{
	internal class MenuRegistry
	{
		private SortedList<int, String> animalBuildingLevelOptions = new SortedList<int, String>()
        {
			{ 1, "Normal" },
			{ 2, "Big" },
			{ 3, "Deluxe" },
			{ int.MaxValue, "Disabled" }
        };

		private string[] animalBuildingLevelNames
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

				api.RegisterModConfig(manifest, () => config = new ModConfig(), () => Helper.WriteConfig<ModConfig>(config));

				api.RegisterLabel(manifest, "Auto Animal Doors", "");

				api.RegisterSimpleOption(manifest, "Auto Open Enabled",
					"Enables or disables the auto opening of animal doors.",
					() => config.AutoOpenEnabled,
					(bool autoOpenEnabled) => config.AutoOpenEnabled = autoOpenEnabled);

				api.RegisterSimpleOption(manifest, "Animal Door Open Time",
					"The time animal doors are scheduled to open (730 -> 7:30 am, 1310 -> 1:10 pm).",
					() => config.AnimalDoorOpenTime,
					(int newOpenTime) => config.AnimalDoorOpenTime = newOpenTime);
				
				api.RegisterSimpleOption(manifest, "Animal Door Close Time",
					"The time animal doors are scheduled to close (730 -> 7:30 am, 1310 -> 1:10 pm).",
					() => config.AnimalDoorCloseTime,
					(int newOpenTime) => config.AnimalDoorCloseTime = newOpenTime);

				api.RegisterSimpleOption(manifest, "Other Mods Enabled",
					"Enables or disables the auto opening of animal doors from other mods (I can't control door sounds/animation or test every mod).",
					() => config.UnrecognizedAnimalBuildingsEnabled,
					(bool unrecognizedAnimalBulidingsEnabled) => config.UnrecognizedAnimalBuildingsEnabled = unrecognizedAnimalBulidingsEnabled);

				api.RegisterChoiceOption(manifest, "Coop Required Upgrade Level",
					"The coop upgrade level required for auto open/close.",
					() => GetAnimalBuildingUpgradeLevelName(config.CoopRequiredUpgradeLevel),
					(string newLevel) => config.CoopRequiredUpgradeLevel = GetAnimalBuildingUpgradeLevel(newLevel), 
					animalBuildingLevelNames);

				api.RegisterChoiceOption(manifest, "Barn Required Upgrade Level",
					"The barn upgrade level required for auto open/close.",
					() => GetAnimalBuildingUpgradeLevelName(config.BarnRequiredUpgradeLevel),
					(string newLevel) => config.BarnRequiredUpgradeLevel = GetAnimalBuildingUpgradeLevel(newLevel),
					animalBuildingLevelNames);

				api.RegisterChoiceOption(manifest, "Door Sound Setting",
					"When to play/not play the door sound when doors are opened and closed.",
					() => config.DoorSoundSetting.Name(),
					(string doorSoundSettingName) => config.DoorSoundSetting = DoorSoundSettingUtils.FromName(doorSoundSettingName),
					DoorSoundSettingUtils.Names);


				api.RegisterSimpleOption(manifest, "Open Doors When Raining",
					"Enables or disables opening doors when raining/lightning.",
					() => config.OpenDoorsWhenRaining,
					(bool autoOpenEnabled) => config.OpenDoorsWhenRaining = autoOpenEnabled);

				api.RegisterSimpleOption(manifest, "Open Doors During Winter",
					"Enables or disables opening doors during winter.",
					() => config.OpenDoorsDuringWinter,
					(bool autoOpenEnabled) => config.OpenDoorsDuringWinter = autoOpenEnabled);
			}
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
	}
}
