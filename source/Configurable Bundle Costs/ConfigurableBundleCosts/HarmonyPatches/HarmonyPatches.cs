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

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System;

namespace ConfigurableBundleCosts
{
	public class HarmonyPatches
	{

		/// <returns><c>True</c> if successfully patched, <c>False</c> if Exception is encountered.</returns>
		public static bool ApplyHarmonyPatches()
		{
			try
			{
				Harmony harmony = new(Globals.Manifest.UniqueID);

				harmony.Patch(
					original: typeof(JojaCDMenu).GetMethod("getPriceFromButtonNumber"),
					prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(getPriceFromButtonNumber_Prefix))
				);

				harmony.Patch(
					original: AccessTools.Method(typeof(JojaMart), "buyMovieTheater"),
					prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(buyMovieTheater_Prefix))
				);

				return true;
			}
			catch (Exception e)
			{
				Globals.Monitor.Log(e.ToString(), LogLevel.Error);
				return false;
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Harmony method - maintain case from original")]
		public static bool getPriceFromButtonNumber_Prefix(int buttonNumber, ref int __result)
		{
			try
			{
				switch (buttonNumber)
				{
					case 0:
						__result = Globals.CurrentValues.Joja.applyValues ? Globals.CurrentValues.Joja.busCost : 40000;
						break;
					case 1:
						__result = Globals.CurrentValues.Joja.applyValues ? Globals.CurrentValues.Joja.minecartsCost : 15000;
						break;
					case 2:
						__result = Globals.CurrentValues.Joja.applyValues ? Globals.CurrentValues.Joja.bridgeCost : 25000;
						break;
					case 3:
						__result = Globals.CurrentValues.Joja.applyValues ? Globals.CurrentValues.Joja.greenhouseCost : 35000;
						break;
					case 4:
						__result = Globals.CurrentValues.Joja.applyValues ? Globals.CurrentValues.Joja.panningCost : 20000;
						break;
					default:
						__result = 10000;
						Globals.Monitor.Log("Unrecognized button number selected", LogLevel.Warn);
						break;
				}

				return false;
			}
			catch (Exception ex)
			{
				Globals.Monitor.Log($"Failed in {nameof(getPriceFromButtonNumber_Prefix)}:\n{ex}", LogLevel.Error);
				return true; // run original logic
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Harmony method - maintain case from original")]
		public static bool buyMovieTheater_Prefix(int response, bool __result)
		{
			try
			{
				int cost = Globals.CurrentValues.Joja.applyValues ? Globals.CurrentValues.Joja.movieTheaterCost : 500000;

				if (response == 0)
				{
					if (Game1.player.Money >= cost)
					{
						Game1.player.Money -= cost;
						Game1.addMailForTomorrow("ccMovieTheater", noLetter: true, sendToEveryone: true);
						Game1.addMailForTomorrow("ccMovieTheaterJoja", noLetter: true, sendToEveryone: true);
						if (Game1.player.team.theaterBuildDate.Value < 0)
						{
							Game1.player.team.theaterBuildDate.Set(Game1.Date.TotalDays + 1);
						}
						JojaMart.Morris.setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Morris_TheaterBought"));
						Game1.drawDialogue(JojaMart.Morris);
					}
					else
					{
						Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11325"));
					}
				}
				__result = true;
				return false;
			}
			catch (Exception ex)
			{
				Globals.Monitor.Log($"Failed in {nameof(buyMovieTheater_Prefix)}:\n{ex}", LogLevel.Error);
				return true; // run original logic
			}
		}
	}
}
