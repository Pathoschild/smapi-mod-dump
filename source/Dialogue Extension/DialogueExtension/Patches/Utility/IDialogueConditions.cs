/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using SDV.Shared.Abstractions;
using StardewValley;

namespace DialogueExtension.Patches.Utility
{
  public interface IDialogueConditions
  {
    INPCWrapper Npc { get; }
    NPC BaseNpc { get; }
    string Year { get; }
    int FirstOrSecondYear { get; }
    Season Season { get; }
    int DayOfMonth { get; }
    DayOfWeek DayOfWeek { get; }
    string Inlaw { get; }
    int Friendship { get; }
    int Hearts { get; }
  }
}