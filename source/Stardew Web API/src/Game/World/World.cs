/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

using StardewValley;

namespace StardewWebApi.Game.World;

public class World
{
    private World(DayInfo today, string farmName, string currentMusicTrack)
    {
        Today = today;
        FarmName = farmName;
        CurrentMusicTrack = currentMusicTrack;
    }

    public static World Current => new(
        DayInfo.Today,
        Game1.getFarm().DisplayName,
        Game1.getMusicTrackName()
    );

    public DayInfo Today { get; }
    public string FarmName { get; }
    public string CurrentMusicTrack { get; }
}