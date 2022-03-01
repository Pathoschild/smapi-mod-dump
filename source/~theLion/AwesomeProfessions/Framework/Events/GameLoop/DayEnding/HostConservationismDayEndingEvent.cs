/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System;
using System.Globalization;
using System.Linq;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

using Common.Extensions;
using AssetEditors;
using Extensions;

#endregion using directives

internal class HostConservationismDayEndingEvent : DayEndingEvent
{
    /// <inheritdoc />
    protected override void OnDayEndingImpl(object sender, DayEndingEventArgs e)
    {
        if (!ModEntry.ModHelper.Content.AssetEditors.ContainsType(typeof(MailEditor)))
            ModEntry.ModHelper.Content.AssetEditors.Add(new MailEditor());

        if (Game1.dayOfMonth != 28) return;

        foreach (var farmer in Game1.getAllFarmers().Where(f => f.HasProfession(Profession.Conservationist)))
        {
            var trashCollectedThisSeason =
                farmer.ReadDataAs<uint>(DataField.ConservationistTrashCollectedThisSeason);
            if (trashCollectedThisSeason <= 0) return;

            var taxBonusNextSeason =
                // ReSharper disable once PossibleLossOfFraction
                Math.Min(trashCollectedThisSeason / ModEntry.Config.TrashNeededPerTaxLevel / 100f,
                    ModEntry.Config.TaxDeductionCeiling);
            farmer.WriteData(DataField.ConservationistActiveTaxBonusPct,
                taxBonusNextSeason.ToString(CultureInfo.InvariantCulture));
            if (taxBonusNextSeason > 0)
            {
                ModEntry.ModHelper.Content.InvalidateCache(PathUtilities.NormalizeAssetName("Data/mail"));
                farmer.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/ConservationistTaxNotice");
            }

            farmer.WriteData(DataField.ConservationistTrashCollectedThisSeason, "0");
        }
    }
}