using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewConfigFramework;
using StardewConfigFramework.Options;
using System.Collections.Generic;

namespace SCFTester2 {

	public class ModEntry: Mod {
		void Testbox_ValueDidChange(string identifier, bool isOn) {
		}

		internal static IConfigMenu Settings;
		internal static SimpleOptionsPackage Package;
		/*********
		** Public methods
		*********/
		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper) {
			SaveEvents.AfterLoad += SaveEvents_AfterLoad;
			GameEvents.FirstUpdateTick += LoadMenu;
		}

		void LoadMenu(object sender, EventArgs e) {
			Settings = Helper.ModRegistry.GetApi<IConfigMenu>("Juice805.StardewConfigMenu");
			Package = new SimpleOptionsPackage(this);
			var config = Helper.ReadConfig<TestConfig>();
			Settings.AddOptionsPackage(Package);

			var testbox = new ConfigToggle("checkbox", "Checkbox", config.checkbox);
			Package.AddOption(testbox);

			var emptyDropdown = new ConfigSelection("emptyDropdown", "Empty Dropdowns are disabled");
			Package.AddOption(emptyDropdown);

			testbox.StateDidChange += (toggle) => {
				emptyDropdown.Enabled = toggle.IsOn; // should not do anything
			};

			var list = new List<ISelectionChoice>();
			list.Add(new SelectionChoice("first", "First", "This is the first option!"));
			list.Add(new SelectionChoice("second", "Second", "This is the Second option!"));
			list.Add(new SelectionChoice("third", "Third"));
			list.Add(new SelectionChoice("fourth", "Fourth"));

			var filledDropdown = new ConfigSelection("filledDropdown", "Filled Dropdown", list, config.filledDropown, true);
			Package.AddOption(filledDropdown);

			var stepper = new ConfigStepper("stepper", "Plus/Minus Controls", (decimal) 5.0, (decimal) 105.0, (decimal) 1.5, config.stepperValue, RangeDisplayType.PERCENT);
			Package.AddOption(stepper);

			var label = new ConfigHeader("catlabel", "Category Label");
			Package.AddOption(label);

			var button = new ConfigAction("setButton", "Click Me!", ButtonType.SET);
			button.ActionWasTriggered += (identifier) => {
				filledDropdown.Enabled = !filledDropdown.Enabled;
			};
			Package.AddOption(button);

			var tranformingButton = new ConfigAction("clearButton", "Clear Button", ButtonType.CLEAR);
			tranformingButton.ButtonType = ButtonType.CLEAR;
			tranformingButton.ActionWasTriggered += (identifier) => {
				switch (tranformingButton.ButtonType) {
					case ButtonType.CLEAR:
						tranformingButton.Label = "Are you sure?";
						tranformingButton.ButtonType = ButtonType.OK;
						break;
					case ButtonType.OK:
						tranformingButton.Label = "Cleared";
						tranformingButton.ButtonType = ButtonType.DONE;
						break;
					case ButtonType.DONE:
						tranformingButton.Label = "Clear Button";
						tranformingButton.ButtonType = ButtonType.CLEAR;
						break;
					default:
						tranformingButton.Label = "Clear Button";
						tranformingButton.ButtonType = ButtonType.CLEAR;
						break;
				}
			};

			Package.AddOption(tranformingButton);

			Package.AddOption(new ConfigAction("doneButton", "Done Button", ButtonType.DONE));
			Package.AddOption(new ConfigAction("giftButton", "Gift Button", ButtonType.GIFT));

			var saveButton = new ConfigAction("okButton", "OK Button", ButtonType.OK);
			Package.AddOption(saveButton);

			saveButton.ActionWasTriggered += (_) => {
				SaveConfig();
			};
		}


		private void SaveEvents_AfterLoad(object sender, EventArgs e) {

		}

		private void SaveConfig() {
			var config = new TestConfig();

			config.checkbox = Package.GetOption<IConfigToggle>("checkbox").IsOn;
			config.filledDropown = Package.GetOption<IConfigSelection>("filledDropdown").SelectedIdentifier;
			config.stepperValue = Package.GetOption<IConfigStepper>("stepper").Value;
			Helper.WriteConfig<TestConfig>(config);
		}



		/*********
		** Private methods
		*********/
		/// <summary>The method invoked when the game is opened.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void StardewConfigFrameworkLoaded() {

		}
	}
}
