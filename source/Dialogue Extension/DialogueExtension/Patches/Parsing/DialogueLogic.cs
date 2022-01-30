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
using SDV.Shared.Abstractions;
using SDV.Shared.Abstractions.Utility;
using StardewModdingAPI;
using StardewValley;
using DayOfWeek = DialogueExtension.Patches.Utility.DayOfWeek;

namespace DialogueExtension.Patches.Parsing
{
  public class DialogueLogic : IDialogueLogic
  {
    private readonly IConditionRepository _conditionRepository;
    private IMonitor _logger;
    private dynamic _gameWrapper;
    private IWrapperFactory _factory;

    public DialogueLogic(IConditionRepository conditionRepository, IMonitor logger, IWrapperFactory factory)
    {
      _factory = factory;
      _conditionRepository = conditionRepository;
      _logger = logger;
      _gameWrapper = _factory.CreateInstance<IGameWrapper>();
    }

    public IDialogueWrapper GetDialogue(ref INPCWrapper npc, bool useSeason)
    {
      var checkInlaw = new List<bool>() {false};
      if (_gameWrapper.player.spouse != string.Empty)
        checkInlaw.Insert(0, true);
      foreach (var isMarried in checkInlaw)
      {
        var season = Season.Unknown;
        if (!string.IsNullOrEmpty(_gameWrapper.currentSeason))
          if (!Enum.TryParse<Season>(_gameWrapper.currentSeason, true, out season)) continue;

        var conditions = new DialogueConditions(
          npc,
          _gameWrapper.year.ToString("00"),
          useSeason ? season : Season.Unknown,
          _gameWrapper.dayOfMonth,
          ParseDayOfWeek(npc, _gameWrapper.shortDayNameFromDayOfSeason(_gameWrapper.dayOfMonth)),
          isMarried ? MarriageAppend(_gameWrapper.player.spouse) : string.Empty,
          _gameWrapper.player.friendshipData.TryGetValue(npc.Name, out IFriendshipWrapper friendshipValue)
            ? friendshipValue.Points
            : 0);

        IDialogueWrapper dialogue;
        if (FirstPassDialogue(conditions, out dialogue)) return dialogue;
        if (HeartDialogue(conditions, out dialogue)) return dialogue;
        if (NullIfTruePass(conditions)) continue;
        if (LastPassDialogue(conditions, out dialogue)) return dialogue;
      }

      return null;
    }

    private bool NullIfTruePass(DialogueConditions conditions)
    {
      if (!conditions.Npc.Dialogue.ContainsKey(FluentDialogueBuilder
        .New(conditions).Season().DayOfWeek().Married().Build(_logger))) return true;

      return false;
    }

    private bool LastPassDialogue(DialogueConditions conditions, out IDialogueWrapper dialogue)
    {
      if (conditions.Npc.Name.Equals("Caroline") && _gameWrapper.isLocationAccessible("CommunityCenter") &&
          (conditions.Season == Season.Summer && conditions.DayOfWeek == DayOfWeek.Mon))
      {
        dialogue = _factory.CreateInstance<IDialogueWrapper>(
          conditions.Npc.Dialogue["summer_Wed"], conditions.Npc);
        return true;
      }

      if (CheckIfDialogueContainsKey(conditions.Npc,
        FluentDialogueBuilder.New(conditions).Season().DayOfWeek().FirstOrSecondYear().Married().Build(_logger),
        out dialogue)) return true;

      if (CheckIfDialogueContainsKey(conditions.Npc,
        FluentDialogueBuilder.New(conditions).Season().DayOfWeek().Married().Build(_logger),
        out dialogue)) return true;

      dialogue = null;
      return false;
    }

    public bool CheckIfDialogueContainsKey(INPCWrapper npc, string key,
      out IDialogueWrapper dialogue, Func<bool> extraConditions = null)
    {
      if (npc.Dialogue.ContainsKey(key) && (extraConditions?.Invoke() ?? true))
      {
        dialogue = _factory.CreateInstance<IDialogueWrapper>(npc.Dialogue[key], npc);
        return true;
      }

      dialogue = null;
      return false;
    }

    private string MarriageAppend(string spouse) =>
      !string.IsNullOrEmpty(spouse) ? $"_inlaw_{spouse}" : string.Empty;

    private DayOfWeek ParseDayOfWeek(INPCWrapper npc, string day)
    {
      if (npc.Name == "Pierre" &&
          (_gameWrapper.isLocationAccessible("CommunityCenter") ||
           _gameWrapper.player.HasTownKey) && day == "Wed")
        day = "Sat";

      return (DayOfWeek) Enum.Parse(typeof(DayOfWeek), day);
    }

    private bool FirstPassDialogue(DialogueConditions conditions, out IDialogueWrapper dialogue)
    {
      if (conditions.Season == Season.Spring && conditions.DayOfMonth ==12)
        Console.Write(string.Empty);
      foreach (var useYear in new[] {false, true})
        if (CheckIfDialogueContainsKey(conditions.Npc,
          FluentDialogueBuilder.New(conditions).Season().DayOfMonth().Married().FirstOrSecondYear(useYear).Build(_logger),
          out dialogue, () => conditions.FirstOrSecondYear == 1))
          return true;
      dialogue = null;
      return false;
    }

    private bool HeartDialogue(DialogueConditions conditions, out IDialogueWrapper dialogue)
    {
      {
        for (var i = 14; i > 0; i--)
        {
          if (_conditionRepository.HeartDialogueDictionary.ContainsKey((i, true)))
            foreach (var func in _conditionRepository.HeartDialogueDictionary[(i, true)])
            {
              dialogue = _factory.CreateInstance<IDialogueWrapper>(func(conditions, i));
              if (dialogue != null) return true;
            }

          if (conditions.Hearts >= i && CheckIfDialogueContainsKey(conditions.Npc,
            FluentDialogueBuilder.New(conditions).Season().DayOfWeek().Hearts(i).FirstOrSecondYear().Married().Build(_logger),
            out dialogue)) return true;
          if (conditions.Hearts >= i && CheckIfDialogueContainsKey(conditions.Npc,
            FluentDialogueBuilder.New(conditions).Season().DayOfWeek().Hearts(i).Married().Build(_logger),
            out dialogue)) return true;
        }

        dialogue = null;
        return false;
      }
    }
  }
}