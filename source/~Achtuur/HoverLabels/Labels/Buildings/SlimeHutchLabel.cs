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
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoverLabels.Labels.Buildings;
internal class SlimeHutchLabel : BuildingLabel
{
    /// <summary>
    /// Max number of slimes in slime hutch, hardcoded in the original code to 20
    /// </summary>
    const int SlimeHutchCapacity = 20;

    SlimeHutch hoverHutch;

    public override bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        return ModEntry.IsPlayerOnFarm()
            && Game1.getFarm().buildings.Any(b => b.indoors.Value is SlimeHutch && b.GetRect().Contains(cursorTile));
    }

    public override void SetCursorTile(Vector2 cursorTile)
    {
        base.SetCursorTile(cursorTile);
        this.hoverHutch = this.hoverBuilding.indoors.Value as SlimeHutch;
    }

    public override void GenerateLabel()
    {
        base.GenerateLabel();

        IEnumerable<NPC> slimes = this.hoverHutch.characters.Where(npc => npc is GreenSlime);

        if (slimes.Count() < 0)
            return;

        Dictionary<string, int> slimesByType = slimes
            .GroupBy(slime => slime.displayName)
            .ToDictionary(group => group.Key, group => group.Count());

        this.Description.Add($"Slime occupancy: {slimes.Count()}/{SlimeHutchCapacity}");
        foreach((string typeName, int slime_count) in slimesByType.OrderBy(type => type.Key).Select(x => (x.Key, x.Value)))
        {
            this.Description.Add($"> {typeName}: {slime_count}");
        }
    }
}
