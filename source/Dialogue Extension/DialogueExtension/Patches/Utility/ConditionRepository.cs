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
using System.Collections.Generic;
using StardewValley;

namespace DialogueExtension.Patches.Utility
{
  public class ConditionRepository : IConditionRepository
  {
    public ConditionRepository() => HeartDialogueDictionary = 
      new Dictionary<(int hearts, bool andHigher), IList<Func<DialogueConditions, int, Dialogue>>>();

    public IDictionary<(int hearts, bool andHigher), IList<Func<DialogueConditions, int, Dialogue>>> 
      HeartDialogueDictionary { get; set; }
  }
}
