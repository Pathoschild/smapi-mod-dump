/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jbossjaslow/Stardew_Mods
**
*************************************************/

using GenericModConfigMenu;
using StardewModdingAPI;

namespace Chatter {
	public class ModConfig {
		public bool enableIndicators = true;
		public SButton enableIndicatorsButton = SButton.O;
		public bool disableIndicatorsForMaxHearts = false;
		public bool useCustomIndicatorImage = false;
		public bool showIndicatorsDuringCutscenes = false;
		public float indicatorScale = 2f;
		public bool disableIndicatorBob = false;
		public bool showIndicatorsWhenMenuIsOpen = false;
		public bool showBirthdayIndicator = false;
		public bool useCustomBirthdayIndicatorImage = false;

		// Debug
		public bool enableDebugOutput = false;
		public bool useDebugOffsetsForAllNPCs = false;
		public float debugIndicatorXOffset = 16f;
		public float debugIndicatorYOffset = -100f;
		public bool useArrowKeysToAdjustDebugOffsets = false;

		public void SetupGenericConfigMenu(IManifest ModManifest, IGenericModConfigMenuApi configMenu) {
			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Enable chat indicators",
				tooltip: () => "Show an indicator above NPCs you haven't yet talked to today",
				getValue: () => enableIndicators,
				setValue: value => enableIndicators = value,
				fieldId: ModConfigField.enableIndicators
			);

			configMenu.AddKeybind(
				mod: ModManifest,
				name: () => "Enable chat indicators keybind",
				tooltip: () => "The keybind to enable or disable chat indicators",
				getValue: () => enableIndicatorsButton,
				setValue: value => enableIndicatorsButton = value,
				fieldId: ModConfigField.enableIndicatorsKeybind
			);

			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Disable indicators for max hearts",
				tooltip: () => "If true, the indicator will not display above an NPC if you already have max friendship with them",
				getValue: () => disableIndicatorsForMaxHearts,
				setValue: value => disableIndicatorsForMaxHearts = value,
				fieldId: ModConfigField.disableIndicatorsForMaxHearts
			);

			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Show indicators during cutscenes",
				tooltip: () => "Kinda silly, but hey it's your config",
				getValue: () => showIndicatorsDuringCutscenes,
				setValue: value => showIndicatorsDuringCutscenes = value,
				fieldId: ModConfigField.showIndicatorsDuringCutscenes
			);

			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Use custom indicator icon",
				tooltip: () => "Whether the custom indicator icon should be used",
				getValue: () => useCustomIndicatorImage,
				setValue: value => useCustomIndicatorImage = value,
				fieldId: ModConfigField.useCustomIndicatorImage
			);

			configMenu.AddParagraph(
				mod: ModManifest,
				text: () => "To use your own custom icon, create a file named \"indicator.png\" and place it in the \"Customization\" folder within the Chatter mod folder. Currently supports 16x16 images"
			);

			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => "Indicator scale",
				tooltip: () => "The size of the indicator",
				getValue: () => indicatorScale,
				setValue: value => indicatorScale = value,
				min: 0.25f,
				max: 4f,
				interval: 0.25f,
				fieldId: ModConfigField.indicatorScale
			);

			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Disable indicator bobbing",
				tooltip: () => "Removes the up and down motion of the indicator",
				getValue: () => disableIndicatorBob,
				setValue: value => disableIndicatorBob = value,
				fieldId: ModConfigField.disableIndicatorBob
			);

			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Show indicator when menu is open",
				tooltip: () => "If the menu is open, continue to show the indicators in the background (may have issues if layer is too high)",
				getValue: () => showIndicatorsWhenMenuIsOpen,
				setValue: value => showIndicatorsWhenMenuIsOpen = value,
				fieldId: ModConfigField.showIndicatorsWhenMenuIsOpen
			);

			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Show a birthday indicator",
				tooltip: () => "Shows a birthday indicator in place of a normal indicator if today is that NPC's birthday",
				getValue: () => showBirthdayIndicator,
				setValue: value => showBirthdayIndicator = value,
				fieldId: ModConfigField.showBirthdayIndicator
			);

			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Use custom birthday indicator icon",
				tooltip: () => "Whether the custom birthday indicator icon should be used",
				getValue: () => useCustomBirthdayIndicatorImage,
				setValue: value => useCustomBirthdayIndicatorImage = value,
				fieldId: ModConfigField.useCustomBirthdayIndicatorImage
			);

			configMenu.AddParagraph(
				mod: ModManifest,
				text: () => "To use your own custom birthday icon, create a file named \"birthdayIndicator.png\" and place it in the \"Customization\" folder within the Chatter mod folder. Currently supports 16x16 images"
			);

			configMenu.AddPageLink(
				mod: ModManifest,
				pageId: ModConfigPageID.debug,
				text: () => "Show Debug Options",
				tooltip: () => "Configs used for debugging, if you want more control over the mod"
			);

			configMenu.AddParagraph(
				mod: ModManifest,
				text: () => ""
			);

			SetupDebugMenu(ModManifest, configMenu);
		}
		private void SetupDebugMenu(IManifest ModManifest, IGenericModConfigMenuApi configMenu) {
			configMenu.AddPage(
				mod: ModManifest,
				pageId: ModConfigPageID.debug,
				pageTitle: () => "Debug Options"
			);

			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Enable debug output",
				tooltip: () => "Show output in the debug console",
				getValue: () => enableDebugOutput,
				setValue: value => enableDebugOutput = value,
				fieldId: ModConfigField.enableDebugOutput
			);

			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Use debug offsets for all NPCs",
				tooltip: () => "Overrides the indicator offets for all NPCs",
				getValue: () => useDebugOffsetsForAllNPCs,
				setValue: value => useDebugOffsetsForAllNPCs = value,
				fieldId: ModConfigField.useDebugOffsetsForAllNPCs
			);

			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Adjust Y offset with arrow keys",
				tooltip: () => "Allows more fine adjustment of the debug y offset using the up and down arrow keys",
				getValue: () => useArrowKeysToAdjustDebugOffsets,
				setValue: value => useArrowKeysToAdjustDebugOffsets = value,
				fieldId: ModConfigField.useArrowKeysToAdjustDebugOffsets
			);

			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => "Indicator X offset",
				tooltip: () => "The X offset from the NPC's origin to draw the indicator, if debug offsets is enabled",
				getValue: () => debugIndicatorXOffset,
				setValue: value => debugIndicatorXOffset = value,
				min: 0f,
				max: 32f,
				interval: 1f,
				fieldId: ModConfigField.indicatorXOffset
			);

			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => "Indicator Y offset",
				tooltip: () => "The Y offset from the NPC's origin to draw the indicator, if debug offsets is enabled",
				getValue: () => debugIndicatorYOffset,
				setValue: value => debugIndicatorYOffset = value,
				min: -150f,
				max: 0f,
				interval: 2f,
				fieldId: ModConfigField.indicatorYOffset
			);

			configMenu.AddParagraph(
				mod: ModManifest,
				text: () => "Version " + ModManifest.Version
			);
		}
	}

	public class ModConfigField {
		public const string enableIndicators = "enableIndicators";
		public const string enableIndicatorsKeybind = "enableIndicatorsKeybind";
		public const string disableIndicatorsForMaxHearts = "disableIndicatorsForMaxHearts";
		public const string useCustomIndicatorImage = "useCustomIndicatorImage";
		public const string showIndicatorsDuringCutscenes = "showIndicatorsDuringCutscenes";
		public const string indicatorScale = "indicatorScale";
		public const string disableIndicatorBob = "disableIndicatorBob";
		public const string showIndicatorsWhenMenuIsOpen = "showIndicatorsWhenMenuIsOpen";
		public const string showBirthdayIndicator = "showBirthdayIndicator";
		public const string useCustomBirthdayIndicatorImage = "useCustomBirthdayIndicatorImage";

		// Debug
		public const string enableDebugOutput = "enableDebugOutput";
		public const string useDebugOffsetsForAllNPCs = "useDebugOffsetsForAllNPCs";
		public const string indicatorXOffset = "debugIndicatorXOffset";
		public const string indicatorYOffset = "debugIndicatorYOffset";
		public const string useArrowKeysToAdjustDebugOffsets = "useArrowKeysToAdjustDebugOffsets";
	}

	public class ModConfigPageID {
		public const string debug = "debug";
	}
}
