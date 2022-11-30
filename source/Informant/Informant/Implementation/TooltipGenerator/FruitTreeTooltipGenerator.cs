/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Slothsoft.Informant.Api;
using Slothsoft.Informant.Implementation.Common;
using StardewValley.TerrainFeatures;

namespace Slothsoft.Informant.Implementation.TooltipGenerator;

internal class FruitTreeTooltipGenerator : ITooltipGenerator<TerrainFeature> {

    private readonly IModHelper _modHelper;
    
    public FruitTreeTooltipGenerator(IModHelper modHelper) {
        _modHelper = modHelper;
    }
    
    public string Id => "fruit-tree";
    public string DisplayName => _modHelper.Translation.Get("FruitTreeTooltipGenerator");
    public string Description => _modHelper.Translation.Get("FruitTreeTooltipGenerator.Description");
    
    public bool HasTooltip(TerrainFeature input) {
        return input is FruitTree;
    }

    public Tooltip Generate(TerrainFeature input) {
        return CreateTooltip((FruitTree) input);
    }

    private Tooltip CreateTooltip(FruitTree fruitTree) {
        var displayName = GameInformation.GetObjectDisplayName(fruitTree.indexOfFruit.Value);
        var daysLeft = CropTooltipGenerator.ToDaysLeftString(_modHelper, CalculateDaysLeft(fruitTree));
        return new Tooltip($"{displayName}\n{daysLeft}") {
            Icon = Icon.ForParentSheetIndex(
                fruitTree.indexOfFruit.Value, 
                false, 
                IPosition.CenterRight,
                new Vector2(Game1.tileSize / 2, Game1.tileSize / 2)
            ),
        };
    }

    internal int CalculateDaysLeft(FruitTree fruitTree) {
        var daysLeft = fruitTree.daysUntilMature.Value;
        if (daysLeft <= 0) {
            // if mature, 0 days are left if there are fruits on the tree, else 1 day
            daysLeft = fruitTree.fruitsOnTree.Value <= 0 ? 1 : 0;
        }
        if (daysLeft > 0) {
            if (fruitTree.currentLocation?.IsGreenhouse ?? false) {
                // if we are in the greenhouse, we don't need to add anything for seasons
                return daysLeft;
            }
            if (fruitTree.currentLocation?.Name.Contains("Island") ?? false) {
                // if we are on the island, we don't need to add anything for seasons
                return daysLeft;
            }
            // check that the date we are calculating is in the correct season   
            var futureDay = Game1.dayOfMonth + daysLeft;
            var seasonsLeft = futureDay / Seasons.LengthInDays;
            futureDay %= Seasons.LengthInDays;

            var futureSeasonIndex = Array.IndexOf(Seasons.All, Game1.currentSeason) + seasonsLeft;
            var futureSeason = Seasons.All[futureSeasonIndex % Seasons.All.Length];
            while (futureSeason != fruitTree.fruitSeason.Value) {
                futureSeasonIndex++;
                futureSeason = Seasons.All[futureSeasonIndex % Seasons.All.Length];
                daysLeft += Seasons.LengthInDays - futureDay; // add only the remainder of the month
                futureDay = 0; // and after the remainder was added, all following months are fully added

                if (daysLeft > Seasons.All.Length * Seasons.LengthInDays) {
                    // daysLeft is now more than one year - which might happen if the fruitSeason is unknown
                    // (or I misspelled "winter" in the seasons constants) -> just ignore this
                    return -1;
                }
            }
        }
        return daysLeft;
    }
}