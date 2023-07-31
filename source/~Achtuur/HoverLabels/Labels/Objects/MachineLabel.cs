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
using HoverLabels.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;
using StardewValley.Objects;

namespace HoverLabels.Labels.Objects;
internal class MachineLabel : ObjectLabel
{
    public MachineLabel(int? priority = null) : base(priority)
    {
    }

    /// <summary>
    /// Returns whether a new label should be created based on <paramref name="cursorTile"/>
    /// </summary>
    /// <param name="cursorTile"></param>
    /// <returns></returns>
    public override bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        SObject sobj = GetCursorObject(cursorTile);

        return sobj is not null
            && sobj.Name != "Stone" //stone in the mines has a nonzero minutesuntilready for some godforsaken reason
            && (sobj.MinutesUntilReady > 0 || sobj.readyForHarvest.Value);
    }

    /// <inheritdoc/>
    public override void GenerateLabel()
    {
        base.GenerateLabel();

        SObject processingItem = hoverObject.heldObject.Value;

        if (processingItem is null)
            return;

        if (hoverObject.readyForHarvest.Value)
        {
            //display either "yx" (where y is number of items > 2) or ""
            if (processingItem.Stack == 1)
                Description.Add(I18n.LabelMachineSingleItemReady(processingItem.DisplayName));
            else
                Description.Add(I18n.LabelMachineMultipleItemsReady(processingItem.DisplayName, processingItem.Stack));

            string quality_string = GetQualityString(processingItem.Quality);
            Description.Add(I18n.LabelMachineQuality(quality_string));
        }
        else
        {
            string duration = GetTimeString(hoverObject.MinutesUntilReady);
            Description.Add(I18n.LabelMachineCrafting(processingItem.DisplayName));
            Description.Add(I18n.LabelMachineReadyIn(duration));
        }
    }

    internal static string GetQualityString(int quality)
    {
        switch (quality)
        {
            case 0: return I18n.NormalQuality();
            case 1: return I18n.SilverQuality();
            case 2: return I18n.GoldQuality();
            case 4: return I18n.IridiumQuality();
            default: return I18n.NormalQuality();
        }
    }

    internal static string GetTimeString(int minutes)
    {
        //days in stardew are 1600 minutes and not 1440 minutes. Hours from 2am to 6am are 100 minutes long for whatever reason.
        int days = minutes >= 1600 ? minutes / 1600 : 0;
        minutes %= 1600;
        int hours = minutes >= 60 ? minutes / 60 : 0;
        minutes %= 60;

        string time = "";

        if (days > 0)
        {
            time += $"{days}d" + (minutes > 0 || hours > 0 ? " " : "");
        }

        if (hours > 0)
        {
            time += $"{hours}h" + (minutes > 0 ? " " : "");
        }

        if (minutes > 0 || days == 0 && hours == 0)
        {
            time += $"{minutes}m";
        }
        return time;
    }

}
