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
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoverLabels.Labels.Buildings;
internal class SiloLabel : BuildingLabel
{
    public override bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        return Game1.currentLocation.IsFarm
            && Game1.currentLocation.IsOutdoors
            && Game1.getFarm().buildings.Any(b => b.buildingType.Value == "Silo" && b.GetRect().Contains(cursorTile));
    }
    public override void GenerateLabel()
    {
        base.GenerateLabel();
        int maxHay = Utility.numSilos() * 240;
        Description.Add($"Hay: {Game1.getFarm().piecesOfHay.Value}/{maxHay}");
    }
}
