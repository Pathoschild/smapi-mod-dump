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

namespace DialogueExtension.Patches.Parsing
{
  public interface IDialogueLogic
  {
    IDialogueWrapper GetDialogue(ref INPCWrapper npc, bool useSeason);
    bool CheckIfDialogueContainsKey(INPCWrapper npc, string key, out IDialogueWrapper dialogue, Func<bool> extraConditions = null);
  }
}