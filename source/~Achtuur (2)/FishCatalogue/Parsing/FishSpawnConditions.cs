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
using FishCatalogue.Parsing.Conditions;
using FishCatalogue.Queries;
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishCatalogue.Parsing;
internal class FishSpawnConditions : SpawnConditions
{
    private Weather weather;
    /// <summary>
    /// Tuple holds a pair of min and max time
    /// </summary>
    //private List<(int, int)> spawnTimes;
    //private List<Season> seasons;
    private int min_fishing_level;

    private List<BaseCondition> spawnConditions;

    private int minimum_depth;
    private float spawn_multiplier;
    private float depth_multiplier;
    public FishSpawnConditions(string fish_data) : base(fish_data)
    {
    }

    public override bool CanSpawn()
    {
        return spawnConditions.All(condition => condition.IsTrue());
    }

    public override IEnumerable<ItemLabel> ConditionsLabel()
    {
        return spawnConditions.Select(c => c.Label()).Where(l => l is not null);
    }

    public override IEnumerable<ItemLabel> UnfulfilledConditionsLabel()
    {
        return spawnConditions
            .Where(c => !c.IsTrue())
            .Select(c => c.Label())
            .Where(l => l is not null);
    }


    public float GetSpawnChance(int actual_depth)
    {
        //https://stardewvalleywiki.com/Modding:Fish_data#spawn%20rate
        //{spawn multiplier} - max(0, {minimum depth} - {actual depth}) × {depth multiplier} × {spawn multiplier} + {fishing level} / 50
        float depth = Math.Max(0, minimum_depth - actual_depth) * this.depth_multiplier;
        return spawn_multiplier - depth * spawn_multiplier + (float) Game1.player.FishingLevel / 50f;
    }
    public override bool CanSpawnThisSeason()
    {
        SeasonCondition cond = spawnConditions.FirstOrDefault(c => c is SeasonCondition) as SeasonCondition;
        if (cond == null)
            return true;
        return cond.IsTrue();
    }

    protected override void parse_fish_data(string fish_data)
    {
        this.spawnConditions = new();
        List<string> split = fish_data.Split('/').ToList();
        FishName = split[0];
        parse_season(split[6]);
        parse_time(split[5]);

        weather = split[7] switch
        {
            "sunny" => Weather.Sunny,
            "rainy" => Weather.Rainy,
            "both" => Weather.Both,
            _ => throw new Exception($"Invalid weather condition: {split[4]}")
        };
        this.spawnConditions.Add(new WeatherCondition(weather));

        min_fishing_level = int.Parse(split[12]);
    }

    private void parse_time(string time_part)
    {
        // time string is [<int> <int>]+ format.
        // split at spaces, then every two elements are a pair of min and max time
        List<(int, int)> spawnTimes = new();
        List<string> time_parts = time_part.Split(' ').ToList();
        for (int i = 0; i < time_parts.Count; i += 2)
        {
            int min_time = int.Parse(time_parts[i]);
            int max_time = int.Parse(time_parts[i + 1]);
            spawnTimes.Add((min_time, max_time));
        }
        this.spawnConditions.Add(new TimeCondition(spawnTimes));
    }

    private void parse_season(string season_part)
    {
        IEnumerable<Season> seasons = season_part
            .Split(' ')
            .AsEnumerable()
            .Select(season => season switch
            {
                "spring" => Season.Spring,
                "summer" => Season.Summer,
                "fall" => Season.Fall,
                "winter" => Season.Winter,
                _ => throw new Exception($"Invalid season: {season}")
            });
        this.spawnConditions.Add(new SeasonCondition(seasons));
    }

   
}
