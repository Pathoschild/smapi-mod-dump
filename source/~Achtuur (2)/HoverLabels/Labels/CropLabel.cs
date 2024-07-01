/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
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
using StardewValley.Tools;
using System.Diagnostics.Tracing;
using AchtuurCore.Framework.Borders;

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
            return hoeDirt.crop is not null || hoeDirt.HasFertilizer();
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
        if (hoverCrop is null)
        {
            TitleLabel no_crop = new(I18n.LabelCropsNoCrop());
            AddBorder(no_crop);
        } 
        else
        {
            SObject harvestedItem = GetCropAsObject(hoverCrop);
            TitleLabel crop_name = new(harvestedItem.DisplayName);
            AddBorder(crop_name);
        }

        foreach (Border border in GenerateCropLabel(hoverCrop, hoverHoeDirt))
            AddBorder(border);
    }

    public static IEnumerable<Border> GenerateCropLabel(Crop crop, HoeDirt hoeDirt)
    {
        Border crop_state = new();
        crop_state.AddLabel(GenerateCropStateLabel(crop, hoeDirt));
        yield return crop_state;

        Border soil_state = new();
        if (GenerateFertilizerStateLabel(hoeDirt) is Label fertilizer_state)
            soil_state.AddLabel(fertilizer_state);

        if (GenerateSoilStateLabel(crop, hoeDirt) is Label soil_state_label)
            soil_state.AddLabel(soil_state_label);
        yield return soil_state;
    }

    private static Label GenerateSoilStateLabel(Crop crop, HoeDirt hoeDirt)
    {
        if (crop is null || IsCropFullyGrown(crop))
            return null;

        Item watering_can = Game1.player.Items.Where(item => item is WateringCan).FirstOrDefault();
        if (watering_can is null)
            watering_can = ItemRegistry.Create("(T)WateringCan");
        if (hoeDirt.state.Value == 0 && !crop.dead.Value)
            return new ItemLabel(watering_can, I18n.LabelCropsWaterNeeded());
        return null;
    }

    private static Label GenerateFertilizerStateLabel(HoeDirt hoeDirt)
    {
        if (hoeDirt is null)
            return null;

        string fertilizerQID = GetFertilizerQID(hoeDirt.fertilizer.Value);
        if (fertilizerQID.Length > 0)
            return new ItemLabel(fertilizerQID);
        return null;
    }

    private static Label GenerateCropStateLabel(Crop crop, HoeDirt hoeDirt)
    {
        if (crop is null || hoeDirt is null)
            return null;

        if (IsCropFullyGrown(crop))
        {
            return new Label(I18n.LabelCropsReadyHarvest());
        }
        else if (crop.dead.Value)
        {
            return new Label(I18n.LabelCropsDead());
        }
        else // Not fully grown yet
        {
            int days = GetDaysUntilFullyGrown(crop);
            string readyDate = ModEntry.GetDateAfterDays(days);

            if (CropCanFullyGrowInTime(crop, hoeDirt))
                return new Label(I18n.LabelCropsGrowTime(days, readyDate));
            else
                return new Label(I18n.LabelCropsInsufficientTime(readyDate));
        }
    }

    internal static string GetFertilizerQID(string qualified_id)
    {
        if (qualified_id is null || qualified_id.Length == 0)
            return "";

        Item fertilizer_item = ItemRegistry.Create(qualified_id);
        if (fertilizer_item.DisplayName.ToLowerInvariant().Contains("weeds"))
            return "";

        return fertilizer_item.QualifiedItemId;
    }

    internal static bool IsCropFullyGrown(Crop crop)
    {
        return (crop is not null) ? GetDaysUntilFullyGrown(crop) <= 0 : false;
    }

    internal static SObject GetCropAsObject(Crop crop)
    {
        if (crop is null)
            return null;

        if (crop.programColored is not null && crop.indexOfHarvest.Value is not null && crop.tintColor is not null)
            return new ColoredObject(crop.indexOfHarvest.Value, 1, crop.tintColor.Value);

        if (crop.whichForageCrop is not null)
        {
            switch (crop.whichForageCrop.Value)
            {
                case "1": return ItemRegistry.Create<SObject>("399"); // spring onion
                case "2": return ItemRegistry.Create<SObject>("829"); // ginger
            }
        }

        return new SObject(crop.indexOfHarvest.Value, 1, false, -1, 0);
    }

    internal static int GetDaysUntilFullyGrown(Crop crop)
    {
        int currentPhase = crop.currentPhase.Value;
        int dayOfCurrentPhase = crop.dayOfCurrentPhase.Value;

        // regrowing crops use different variable
        if (crop.fullyGrown.Value && crop.RegrowsAfterHarvest())
            return dayOfCurrentPhase;
        // fully grown if current phase is last phase
        else if (currentPhase == crop.phaseDays.Count - 1)
            return 0;
        else if (currentPhase >= crop.phaseDays.Count)
            return 0; // out of bounds -> fully grown?

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
            || cropDirt.Location is null 
            || !cropDirt.Location.IsOutdoors 
            || cropDirt.Location.SeedsIgnoreSeasonsHere())
            return true;

        // current location overrides season -> check if crop survives current season
        // this will (probably) always return true if seasonOverride is set
        if (cropDirt.Location.SeedsIgnoreSeasonsHere())
        {
            return crop.IsInSeason(cropDirt.Location);
        }
        /// Growth would only finish next season -> check if crop can surive next season
        Season location_season = cropDirt.Location.GetSeason();
        int idx = (ModEntry.Seasons.IndexOf(location_season) + 1) % ModEntry.Seasons.Count;
        Season next_season = ModEntry.Seasons[idx];
        return crop.GetData().Seasons.Contains(next_season);
    }

    internal static string ToTitleString(string s)
    {
        return char.ToUpper(s[0]) + s.Substring(1).ToLowerInvariant();
    }
}
