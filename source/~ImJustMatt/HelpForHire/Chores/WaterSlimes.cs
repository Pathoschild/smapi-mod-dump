/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace HelpForHire.Chores;

using System.Collections.Generic;
using System.Linq;
using StardewValley;

internal class WaterSlimes : GenericChore
{
    public WaterSlimes(ServiceLocator serviceLocator)
        : base("water-slimes", serviceLocator)
    {
    }

    protected override bool DoChore()
    {
        var slimesWatered = false;

        foreach (var slimeHutch in WaterSlimes.GetSlimeHutches())
        {
            for (var index = 0; index < slimeHutch.waterSpots.Count; ++index)
            {
                if (slimeHutch.waterSpots[index])
                {
                    continue;
                }

                slimeHutch.waterSpots[index] = true;
                slimesWatered = true;
            }
        }

        return slimesWatered;
    }

    protected override bool TestChore()
    {
        return WaterSlimes.GetSlimeHutches().Any();
    }

    private static IEnumerable<SlimeHutch> GetSlimeHutches()
    {
        return
            from building in Game1.getFarm().buildings
            where building.daysOfConstructionLeft.Value <= 0 && building.indoors.Value is SlimeHutch
            select building.indoors.Value as SlimeHutch;
    }
}