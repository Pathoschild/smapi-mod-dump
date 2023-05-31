/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jbossjaslow/Chatter
**
*************************************************/

using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System.Collections.Generic;

namespace VillagerCompass {
	public class ModConfig {
		public bool enableMod = false;
		public SButton enableModButton = SButton.P;
		public KeybindList openModPageKeybindCombo = new();
		public string villagerToFind = "Emily";
		public List<string> villagerList = new();

		public void SetupGenericConfigMenu(IManifest ModManifest, IGenericModConfigMenuApi configMenu) {
			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Enable the villager compass",
				tooltip: () => "Show an arrow pointing towards the desired villager",
				getValue: () => enableMod,
				setValue: value => enableMod = value,
				fieldId: ModConfigField.enableMod
			);

			configMenu.AddKeybind(
				mod: ModManifest,
				name: () => "Enable villager compass keybind",
				tooltip: () => "The keybind to enable or disable the villager compass",
				getValue: () => enableModButton,
				setValue: value => enableModButton = value,
				fieldId: ModConfigField.enableModButton
			);

			configMenu.AddKeybindList(
				mod: ModManifest,
				getValue: () => openModPageKeybindCombo,
				setValue: value => openModPageKeybindCombo = value,
				name: () => "Config page keybind",
				tooltip: () => "Keybind to open this config page, for easier access to villager list",
				fieldId: ModConfigField.openModPageKeybindCombo
			);

			configMenu.AddTextOption(
				mod: ModManifest,
				getValue: () => villagerToFind,
				setValue: value => villagerToFind = value,
				name: () => "Villager",
				tooltip: () => "The villager to search for",
				allowedValues: villagerList.ToArray(),
				formatAllowedValue: null,
				fieldId: villagerToFind
			);

			if (villagerList.Count == 0) {
				configMenu.AddParagraph(
					mod: ModManifest,
					text: () => "To populate the villager list, load the save, then restart the game."
				);
			}
		}
	}

	public class ModConfigField {
		public const string enableMod = "enableMod";
		public const string enableModButton = "enableModButton";
		public const string openModPageKeybindCombo = "openModPageKeybindCombo";
		public const string villagerToFind = "villagerToFind";
	}
}
