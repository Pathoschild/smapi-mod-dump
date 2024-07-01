/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shockah.XPDisplay.IMargoAPI.IModConfig;

namespace Shockah.XPDisplay.WalkOfLife
{
	public interface IProfessionsApi
	{
		#region professions

		/// <summary>Gets the value of an Ecologist's forage quality.</summary>
		/// <param name="farmer">The player.</param>
		/// <returns>A <see cref="SObject"/> quality level.</returns>
		int GetEcologistForageQuality(Farmer? farmer = null);

		/// <summary>Gets the value of a Gemologist's mineral quality.</summary>
		/// <param name="farmer">The player.</param>
		/// <returns>A <see cref="SObject"/> quality level.</returns>
		int GetGemologistMineralQuality(Farmer? farmer = null);

		/// <summary>Gets the price bonus applied to animal produce sold by Producer.</summary>
		/// <param name="farmer">The player.</param>
		/// <returns>A bonus applied to Producer animal product prices.</returns>
		float GetProducerSaleBonus(Farmer? farmer = null);

		/// <summary>Gets the price bonus applied to fish sold by Angler.</summary>
		/// <param name="farmer">The player.</param>
		/// <returns>A bonus applied to Angler fish prices.</returns>
		float GetAnglerSaleBonus(Farmer? farmer = null);

		/// <summary>
		///     Gets the value of the Conservationist's effective tax deduction based on the preceding season's trash
		///     collection.
		/// </summary>
		/// <param name="farmer">The player.</param>
		/// <returns>The percentage of tax deductions currently in effect due to the preceding season's collected trash.</returns>
		float GetConservationistTaxDeduction(Farmer? farmer = null);

		/// <summary>Determines the extra power of Desperado shots.</summary>
		/// <param name="farmer">The player.</param>
		/// <returns>A percentage between 0 and 1.</returns>
		float GetDesperadoOvercharge(Farmer? farmer = null);

		#endregion professions

		#region tresure hunts

		/// <inheritdoc cref="ITreasureHunt.IsActive"/>
		/// <param name="farmer">The <see cref="Farmer"/>.</param>
		/// <returns><see langword="true"/> if the specified <see cref="ITreasureHunt"/> <paramref name="type"/> is currently active, otherwise <see langword="false"/>.</returns>
		bool IsHuntActive(Farmer? farmer = null);

		#endregion treasure hunts

		#region limit break

		/// <summary>Gets the <paramref name="farmer"/>'s currently registered <see cref="ILimitBreak"/>, if any.</summary>
		/// <param name="farmer">The <see cref="Farmer"/>.</param>
		/// <returns>The <paramref name="farmer"/>'s <see cref="ILimitBreak"/>'s technical name, or the local player's if supplied null.</returns>
		int GetLimitBreakId(Farmer? farmer = null);


		#endregion limit break
		/// <summary>Gets the mod's current config schema.</summary>
		/// <returns>The current <see cref="IProfessionsConfig"/> instance.</returns>
		IProfessionsConfig GetConfig();
	}
}
