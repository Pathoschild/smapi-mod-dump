/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/StardewConfigFramework/StardewConfigMenu
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewConfigFramework;
using System.Collections.Specialized;
using System.Collections.Generic;
using StardewConfigFramework.Options;

namespace SCFTester {
	public class ModEntry: Mod {
		internal static IConfigMenu Settings;
		internal static TabbedOptionsPackage Package;
		/*********
		** Public methods
		*********/
		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper) {
			GameEvents.FirstUpdateTick += GameEvents_FirstUpdateTick;
		}

		void GameEvents_FirstUpdateTick(object sender, System.EventArgs e) {
			Settings = Helper.ModRegistry.GetApi<IConfigMenu>("Juice805.StardewConfigMenu");
			Package = new TabbedOptionsPackage(this);
			var config = Helper.ReadConfig<ModConfig>();
			GenerateOptions(Package, config);
			Settings.AddOptionsPackage(Package);
		}

		private void GenerateOptions(TabbedOptionsPackage options, ModConfig config) {
			var firstTab = new OptionsTab("main", "Main");
			options.Tabs.Add(firstTab);

			var enableDrop = new ConfigToggle("enableDrop", "Enable Dropdown", config.enableDropdown);
			firstTab.Options.Add(enableDrop);

			var choices = new List<ISelectionChoice> {
				new SelectionChoice("none", "None"),
				new SelectionChoice("5", "Checkbox 5", "Hover text for Checkbox 5"),
				new SelectionChoice("6", "Checkbox 6", "Hover text for Checkbox 6"),
				new SelectionChoice("7", "Checkbox 7", "Hover text for Checkbox 7")
			};

			var dropdown = new ConfigSelection("drop", "Disable Another Option", choices, config.dropdownChoice, config.enableDropdown);
			dropdown.SelectionDidChange += Dropdown_SelectionDidChange; ;
			firstTab.Options.Add(dropdown);

			enableDrop.StateDidChange += (toggle) => {
				dropdown.Enabled = toggle.IsOn;
			};

			var checkbox2 = new ConfigToggle("toggle2", "Add checkbox 9", config.checkbox2);
			firstTab.Options.Add(checkbox2);
			checkbox2.StateDidChange += AddDynamicOption;

			firstTab.Options.Add(new ConfigToggle("toggle3", "Checkbox 3", false));

			var slider = new ConfigRange("range", "Slider", 10, 25, 1, config.rangeValue, true);

			var stepper = new ConfigStepper("stepper", "Plus/Minus Controls", (decimal) 5.0, (decimal) 105.0, (decimal) 1.5, config.stepperValue, RangeDisplayType.PERCENT);

			firstTab.Options.Add(slider);
			firstTab.Options.Add(stepper);

			firstTab.Options.Add(new ConfigToggle("stepperCheck", "Show Stepper Value", false));

			firstTab.Options.Add(new ConfigToggle("toggle5", "Checkbox 5"));
			firstTab.Options.Add(new ConfigToggle("toggle6", "Checkbox 6"));
			firstTab.Options.Add(new ConfigToggle("toggle7", "Checkbox 7"));
			firstTab.Options.Add(new ConfigToggle("toggle8", "Checkbox 8"));

			var saveButton = new ConfigAction("okButton", "OK Button", ButtonType.OK);
			firstTab.Options.Add(saveButton);

			saveButton.ActionWasTriggered += SaveButton_ActionWasTriggered;

			GraphicsEvents.OnPostRenderEvent += (sender, e) => {
				if (firstTab.GetOption<IConfigToggle>("toggle3").IsOn)
					Game1.spriteBatch.DrawString(Game1.dialogueFont, "Cool!", new Vector2(Game1.getMouseX(), Game1.getMouseY()), Color.Black);

				if (firstTab.GetOption<IConfigToggle>("stepperCheck").IsOn) {
					Game1.spriteBatch.DrawString(Game1.dialogueFont, stepper.Value.ToString(), new Vector2(Game1.getMouseX(), Game1.getMouseY() + 12 * Game1.pixelZoom), Color.Black);
				}
			};

			var secondTab = new OptionsTab("second", "Second");
			secondTab.Options.Add(new ConfigHeader("secondTabHeader", "Second Tab!"));
			options.Tabs.Add(secondTab);
		}

		void AddDynamicOption(IConfigToggle toggle) {
			if (toggle.IsOn) {
				var targetIndex = Package.Tabs[0].Options.IndexOf("toggle8") + 1;
				var dynamicOption = new ConfigToggle("toggle9", "Added dynamically!", false);
				Package.Tabs[0].Options.Insert(targetIndex, dynamicOption);
				dynamicOption.StateDidChange += AddDynamicTab;
			} else {
				Package.Tabs[0].Options.Remove("toggle9");
			}
		}

		void AddDynamicTab(IConfigToggle toggle) {
			if (toggle.IsOn) {
				var dynamicTab = new OptionsTab("dynamic", "Dynamic");
				dynamicTab.Options.Add(new ConfigHeader("header", "Dynamically Added Tab!"));
				Package.Tabs.Add(dynamicTab);
			} else {
				Package.Tabs.Remove("dynamic");
			}
		}

		void Dropdown_SelectionDidChange(IConfigSelection selection) {
			var selected = selection.SelectedIdentifier;
			Package.Tabs[0].GetOption<IConfigToggle>("toggle5").Enabled = !("5" == selected);
			Package.Tabs[0].GetOption<IConfigToggle>("toggle6").Enabled = !("6" == selected);
			Package.Tabs[0].GetOption<IConfigToggle>("toggle7").Enabled = !("7" == selected);
		}

		void SaveButton_ActionWasTriggered(IConfigAction action) {
			var config = new ModConfig() {
				dropdownChoice = Package.Tabs[0].GetOption<IConfigSelection>("drop").SelectedIdentifier,
				enableDropdown = Package.Tabs[0].GetOption<IConfigToggle>("enableDrop").IsOn,
				checkbox2 = Package.Tabs[0].GetOption<IConfigToggle>("toggle2").IsOn
			};
			this.Helper.WriteConfig<ModConfig>(config);
		}


		/*********
		** Private methods
		*********/

		private void StardewConfigFrameworkLoaded() {

		}
	}
}
