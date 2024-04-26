/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Framework.Borders;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
namespace FishCatalogue.Parsing;
enum Weather
{
    Sunny,
    Rainy,
    Both,
}

enum TrapWaterType
{
    Ocean,
    Freshwater,
}

public abstract class SpawnConditions
{
    public string FishName;
    private string raw_data;
    public SpawnConditions(string fish_data)
    {
        raw_data = fish_data;
        parse_fish_data(fish_data);
    }

    public abstract bool CanSpawn();
    public abstract IEnumerable<ItemLabel> ConditionsLabel();
    public abstract IEnumerable<ItemLabel> UnfulfilledConditionsLabel();
    protected abstract void parse_fish_data(string fish_data);

    public abstract bool CanSpawnThisSeason();

}


public class SpawnCondtionsFactory
{
    public static SpawnConditions Create(string fish_data)
    {
        // if it's a trap, the second part of the split will contain "trap"
        string trap = fish_data.Split('/')[1];
        if (trap == "trap")
            return new TrapSpawnConditions(fish_data);
        return new FishSpawnConditions(fish_data);
    }
}