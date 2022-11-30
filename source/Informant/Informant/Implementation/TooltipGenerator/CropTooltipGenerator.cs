/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using System.Linq;
using Microsoft.Xna.Framework;
using Slothsoft.Informant.Api;
using Slothsoft.Informant.Implementation.Common;
using StardewValley.TerrainFeatures;

namespace Slothsoft.Informant.Implementation.TooltipGenerator;

internal class CropTooltipGenerator : ITooltipGenerator<TerrainFeature> {

    private readonly IModHelper _modHelper;
    
    public CropTooltipGenerator(IModHelper modHelper) {
        _modHelper = modHelper;
    }

    public string DisplayName => _modHelper.Translation.Get("CropTooltipGenerator");
    public string Description => _modHelper.Translation.Get("CropTooltipGenerator.Description");
    
    public string Id => "crop";

    public bool HasTooltip(TerrainFeature input) {
        return input is HoeDirt {crop: { }};
    }

    public Tooltip Generate(TerrainFeature input) {
        return CreateTooltip(_modHelper, ((HoeDirt) input).crop);
    }

    internal static Tooltip CreateTooltip(IModHelper modHelper, Crop crop) {
        // for some reason, ginger is displayed as weeds
        var parentSheetIndex = crop.whichForageCrop.Value == ObjectIds.GingerForageCropId ? ObjectIds.Ginger : crop.indexOfHarvest.Value;
        var displayName = GameInformation.GetObjectDisplayName(parentSheetIndex);
        var daysLeft = CalculateDaysLeftString(modHelper, crop);
        return new Tooltip($"{displayName}\n{daysLeft}") {
            Icon = Icon.ForParentSheetIndex(
                parentSheetIndex, 
                false, 
                IPosition.CenterRight,
                new Vector2(Game1.tileSize / 2f, Game1.tileSize / 2f)
            ),
        };
    }

    internal static string CalculateDaysLeftString(IModHelper modHelper, Crop crop) {
        if (crop.dead.Value) {
            return modHelper.Translation.Get("CropTooltipGenerator.Dead");
        }
        return ToDaysLeftString(modHelper, CalculateDaysLeft(crop));
    }

    internal static string ToDaysLeftString(IModHelper modHelper, int daysLeft) {
        return daysLeft switch {
            -1 => "", // something went very wrong, but we don't want to break the game
            0 => modHelper.Translation.Get("CropTooltipGenerator.0DaysLeft"),
            1 => modHelper.Translation.Get("CropTooltipGenerator.1DayLeft"),
            _ => modHelper.Translation.Get("CropTooltipGenerator.XDaysLeft", new {X = daysLeft})
        };
    }

    internal static int CalculateDaysLeft(Crop crop) {
        var currentPhase = crop.currentPhase.Value;
        var dayOfCurrentPhase = crop.dayOfCurrentPhase.Value;
        var regrowAfterHarvest = crop.regrowAfterHarvest.Value;
        var cropPhaseDays = crop.phaseDays.ToArray();

        // Amaranth:  current = 4 | day = 0 | days = 1, 2, 2, 2, 99999 | result => 0
        // Fairy Rose:  current = 4 | day = 1 | days = 1, 4, 4, 3, 99999 | result => 0
        // Cranberry:  current = 5 | day = 4 | days = 1, 2, 1, 1, 2, 99999 | result => ???
        // Ancient Fruit: current = 5 | day = 4 | days = 1 5 5 6 4 99999 | result => 4
        // Blueberry (harvested): current = 5 | day = 4 | days = 1 3 3 4 2 99999 | regrowAfterHarvest = 4 | result => 4
        // Blueberry (harvested): current = 5 | day = 0 | days = 1 3 3 4 2 99999 | regrowAfterHarvest = 4 | result => 0
        var result = 0;
        for (var phase = currentPhase; phase < cropPhaseDays.Length; phase++) {
            if (cropPhaseDays[phase] < 99999) {
                result += cropPhaseDays[phase];
                if (phase == currentPhase) {
                    result -= dayOfCurrentPhase;
                }
            } else if (currentPhase == cropPhaseDays.Length - 1 && regrowAfterHarvest > 0) {
                // calculate the repeating harvests, it seems the dayOfCurrentPhase counts backwards now
                result = dayOfCurrentPhase;
            }
        }

        return result;
    }
}