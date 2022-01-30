/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using System;
using Netcode;
using StardewValley;

namespace SDV.Shared.Abstractions
{
  public interface IWorldDateWrapper : IWrappedType<WorldDate>
  {
    int Year { get; set; }
    int SeasonIndex { get; }
    int DayOfMonth { get; set; }
    DayOfWeek DayOfWeek { get; }
    string Season { get; set; }
    int TotalDays { get; set; }
    int TotalWeeks { get; }
    int TotalSundayWeeks { get; }
    NetFields NetFields { get; }
    string Localize();
    string ToString();
  }
}