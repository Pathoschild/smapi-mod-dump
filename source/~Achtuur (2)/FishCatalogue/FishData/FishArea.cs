/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using FishCatalogue.Parsing;
using StardewValley.GameData.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishCatalogue.Data;
internal struct FishArea
{
    public string DisplayName;
    public string Location;
    public List<TrapWaterType> waterTypes;
    public float JunkChance;

    public FishArea(string loc, FishAreaData fish_area_data)
    {
        this.Location = loc;
        this.DisplayName = fish_area_data.DisplayName;
        this.waterTypes = fish_area_data.CrabPotFishTypes.Select(x => x switch
        {
            "ocean" => TrapWaterType.Ocean,
            "freshwater" => TrapWaterType.Freshwater,
            _ => throw new Exception($"Invalid water type: {x}")
        }).ToList();

        this.JunkChance = fish_area_data.CrabPotJunkChance;
    }
}
