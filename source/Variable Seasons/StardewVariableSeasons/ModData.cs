/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/calebstein1/StardewVariableSeasons
**
*************************************************/

using StardewValley;

namespace StardewVariableSeasons
{
    public sealed class ModData
    {
        public int NextSeasonChange { get; init; }
        public int CropSurvivalCounter { get; init; }
        public Season SeasonByDay { get; init; }
    }
    
    public sealed class ModDataLegacy
    {
        public int NextSeasonChange { get; init; }
        public int CropSurvivalCounter { get; init; }
        public string SeasonByDay { get; init; }
    }
}