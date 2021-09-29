/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Globalization;
using System.IO;
using TheLion.Stardew.Common.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class ConservationistDayEndingEvent : DayEndingEvent
	{
		/// <inheritdoc/>
		public override void OnDayEnding(object sender, DayEndingEventArgs e)
		{
			if (!ModEntry.ModHelper.Content.AssetEditors.ContainsType(typeof(AssetEditors.FRSMailEditor)))
				ModEntry.ModHelper.Content.AssetEditors.Add(new AssetEditors.FRSMailEditor());

			uint trashCollectedThisSeason;
			if (Game1.dayOfMonth == 28 && (trashCollectedThisSeason = ModEntry.Data.ReadField<uint>("WaterTrashCollectedThisSeason")) > 0)
			{
				var taxBonusNextSeason = Math.Min(trashCollectedThisSeason / ModEntry.Config.TrashNeededPerTaxLevel / 100f, ModEntry.Config.TaxDeductionCeiling);
				ModEntry.Data.WriteField("ActiveTaxBonusPercent", taxBonusNextSeason.ToString(CultureInfo.InvariantCulture));
				if (taxBonusNextSeason > 0)
				{
					ModEntry.ModHelper.Content.InvalidateCache(Path.Combine("Data", "mail"));
					Game1.addMailForTomorrow("ConservationistTaxNotice");
				}
				ModEntry.Data.WriteField("WaterTrashCollectedThisSeason", "0");
			}
		}
	}
}