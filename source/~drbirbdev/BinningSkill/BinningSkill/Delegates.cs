/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Globalization;
using BirbCore.Attributes;
using SpaceCore;
using StardewValley;

namespace BinningSkill;

[SDelegate]
internal class Delegates
{
    [SDelegate.GameStateQuery]
    public static bool LEVEL(string[] query, GameLocation location, Farmer player, Item targetItem, Item inputItem, Random random)
    {
        return GameStateQuery.Helpers.PlayerSkillLevelImpl(query, player, (target) => target.GetCustomBuffedSkillLevel("drbirbdev.Binning")); ;
    }

    [SDelegate.GameStateQuery]
    public static bool RANDOM(string[] query, GameLocation location, Farmer player, Item targetItem, Item inputItem, Random random)
    {
        if (!ArgUtility.TryGetFloat(query, 1, out float chance, out string error))
        {
            return GameStateQuery.Helpers.ErrorResult(query, error);
        }
        for (int i = 2; i < query.Length; i++)
        {
            chance = query[i].ToLower(CultureInfo.InvariantCulture) switch
            {
                "@adddailyluck" => chance + (float)Game1.player.DailyLuck,
                "@addbuffluck" => chance + (Game1.player.LuckLevel / 100.0f),
                "@addbinningluck" => chance + (ModEntry.Config.PerLevelRareDropChanceBonus * player.GetCustomBuffedSkillLevel("drbirbdev.Binning")),
                "@addallluck" => chance + (float)Game1.player.DailyLuck + (Game1.player.LuckLevel / 100.0f) + (ModEntry.Config.PerLevelRareDropChanceBonus * player.GetCustomBuffedSkillLevel("drbirbdev.Binning")),
                "@multdailyluck" => chance * (float)(1 + Game1.player.DailyLuck),
                "@multbuffluck" => chance * (1 + (Game1.player.LuckLevel / 100.0f)),
                "@multbinningluck" => chance * (1 + (ModEntry.Config.PerLevelRareDropChanceBonus * player.GetCustomBuffedSkillLevel("drbirbdev.Binning"))),
                "@multallluck" => chance * (float)(1 + (Game1.player.DailyLuck + (Game1.player.LuckLevel / 100.0f) + (ModEntry.Config.PerLevelRareDropChanceBonus * player.GetCustomBuffedSkillLevel("drbirbdev.Binning")))),
                _ => chance
            };
        }
        return random.NextDouble() < (double)chance;
    }
}
