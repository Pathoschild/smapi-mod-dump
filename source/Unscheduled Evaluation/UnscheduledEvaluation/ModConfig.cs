/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/wrongcoder/UnscheduledEvaluation
**
*************************************************/

namespace UnscheduledEvaluation;

public sealed class ModConfig
{
    public bool EnableAlwaysActiveShrinePatch { get; set; } = true;

    public bool SkippableShrineCandles { get; set; } = false;

    public bool SkippableEvaluation1 { get; set; } = false;

    public bool SkippableEvaluation2 { get; set; } = false;

    public string? GrandpaNote { get; set; } =
        "{0}-\n\nWhen you are ready for me, place a diamond on this shrine.\n\n-Grandpa";
}