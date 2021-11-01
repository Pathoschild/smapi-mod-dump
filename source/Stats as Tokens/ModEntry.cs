/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dtomlinson-ga/StatsAsTokens
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

namespace StatsAsTokens
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{

		/// <summary>The mod entry point.</summary>
		/// <param name="helper" />
		public override void Entry(IModHelper helper)
		{
			SetUpGlobals(helper);
			SetUpEventHooks();
		}

		/// <summary>Add methods to event hooks.</summary>
		private void SetUpEventHooks()
		{
			Globals.Helper.Events.GameLoop.GameLaunched += (_, _) =>
			{
				HarmonyPatches.PerformHarmonyPatches();
				ContentPatcherHelper.TryLoadContentPatcherAPI();
				ContentPatcherHelper.RegisterSimpleTokens();
				ContentPatcherHelper.RegisterAdvancedTokens();
			};

			Globals.Helper.Events.GameLoop.SaveLoaded += (_, _) =>
			{
				ContentPatcherHelper.RestoreValues();
			};

			Globals.Helper.Events.GameLoop.DayEnding += (_, _) =>
			{
				ContentPatcherHelper.SaveValues();
			};
		}

		/// <summary>Initializes Global variables.</summary>
		/// <param name="helper" />
		private void SetUpGlobals(IModHelper helper)
		{
			Globals.Helper = helper;
			Globals.Monitor = Monitor;
			Globals.Manifest = ModManifest;
		}
	}
}
