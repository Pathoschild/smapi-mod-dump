/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using System.Collections.Generic;
using DialogueExtension.Patches.Utility;

namespace DialogueExtension.Tests
{
  public static partial class MockData
  {
    public static IEnumerable<int> DaysOfMonth =>
      new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28};

    public static IEnumerable<DayOfWeek> DaysOfWeek => new[]
    {
      DayOfWeek.Mon,
      DayOfWeek.Tue,
      DayOfWeek.Wed,
      DayOfWeek.Thu,
      DayOfWeek.Fri,
      DayOfWeek.Sat,
      DayOfWeek.Sun
    };

    public static DayOfWeek NextDay(this DayOfWeek currentDay)
    {
      switch (currentDay)
      {
        case DayOfWeek.Mon:
          return DayOfWeek.Tue;
        case DayOfWeek.Tue:
          return DayOfWeek.Wed;
        case DayOfWeek.Wed:
          return DayOfWeek.Thu;
        case DayOfWeek.Thu:
          return DayOfWeek.Fri;
        case DayOfWeek.Fri:
          return DayOfWeek.Sat;
        case DayOfWeek.Sat:
          return DayOfWeek.Sun;
        case DayOfWeek.Sun:
          return DayOfWeek.Mon;
        default:
          return DayOfWeek.Unknown;
      }
    }

    public static IEnumerable<int> Friendships => 
      new[] {250, 500, 750, 1000, 1250, 1500, 1750, 2000, 2250, 2500, 2750, 3000, 3250, 3500};

    public static IEnumerable<Season> Seasons => new[]
    {
      Season.Spring,
      Season.Summer,
      Season.Fall,
      Season.Winter,
      Season.Unknown
    };

    public static IEnumerable<int> Years => new[]
    {
      1, 2, 3, 4, 5, 6, 7, 8, 9, 10
    };

    public static IEnumerable<string> NpcNames => new[]
    {
      "Abigail",
      "Alex",
      "Caroline",
      "Clint",
      "Demetrius",
      "Dwarf",
      "Elliott",
      "Emily",
      "Evelyn",
      "George",
      "Gil",
      "Gus",
      "Haley",
      "Harvey",
      "Jas",
      "Jodi",
      "Kent",
      "Krobus",
      "Leah",
      //"Leo",
      "LeoMainland",
      "Lewis",
      "Linus",
      "Marnie",
      "Maru",
      "Mister Qi",
      "Pam",
      "Penny",
      "Pierre",
      "Robin",
      "Sam",
      "Sandy",
      "Sebastian",
      "Shane",
      "Vincent",
      "Willy",
      "Wizard"
    };

    public static IEnumerable<string> MarriageNpcNames => new[]
    {
      string.Empty, // Not married condition
      "Abigail",
      "Alex",
      "Elliott",
      "Emily",
      "Haley",
      "Harvey",
      "Krobus",
      "Leah",
      "Maru",
      "Penny",
      "Sam",
      "Sebastian",
      "Shane",
    };
  }
}