/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace HoverLabels.Labels.Objects;
internal class Casklabel : ObjectLabel
{
    Cask hoverCask;
    public override bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        SObject sobj = ObjectLabel.GetCursorObject(cursorTile);
        return sobj is not null && sobj is Cask;
    }

    public override void SetCursorTile(Vector2 cursorTile)
    {
        base.SetCursorTile(cursorTile);
        hoverCask = this.hoverObject as Cask;
    }

    public override void GenerateLabel()
    {
        base.GenerateLabel();

        if (hoverCask.readyForHarvest.Value)
            GenerateReadyLabel();
        else if (hoverCask.heldObject.Value is not null)
            GenerateProcessingLabel(); 
    }

    private void GenerateReadyLabel()
    {
        SObject heldItem = hoverCask.heldObject.Value;
        this.Description.Add(I18n.LabelMachineSingleItemReady(heldItem.DisplayName));
    }

    private void GenerateProcessingLabel()
    {
        // get days needed to reach next quality
        float aging_rate = hoverCask.agingRate.Value;
        int nextQuality = hoverCask.GetNextQuality(hoverCask.heldObject.Value.Quality);
        float daysToMature = hoverCask.daysToMature.Value - hoverCask.GetDaysForQuality(nextQuality);
        int days_to_next_quality = (int)Math.Ceiling(daysToMature / aging_rate);

        // Add time to next quality to description
        string nextQualityReadyDate = ModEntry.GetDateAfterDays(days_to_next_quality);
        string nextQualityString = MachineLabel.GetQualityString(nextQuality);
        this.Description.Add(I18n.LabelCaskProcessing(days_to_next_quality, nextQualityString, nextQualityReadyDate));

        // Add time until iridium quality to description
        if (nextQuality != 4)
        {
            int days_to_iridium = (int)Math.Ceiling(hoverCask.daysToMature.Value / aging_rate);
            string iridiumReadyDate = ModEntry.GetDateAfterDays(days_to_iridium);
            string iridiumQualityString = I18n.IridiumQuality();
            this.Description.Add(I18n.LabelCaskProcessing(days_to_iridium, iridiumQualityString, iridiumReadyDate));
        }
    }
}
