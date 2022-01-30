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
using StardewValley;

namespace DialogueExtension.Patches.Utility
{
  public struct DialogueConditions : IDialogueConditions
  {
    public DialogueConditions(NPC npc, string year, Season season, int dayOfMonth,
      DayOfWeek dayOfWeek, string inlaw, int friendship) : this(new NPCWrapper(npc), year,
      season, dayOfMonth, dayOfWeek, inlaw, friendship) { }

    public DialogueConditions(INPCWrapper npc, string year, Season season, int dayOfMonth, 
      DayOfWeek dayOfWeek, string inlaw, int friendship)
    {
      Npc = npc;
      _year = "1";
      Season = season;
      _dayOfMonth = 1;
      DayOfWeek = dayOfWeek;
      Inlaw = inlaw;
      _friendship = 0;

      // Struct restriction: all values must be set before conditional code can be run
      Year = year;
      DayOfMonth = dayOfMonth;
      Friendship = friendship;
    }

    private string _year;
    private int _dayOfMonth;
    private int _friendship;

    public INPCWrapper Npc { get; }
    public NPC BaseNpc => (NPC)Npc.GetBaseType;
    public string Year
    {
      get => _year;
      private set
      {
        if (!int.TryParse(value, out var year) || (year <= 0 && year != -1))
          throw new ArgumentException($"Value '{value}' is not a non-zero positive integer or -1 if unknown");
        _year = value;
      }
    }

    public int FirstOrSecondYear =>
      int.Parse(Year) == 1 ? 1 : 2;

    public Season Season { get; }

    public int DayOfMonth
    {
      get => _dayOfMonth;
      private set
      {
        if ((value != -1 && value < 1) || value > 28)
          throw new ArgumentException(
            $"Value '{value}' is not a valid day of month. Allowed values: 1-28 or -1 if unknown");
        _dayOfMonth = value;
      }
    }

    public DayOfWeek DayOfWeek { get; }
    public string Inlaw { get; }

    public int Friendship
    {
      get => _friendship;
      private set
      {
        if ((value == -1 || value >= 0) && value <= 3500)
          _friendship = value;
      }
    }

    public int Hearts => Friendship >= 0 ? Friendship / 250 : -1;
  }
}