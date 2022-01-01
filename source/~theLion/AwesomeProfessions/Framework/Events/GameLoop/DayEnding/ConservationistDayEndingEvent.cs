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
using System.Globalization;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Professions.Framework.AssetEditors;

namespace TheLion.Stardew.Professions.Framework.Events;

internal class ConservationistDayEndingEvent : DayEndingEvent
{
    /// <inheritdoc />
    public override void OnDayEnding(object sender, DayEndingEventArgs e)
    {
        if (!ModEntry.ModHelper.Content.AssetEditors.ContainsType(typeof(MailEditor)))
            ModEntry.ModHelper.Content.AssetEditors.Add(new MailEditor());

        uint trashCollectedThisSeason;
        if (Game1.dayOfMonth != 28 ||
            (trashCollectedThisSeason = ModEntry.Data.Read<uint>("WaterTrashCollectedThisSeason")) <=
            0) return;

        var taxBonusNextSeason =
            // ReSharper disable once PossibleLossOfFraction
            Math.Min(trashCollectedThisSeason / ModEntry.Config.TrashNeededPerTaxLevel / 100f,
                ModEntry.Config.TaxDeductionCeiling);
        ModEntry.Data.Write("ActiveTaxBonusPercent",
            taxBonusNextSeason.ToString(CultureInfo.InvariantCulture));
        if (taxBonusNextSeason > 0)
        {
            ModEntry.ModHelper.Content.InvalidateCache(PathUtilities.NormalizeAssetName("Data/mail"));
            Game1.addMailForTomorrow($"{ModEntry.Manifest.UniqueID}/ConservationistTaxNotice");
        }

        ModEntry.Data.Write("WaterTrashCollectedThisSeason", "0");
    }
}