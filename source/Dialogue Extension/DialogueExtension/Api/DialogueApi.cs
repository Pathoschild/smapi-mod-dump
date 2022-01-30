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
  public class DialogueApi : IDialogueApi
  {
    private readonly IConditionRepository _repository;
    public DialogueApi(IConditionRepository repository) => _repository = repository;

    public void AddCustomHeartLevelCondition(int heartLevel, bool levelAndHigher, Func<DialogueConditions, int, Dialogue> func)
    {
      var key = (heartLevel, levelAndHigher);
      if (!_repository.HeartDialogueDictionary.ContainsKey(key))
        _repository.HeartDialogueDictionary.Add(key, new List<Func<DialogueConditions, int, Dialogue>>());
      _repository.HeartDialogueDictionary[key].Add(func);
    }

    public void AddRangeCustomHeartLevelCondition(IDictionary<(int hearts, bool andHigher), 
      IEnumerable<Func<DialogueConditions, int, Dialogue>>> conditions)
    {
      foreach (var kvp in conditions)
      {
        foreach (var condition in kvp.Value)
        {
          AddCustomHeartLevelCondition(kvp.Key.hearts, kvp.Key.andHigher, condition);
        }
      }
    }
  }
}
