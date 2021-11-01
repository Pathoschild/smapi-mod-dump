/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dtomlinson-ga/ConfigurableBundleCosts
**
*************************************************/

// Copyright (C) 2021 Vertigon
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://www.gnu.org/licenses/.

using StardewModdingAPI;
using System;

namespace ConfigurableBundleCosts
{
	/// <summary>
	/// Collection of methods to simplify the process of adding Generic Mod Config Menu options.
	/// </summary>
	internal class ModConfigMenuHelper
	{

		private static IGenericModConfigMenuAPI api;

		/// <summary>
		/// Checks to see if GMCM is installed - if so, creates options page with all configurable settings.
		/// </summary>
		/// <returns> <c>True</c> if options page successfully created, <c>False</c> otherwise.</returns>
		public static bool TryLoadModConfigMenu()
		{
			try
			{
				// Check to see if Generic Mod Config Menu is installed
				if (!Globals.Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
				{
					Globals.Monitor.Log("GenericModConfigMenu not present - skipping mod menu setup");
					return false;
				}

				api = Globals.Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
				api.RegisterModConfig(Globals.Manifest,
					() => Globals.InitialValues = new ModConfig(),
					() => Globals.Helper.WriteConfig(Globals.InitialValues)
				);

				//CheckForContentPacks();
				RegisterModOptions();
				return true;
			}
			catch (Exception e)
			{
				Globals.Monitor.Log("Failed to register GMCM menu - skipping mod menu setup", LogLevel.Error);
				Globals.Monitor.Log(e.Message, LogLevel.Error);
				return false;
			}
		}

		/// <summary>
		/// Adds all descriptions and options to options page. Adds disclaimer if content packs are loaded that they will take priority over values set here.
		/// </summary>
		public static void RegisterModOptions()
		{
			if (ContentPackHelper.contentPacksLoaded)
			{
				AddLabel("Note:");
				AddParagraph("Any values provided by a loaded content pack will take priority over values set here. To override content-pack-provided values, see Configurable Bundle Cost's NexusMods page for instructions on creating a config.override.json file.");
				AddLabel("");
			}

			AddCheckBox("Apply Joja config", "Enables or disables these values being applied. May be useful if you are using another mod or content pack which alters these values.",
				() => Globals.InitialValues.Joja.applyValues,
				(bool var) => Globals.InitialValues.Joja.applyValues = var
			);

			AddLabel("Joja Costs");
			AddIntUnclamped("Membership Cost", "Cost to purchase a Joja membership",
				() => Globals.InitialValues.Joja.membershipCost,
				(int var) => Globals.InitialValues.Joja.membershipCost = var
			);

			AddIntUnclamped("Movie Theater Cost", "Cost to purchase the Movie Theater from Joja",
				() => Globals.InitialValues.Joja.movieTheaterCost,
				(int var) => Globals.InitialValues.Joja.movieTheaterCost = var
			);

			AddLabel("Joja Bundle Costs");
			AddIntUnclamped("Bus Cost", "Cost to repair the bus to Calico Desert",
				() => Globals.InitialValues.Joja.busCost,
				(int var) => Globals.InitialValues.Joja.busCost = var
			);

			AddIntUnclamped("Minecarts Cost", "Cost to repair the minecart system around town",
				() => Globals.InitialValues.Joja.minecartsCost,
				(int var) => Globals.InitialValues.Joja.minecartsCost = var
			);

			AddIntUnclamped("Bridge Cost", "Cost to repair the bridge to the quarry",
				() => Globals.InitialValues.Joja.bridgeCost,
				(int var) => Globals.InitialValues.Joja.bridgeCost = var
			);

			AddIntUnclamped("Greenhouse Cost", "Cost to repair the greenhouse on the farm",
				() => Globals.InitialValues.Joja.greenhouseCost,
				(int var) => Globals.InitialValues.Joja.greenhouseCost = var
			);

			AddIntUnclamped("Panning Cost", "Cost to remove the glittering boulder on the mountain",
				() => Globals.InitialValues.Joja.panningCost,
				(int var) => Globals.InitialValues.Joja.panningCost = var
			);

			AddLabel("");

			AddCheckBox("Apply Vault config", "Enables or disables these values being applied. May be useful if you are using another mod or content pack which alters these values.",
				() => Globals.InitialValues.Vault.applyValues,
				(bool var) => Globals.InitialValues.Vault.applyValues = var
			);
			AddLabel("Vault Bundle Costs");

			AddIntUnclamped("Vault Bundle 1 Cost", "Cost of Vault Bundle 1",
				() => Globals.InitialValues.Vault.bundle1,
				(int var) => Globals.InitialValues.Vault.bundle1 = var
			);

			AddIntUnclamped("Vault Bundle 2 Cost", "Cost of Vault Bundle 2",
				() => Globals.InitialValues.Vault.bundle2,
				(int var) => Globals.InitialValues.Vault.bundle2 = var
			);

			AddIntUnclamped("Vault Bundle 3 Cost", "Cost of Vault Bundle 3",
				() => Globals.InitialValues.Vault.bundle3,
				(int var) => Globals.InitialValues.Vault.bundle3 = var
			);

			AddIntUnclamped("Vault Bundle 4 Cost", "Cost of Vault Bundle 4",
				() => Globals.InitialValues.Vault.bundle4,
				(int var) => Globals.InitialValues.Vault.bundle4 = var
			);

		}

		/// <summary>
		/// Shorthand method to create a Label.
		/// </summary>
		private static void AddLabel(string name, string desc = "")
		{
			api.RegisterLabel(Globals.Manifest, name, desc);
		}

		/// <summary>
		/// Shorthand method to create a Paragraph.
		/// </summary>
		private static void AddParagraph(string text)
		{
			api.RegisterParagraph(Globals.Manifest, text);
		}

		/// <summary>
		/// Shorthand method to create a Dropdown menu.
		/// </summary>
		private static void AddDropdown(string name, string desc, Func<string> get, Action<string> set, string[] choices)
		{
			api.RegisterChoiceOption(Globals.Manifest, name, desc, get, set, choices);
		}

		/// <summary>
		/// Shorthand method to create an Integer input field.
		/// </summary>
		private static void AddIntUnclamped(string name, string desc, Func<int> get, Action<int> set)
		{
			api.RegisterSimpleOption(Globals.Manifest, name, desc, get, set);
		}

		/// <summary>
		/// Shorthand method to create an Integer slider.
		/// </summary>
		private static void AddIntClamped(string name, string desc, Func<int> get, Action<int> set, int min, int max)
		{
			api.RegisterClampedOption(Globals.Manifest, name, desc, get, set, min, max);
		}

		/// <summary>
		/// Shorthand method to create a checkbox.
		/// </summary>
		private static void AddCheckBox(string name, string desc, Func<bool> get, Action<bool> set)
		{
			api.RegisterSimpleOption(Globals.Manifest, name, desc, get, set);
		}
	}
}
