/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Globalization;
using BirbCore.Attributes;
using SpaceCore;
using StardewValley;
using StardewValley.Delegates;

namespace BinningSkill;

[SDelegate]
internal class Delegates
{
    [SDelegate.GameStateQuery]
    public static bool Level(string[] query, GameStateQueryContext context)
    {
        return GameStateQuery.Helpers.PlayerSkillLevelImpl(query, context.Player,
            (target) => target.GetCustomBuffedSkillLevel("drbirbdev.Binning"));
    }

    [SDelegate.GameStateQuery]
    public static bool Random(string[] query, GameStateQueryContext context)
    {
        if (!ArgUtility.TryGetFloat(query, 1, out float chance, out string error))
        {
            return GameStateQuery.Helpers.ErrorResult(query, error);
        }

        for (int i = 2; i < query.Length; i++)
        {
            chance = query[i].ToLower(CultureInfo.InvariantCulture) switch
            {
                "@adddailyluck" => chance + (float)context.Player.DailyLuck,
                "@addbuffluck" => chance + (context.Player.LuckLevel / 100.0f),
                "@addbinningluck" => chance + (ModEntry.Config.PerLevelRareDropChanceBonus *
                                               context.Player.GetCustomBuffedSkillLevel("drbirbdev.Binning")),
                "@addallluck" => chance + (float)context.Player.DailyLuck + (context.Player.LuckLevel / 100.0f) +
                                 (ModEntry.Config.PerLevelRareDropChanceBonus *
                                  context.Player.GetCustomBuffedSkillLevel("drbirbdev.Binning")),
                "@multdailyluck" => chance * (float)(1 + context.Player.DailyLuck),
                "@multbuffluck" => chance * (1 + (context.Player.LuckLevel / 100.0f)),
                "@multbinningluck" => chance * (1 + (ModEntry.Config.PerLevelRareDropChanceBonus *
                                                     context.Player.GetCustomBuffedSkillLevel("drbirbdev.Binning"))),
                "@multallluck" => chance * (float)(1 + (context.Player.DailyLuck + (context.Player.LuckLevel / 100.0f) +
                                                        (ModEntry.Config.PerLevelRareDropChanceBonus *
                                                         context.Player
                                                             .GetCustomBuffedSkillLevel("drbirbdev.Binning")))),
                _ => chance
            };
        }

        return context.Random.NextDouble() < chance;
    }
}
