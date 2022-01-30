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
using DialogueExtension.Patches.Utility;
using StardewValley;

namespace DialogueExtension.Api
{
  public interface IDialogueApi
  {
    void AddCustomHeartLevelCondition(
      int heartLevel, bool levelAndHigher, Func<DialogueConditions, int, Dialogue> func);

    void AddRangeCustomHeartLevelCondition(
      IDictionary<(int hearts, bool andHigher), IEnumerable<Func<DialogueConditions, int, Dialogue>>> conditions);
  }
}
