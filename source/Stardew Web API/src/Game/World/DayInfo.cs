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
using StardewWebApi.Game.NPCs;

namespace StardewWebApi.Game.World;

public class DayInfo
{
    public DayInfo(Date date, string weather)
    {
        Date = date;
        Weather = weather;

        var festivalDateKey = $"{date.Season.ToString().ToLower()}{date.Day}";
        if (DataLoader.Festivals_FestivalDates(Game1.content).ContainsKey(festivalDateKey))
        {
            Festival = DataLoader.Festivals_FestivalDates(Game1.content)[festivalDateKey];
        }
    }

    public Date Date { get; }
    public string Weather { get; }
    public string? Festival { get; }

    public static DayInfo Today => new(Date.Today, Game1.getFarm().GetWeather().Weather);

    public IEnumerable<NPCStub> Birthdays => NPCUtilities.GetNPCsByBirthday(Date.Season, Date.Day)
        .Select(n => n.CreateStub());
}