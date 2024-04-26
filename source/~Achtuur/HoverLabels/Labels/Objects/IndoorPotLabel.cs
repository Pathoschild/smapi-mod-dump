/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Extensions;
using HarmonyLib;
using HoverLabels.Drawing;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace HoverLabels.Labels.Objects;
internal class IndoorPotLabel : ObjectLabel
{
    IndoorPot hoverPot;
    HoeDirt hoverHoeDirt;
    Crop hoverPotCrop;

    public IndoorPotLabel(int? priority = null) : base(priority)
    {
    }

    public override bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        if (Game1.currentLocation.isObjectAtTile(cursorTile))
        {
            SObject sobj = Game1.currentLocation.getObjectAtTile(cursorTile);
            return sobj is not null && sobj is IndoorPot;
        }
        return false;
    }

    public override void GenerateLabel()
    {
        base.GenerateLabel();

        hoverPot = hoverObject as IndoorPot;
        hoverHoeDirt = hoverPot.hoeDirt.Value;
        hoverPotCrop = hoverHoeDirt.crop;

        SetCropTitle();
        foreach (Border border in CropLabel.GenerateCropLabel(hoverPotCrop, hoverHoeDirt))
            AddBorder(border);
    }

    /// <summary>
    /// Changes title from "Garden Pot" to "Garden Pot (<c>crop_display_name</c>)"
    /// </summary>
    private void SetCropTitle()
    {
        if (this.hoverPotCrop is null)
            return;

        ResetBorders();
        SObject harvestedItem = hoverPotCrop.programColored.Value ? new ColoredObject(hoverPotCrop.indexOfHarvest.Value, 1, hoverPotCrop.tintColor.Value) : new SObject(hoverPotCrop.indexOfHarvest.Value, 1, false, -1, 0);
        string title = $"{hoverObject.DisplayName} ({harvestedItem.DisplayName})";
        AddBorder(new TitleLabelText(title));
    }
}
