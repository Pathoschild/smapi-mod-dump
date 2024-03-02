/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

namespace NoFestivalTimer;

public class ExclusionData
{
    public bool IgnoreTimer { get; set; }
    public int OnScore { get; set; }
    public bool Props { get; set; }

    public ExclusionData()
    {
    }

    public ExclusionData(bool ignoreTimer, int minScore, bool useProps)
    {
        IgnoreTimer = ignoreTimer;
        OnScore = minScore;
        Props = useProps;
    }
}