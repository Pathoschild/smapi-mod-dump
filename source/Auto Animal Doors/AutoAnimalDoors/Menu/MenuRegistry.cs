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
using AutoAnimalDoors.Config;
using AutoAnimalDoors.StardewValleyWrapper;
using GenericModConfigMenu;
using StardewModdingAPI;

namespace AutoAnimalDoors.Menu
{
	internal class MenuRegistry
	{
		private string[] animalBuildingLevelNames = new string[3] { "Normal", "Big", "Deluxe" };


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

				api.RegisterChoiceOption(manifest, "Coop Required Upgrade Level",
					"The coop upgrade level required for auto open/close.",
					() => GetAnimalBuildingUpgradeLevelName(config.CoopRequiredUpgradeLevel),
					(string newLevel) => config.CoopRequiredUpgradeLevel = GetAnimalBuildingUpgradeLevel(newLevel), animalBuildingLevelNames);

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
			int num = level - 1;
			if (num < 0 || num >= animalBuildingLevelNames.Length)
			{
				num = 0;
			}
			return animalBuildingLevelNames[num];
		}

		private int GetAnimalBuildingUpgradeLevel(string name)
		{
			int num = Array.FindIndex(animalBuildingLevelNames, (string element) => element == name);
			if (num < 0)
			{
				num = 0;
			}
			return num + 1;
		}
	}
}
