/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Extensions;
using AchtuurCore.Framework.Borders;
using HarmonyLib;
using HoverLabels.Framework;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoverLabels.Labels.Buildings;
internal class BuildingLabel : BaseLabel
{
    protected Building hoverBuilding;

    public BuildingLabel(int? priority = null) : base(priority)
    {
    }

    public override bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        return Game1.currentLocation.IsFarm
            && Game1.currentLocation.IsOutdoors
            && Game1.getFarm().buildings.Any(b => b.GetRect().Contains(cursorTile));
    }

    public override void SetCursorTile(Vector2 cursorTile)
    {
        hoverBuilding = Game1.getFarm().buildings.Where(b => b.GetRect().Contains(cursorTile)).First();
    }
    public override void GenerateLabel()
    {
        if (hoverBuilding is null)
            return;

        AddBorder(new TitleLabel(hoverBuilding.buildingType.Value));

        int daysLeft = hoverBuilding.daysOfConstructionLeft.Value; // alias for easier to read code here
        if (daysLeft > 0)
        {
            string text = I18n.LabelBuildingFinishedIn(daysLeft, ModEntry.GetDateAfterDays(daysLeft));
            AddBorder(new Label(text));
        }

    }

    protected static IEnumerable<Building> GetFarmBuildings()
    {
        return Game1.getFarm().buildings;
    }
}
