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

using Harmony;
using StardewModdingAPI;
using StardewValley;
using System;

namespace ConfigurableSpecialOrdersUnlock
{
	public class SpecialOrdersPatch
	{

		///<summary>
		///Attempts to Harmony patch the following:
		///<br />SpecialOrder.IsSpecialOrdersBoardUnlocked &#8594; CommunityUpgradePatches.IsSpecialOrdersBoardUnlocked_Prefix
		///</summary>
		/// <returns><c>True</c> if successfully patched, <c>False</c> if Exception is encountered.</returns>
		public static bool ApplyHarmonyPatches()
		{
			try
			{
				HarmonyInstance harmony = HarmonyInstance.Create(Globals.Manifest.UniqueID);

				harmony.Patch(
					original: typeof(SpecialOrder).GetMethod("IsSpecialOrdersBoardUnlocked"),
					prefix: new HarmonyMethod(typeof(SpecialOrdersPatch).GetMethod("IsSpecialOrdersBoardUnlocked_Prefix"))
					);

				return true;
			}
			catch (Exception e)
			{
				Globals.Monitor.Log(e.ToString(), LogLevel.Error);
				return false;
			}
		}

		/// <summary>
		/// Harmony patch for <c>SpecialOrder.IsSpecialOrdersBoardUnlocked</c>. Overwrites the standard check with a configurable custom check.
		/// </summary>
		/// <param name="__result"> The modified output to pass to the caller. <c>True</c> if the configured settings are met, <c>False</c> otherwise.</param>
		/// <returns>
		/// <c>True</c> if unable to patch, so that the original method runs instead.
		/// <c>False</c> if successfully patched, in order to skip the original method.
		/// </returns>
		public static bool IsSpecialOrdersBoardUnlocked_Prefix(ref bool __result)
		{
			try
			{
				__result = CheckBoardUnlocked();
				return false;
			}
			catch (Exception ex)
			{
				Globals.Monitor.Log($"Failed in {nameof(IsSpecialOrdersBoardUnlocked_Prefix)}:\n{ex}", LogLevel.Error);
				return true; // run original logic
			}
		}

		/// <summary>
		/// Determines whether or not the board should be unlocked, according to the configured unlock date.
		/// Adds setup cutscene to player's viewed cutscenes if they wish to skip it.
		/// </summary>
		/// <returns>
		/// <c>True</c> if board should be unlocked, <c>False</c> otherwise.</returns>
		public static bool CheckBoardUnlocked()
		{
			if (Game1.stats.DaysPlayed >= Globals.Config.GetUnlockDaysPlayed())
			{
				if (Globals.Config.skipCutscene)
				{
					Game1.player.eventsSeen.Add(15389722);
					Globals.Monitor.Log("Skipping cutscene");
				}

				return true;
			}
			return false;
		}

	}
}
