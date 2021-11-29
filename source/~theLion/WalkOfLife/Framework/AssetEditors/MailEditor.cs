/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.AssetEditors
{
	public class MailEditor : IAssetEditor
	{
		/// <inheritdoc />
		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Data/mail"));
		}

		/// <inheritdoc />
		public void Edit<T>(IAssetData asset)
		{
			if (!asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Data/mail")))
				throw new InvalidOperationException($"Unexpected asset {asset.AssetName}.");

			// patch mail from the Ferngill Revenue Service
			var data = asset.AsDictionary<string, string>().Data;
			var taxBonus = ModEntry.Data.Read<float>("ActiveTaxBonusPercent");
			var key = taxBonus >= ModEntry.Config.TaxDeductionCeiling
				? "conservationist.mail2"
				: "conservationist.mail1";

			string message = ModEntry.ModHelper.Translation.Get(key,
				new {taxBonus = $"{taxBonus:p0}", farmName = Game1.getFarm().Name});
			data[$"{ModEntry.Manifest.UniqueID}/ConservationistTaxNotice"] = message;
		}
	}
}