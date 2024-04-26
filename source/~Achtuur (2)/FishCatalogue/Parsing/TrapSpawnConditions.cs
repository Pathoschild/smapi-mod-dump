/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Framework.Borders;
using FishCatalogue.Data;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishCatalogue.Parsing;
internal class TrapSpawnConditions : SpawnConditions
{
    TrapWaterType waterType;
    public TrapSpawnConditions(string fish_data) : base(fish_data)
    {
    }

    public override bool CanSpawn()
    {
        return LocationContainsWaterType();
    }

    private bool LocationContainsWaterType()
    {
        return FishCatalogue.LocationFishAreas[Game1.currentLocation.Name]
            .Any(area => area.waterTypes.Contains(waterType));
    }

    protected override void parse_fish_data(string fish_data)
    {
        List<string> parts = fish_data.Split('/').ToList();
        FishName = parts[0];
        waterType = parts[4] switch
        {
            "ocean" => TrapWaterType.Ocean,
            "freshwater" => TrapWaterType.Freshwater,
            _ => throw new Exception($"Invalid water type: {parts[4]}")
        };
    }

    public override bool CanSpawnThisSeason()
    {
        return true;
    }

    public override IEnumerable<ItemLabel> ConditionsLabel()
    {
        throw new NotImplementedException();
    }
    public override IEnumerable<ItemLabel> UnfulfilledConditionsLabel()
    {
        throw new NotImplementedException();
    }
}
