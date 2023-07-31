/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using HoverLabels.Framework;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;
using StardewValley.Objects;

namespace HoverLabels.Labels;
internal class CropLabel : BaseLabel
{

    Crop hoverCrop { get; set; }
    HoeDirt hoverHoeDirt { get; set; }

    public CropLabel(int? priority=null) : base(priority)
    {
    }

    public override bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        if (!Game1.currentLocation.terrainFeatures.ContainsKey(cursorTile))
            return false;

        TerrainFeature terrainFeature = Game1.currentLocation.terrainFeatures[cursorTile];
        if (terrainFeature is HoeDirt hoeDirt)
        {
            return hoeDirt.crop is not null || hoeDirt.fertilizer.Value != HoeDirt.noFertilizer;
        }

        return false;
    }
    public override void SetCursorTile(Vector2 cursorTile)
    {
        base.SetCursorTile(cursorTile);

        if (!ShouldGenerateLabel(cursorTile))
            return;

        this.hoverHoeDirt = Game1.currentLocation.terrainFeatures[cursorTile] as HoeDirt;
        this.hoverCrop = this.hoverHoeDirt.crop;
    }

    public override void GenerateLabel()
    {
        GenerateCropStateLabel();
        GenerateFertilizerStateLabel();
        GenerateSoilStateLabel();
    }

    private void GenerateSoilStateLabel()
    {
        if (this.hoverCrop is null || IsCropFullyGrown(this.hoverCrop))
            return;

        if (this.hoverHoeDirt.state.Value == 0 && !this.hoverCrop.dead.Value)
            this.Description.Add("Needs water!");
    }

    private void GenerateFertilizerStateLabel()
    {
        string fertilizerName = GetFertilizerName(hoverHoeDirt.fertilizer.Value);
        if (fertilizerName.Length > 0)
            this.Description.Add(I18n.LabelCropsFertilizer(fertilizerName));
    }

    private void GenerateCropStateLabel()
    {
        if (hoverCrop is null)
        {
            this.Name = I18n.LabelCropsNoCrop();
            return;
        }

        SObject harvestedItem = GetCropAsObject(hoverCrop);
        this.Name = harvestedItem.DisplayName;

        if (IsCropFullyGrown(hoverCrop))
        {
            Description.Add(I18n.LabelCropsReadyHarvest());

            if (hoverCrop.minHarvest.Value == hoverCrop.maxHarvest.Value)
            {
                Description.Add(I18n.LabelCropsHarvestAmount(hoverCrop.minHarvest.Value));
            }
            else
            {
                Description.Add(I18n.LabelCropsHarvestRange(hoverCrop.minHarvest.Value, hoverCrop.maxHarvest.Value));
            }
        }
        else if (hoverCrop.dead.Value)
        {
            Description.Add(I18n.LabelCropsDead());
        }
        else // Not fully grown yet
        {
            int days = GetDaysUntilFullyGrown(hoverCrop);
            string readyDate = ModEntry.GetDateAfterDays(days);

            if (CropCanFullyGrowInTime(hoverCrop, hoverHoeDirt))
                Description.Add(I18n.LabelCropsGrowTime(days, readyDate));
            else
                Description.Add(I18n.LabelCropsInsufficientTime(readyDate));
        }
    }

    internal static string GetFertilizerName(int id)
    {
        SObject sobj = ModEntry.GetObjectWithId(id);
        if (!sobj.DisplayName.ToLowerInvariant().Contains("weeds"))
            return sobj.displayName;
        return "";
    }

    internal static bool IsCropFullyGrown(Crop crop)
    {
        return GetDaysUntilFullyGrown(crop) <= 0;
    }

    internal static SObject GetCropAsObject(Crop crop)
    {
        if (crop is null)
            return null;

        return crop.programColored.Value 
            ? new ColoredObject(crop.indexOfHarvest.Value, 1, crop.tintColor.Value) 
            : new SObject(crop.indexOfHarvest.Value, 1, false, -1, 0);
    }

    internal static int GetDaysUntilFullyGrown(Crop crop)
    {
        int currentPhase = crop.currentPhase.Value;
        int dayOfCurrentPhase = crop.dayOfCurrentPhase.Value;

        // regrowing crops use different variable
        if (crop.fullyGrown.Value && crop.regrowAfterHarvest.Value != -1)
            return dayOfCurrentPhase;
        // fully grown if current phase is last phase
        else if (currentPhase == crop.phaseDays.Count - 1)
            return 0;

        return crop.phaseDays[currentPhase] - dayOfCurrentPhase // days left in current phase
            + crop.phaseDays.Skip(currentPhase + 1) // go to phase after current phase
            .SkipLast(1) // last phaseDays is 9999, because it should last forever
            .Sum(); // sum remaining days
    }

    /// <summary>
    /// Returns whether <paramref name="crop"/> can fully grow in time for this (or next) season
    /// </summary>
    /// <param name="crop"></param>
    /// <param name="days"></param>
    /// <returns></returns>
    internal static bool CropCanFullyGrowInTime(Crop crop, HoeDirt cropDirt)
    {
        int days = GetDaysUntilFullyGrown(crop);

        // growth fits within current season -> can always grow
        if (Game1.dayOfMonth + days <= 28 
            || cropDirt.currentLocation is null 
            || !cropDirt.currentLocation.IsOutdoors 
            || cropDirt.currentLocation.SeedsIgnoreSeasonsHere())
            return true;

        // current location overrides season -> check if crop survives current season
        // this will (probably) always return true if seasonOverride is set
        if (cropDirt.currentLocation.seasonOverride is not null && cropDirt.currentLocation.seasonOverride != String.Empty)
            return crop.seasonsToGrowIn.Contains(cropDirt.currentLocation.seasonOverride);

        /// Growth would only finish next season -> check if crop can surive next season
        string location_season = cropDirt.currentLocation.GetSeasonForLocation();
        string next_season = ModEntry.Seasons[ModEntry.Seasons.IndexOf(location_season) + 1];
        return crop.seasonsToGrowIn.Contains(next_season);
    }

    internal static string ToTitleString(string s)
    {
        return char.ToUpper(s[0]) + s.Substring(1).ToLowerInvariant();
    }
}
