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
using SDV.Shared.Abstractions;

namespace DialogueExtension.Patches.Utility
{
  // ReSharper disable once UnusedMember.Global
  public class VanillaDialogueParser : IDialogueParser
  {
    private INPCWrapper _npc;

    /*
    "<Season>_" +
    "<DayOfWeek>||<DayOfMonth>" +
    "<Hearts>||<Friendship>" +
    "<FirstOrSecondYear>||<Year>" +
    "<Inlaw>";
    */

    private string _workingString;

    public VanillaDialogueParser(ref INPCWrapper npc, string dialogueKey)
    {
      _workingString = dialogueKey;
      _npc = npc;
    }

    public DialogueConditions GetConditions()
    {
      var season = GetSeason();
      var dayOfWeek = GetDayOfWeek();
      var inlaw = GetInlaw();
      var year = GetYear();
      var dayOfMonth = GetDayOfMonth(dayOfWeek);
      var friendship = GetFriendship(dayOfWeek);

      return new DialogueConditions(
        _npc,
        year,
        season,
        dayOfMonth,
        dayOfWeek,
        inlaw,
        friendship);
    }

    private int GetFriendship(DayOfWeek dayOfWeek)
    {
      if (dayOfWeek != DayOfWeek.Unknown || !int.TryParse(_workingString, out var hearts)) return -1;
      _workingString = string.Empty;
      var friendship = hearts > 14 ? hearts : hearts * 250;
      return friendship;
    }

    private int GetDayOfMonth(DayOfWeek dayOfWeek)
    {
      if (dayOfWeek == DayOfWeek.Unknown && int.TryParse(_workingString, out var day))
      {
        _workingString = string.Empty;
        return day;
      }

      return -1;
    }

    private string GetYear()
    {
      if (!_workingString.Contains("_")) return string.Empty;
      var index = _workingString.IndexOf("_", StringComparison.Ordinal);
      var yearString = _workingString.Substring(index + 1);
      if (!int.TryParse(yearString, out _)) return string.Empty;
      _workingString = _workingString.Replace("_" + yearString, string.Empty);
      return yearString;
    }

    private string GetInlaw()
    {
      if (!_workingString.Contains("_inlaw_")) return string.Empty;
      var index = _workingString.IndexOf("_inlaw_", StringComparison.Ordinal);
      var inlawString = _workingString.Substring(index);
      _workingString = _workingString.Replace(inlawString, string.Empty);
      return inlawString;
    }

    private DayOfWeek GetDayOfWeek()
    {
      foreach (var day in new[]
        {DayOfWeek.Mon, DayOfWeek.Tue, DayOfWeek.Wed, DayOfWeek.Thu, DayOfWeek.Fri, DayOfWeek.Sat, DayOfWeek.Sun})
      {
        if (!_workingString.Contains(day.ToString())) continue;
        _workingString = _workingString.Replace(day.ToString(), string.Empty);
        return day;
      }

      return DayOfWeek.Unknown;
    }

    private Season GetSeason()
    {
      foreach (var season in new[] {Season.Spring, Season.Summer, Season.Fall, Season.Winter})
      {
        if (!_workingString.Contains(season.ToString())) continue;
        _workingString = _workingString.Replace(season + "_", string.Empty);
        return season;
      }

      return Season.Unknown;
    }
  }
}