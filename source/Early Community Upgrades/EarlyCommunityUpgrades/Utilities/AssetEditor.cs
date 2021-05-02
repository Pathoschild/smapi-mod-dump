/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dtomlinson-ga/EarlyCommunityUpgrades
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;
using System.Globalization;

namespace EarlyCommunityUpgrades
{
	public class AssetEditor : IAssetEditor
	{

		/// <summary>
		/// Determines whether or not the asset should be edited.
		/// Only edits Strings/Locations if the config options are not the default values.
		/// </summary>
		/// <typeparam name="T" />
		/// <param name="asset" />
		/// <returns><c>True</c> if the asset should be edited, otherwise <c>False</c>.</returns>
		public bool CanEdit<T>(IAssetInfo asset)
		{
			if (asset.AssetNameEquals("Strings/Locations"))
			{
				return !(Globals.Config.Costs.pamCostGold == 500000
					&& Globals.Config.Costs.pamCostWood == 950
					&& Globals.Config.Costs.shortcutCostGold == 300000);
			}
			return false;
		}

		/// <summary>
		/// Performs edits to Robin's dialog lines concerning the amounts of wood and gold required for the community upgrades.
		/// </summary>
		/// <typeparam name="T" />
		/// <param name="asset" />
		public void Edit<T>(IAssetData asset)
		{
			if (asset.AssetNameEquals("Strings/Locations"))
			{
				IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
				CultureInfo culture = CultureInfo.GetCultureInfo(Globals.Helper.Translation.Locale);

				string pamWood = (Globals.Config.Costs.pamCostWood == 0) ? "0" : Globals.Config.Costs.pamCostWood.ToString("#,###", culture);
				string pamGold = (Globals.Config.Costs.pamCostGold == 0) ? "0" : Globals.Config.Costs.pamCostGold.ToString("#,###", culture);
				string shortcutGold = (Globals.Config.Costs.shortcutCostGold == 0) ? "0" : Globals.Config.Costs.shortcutCostGold.ToString("#,###", culture);

				data["ScienceHouse_Carpenter_CommunityUpgrade1"] = Globals.GetTranslation("ScienceHouse_Carpenter_CommunityUpgrade1", new
				{
					woodPieces = pamWood,
					goldPieces = pamGold
				});

				data["ScienceHouse_Carpenter_CommunityUpgrade2"] = Globals.GetTranslation("ScienceHouse_Carpenter_CommunityUpgrade2", new
				{
					goldPieces = shortcutGold
				});

				data["ScienceHouse_Carpenter_NotEnoughWood3"] = Globals.GetTranslation("ScienceHouse_Carpenter_NotEnoughWood3", new
				{
					woodPieces = pamWood
				});

			}
		}

		/// <summary>
		/// Forces invalidation of Strings/Locations.
		/// This prevents cached values from being used if the player has changed config options or locales.
		/// </summary>
		/// <param name="args" />
		public void ReloadI18n()
		{
			Globals.Helper.Content.InvalidateCache("Strings/Locations");
		}
	}
}
