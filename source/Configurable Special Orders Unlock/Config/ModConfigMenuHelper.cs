/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dtomlinson-ga/ConfigurableSpecialOrdersUnlock
**
*************************************************/

// Configurable Special Orders Unlock mod for Stardew Valley
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

using System;

namespace ConfigurableSpecialOrdersUnlock
{
	/// <summary>
	/// Collection of methods to simplify the process of adding Generic Mod Config Menu options.
	/// </summary>
	class ModConfigMenuHelper
	{
		private static IGenericModConfigMenuAPI api;
		private static readonly string[] seasons = new[] { "Spring", "Summer", "Fall", "Winter" };

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
					() => Globals.Config = new ModConfig(),
					() => Globals.Helper.WriteConfig(Globals.Config)
				);

				RegisterModOptions();
				return true;
			}
			catch (Exception e)
			{
				Globals.Monitor.Log("Failed to register GMCM menu - skipping mod menu setup", StardewModdingAPI.LogLevel.Error);
				Globals.Monitor.Log(e.Message, StardewModdingAPI.LogLevel.Error);
				return false;
			}
		}

		/// <summary>
		/// Adds all descriptions and options to options page.
		/// </summary>
		public static void RegisterModOptions()
		{
			AddLabel("Configurable Special Orders Unlock");

			AddCheckBox("Skip installation cutscene", "Skip cutscene where Robin and Lewis install the Special Orders board", () => Globals.Config.skipCutscene, (bool var) => Globals.Config.skipCutscene = var);

			AddLabel("");

			AddIntClamped("Unlock Year", "Year in which Special Orders board unlocks (default is Year 1)", () => Globals.Config.unlockYear, (int var) => Globals.Config.unlockYear = var, 1, 10);
			AddDropdown("Unlock Season", "Season in which Special Orders board unlocks (default is Fall)", () => Globals.Config.unlockSeason, (string var) => Globals.Config.unlockSeason = var, seasons);
			AddIntClamped("Unlock Day", "Day of the month on which Special Orders board unlocks (default is day 2)", () => Globals.Config.unlockDay, (int var) => Globals.Config.unlockDay = var, 1, 28);
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
