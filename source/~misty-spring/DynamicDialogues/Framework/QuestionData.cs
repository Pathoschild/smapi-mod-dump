/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using StardewModdingAPI;
using StardewValley;

namespace DynamicDialogues.Framework;

public class QuestionData : Dialogue
{
  //private string _questToAdd;
  //private bool _isLastDialogueInteractive = false;
  
  private List<NPCDialogueResponse> _playerResponses = new();
  private List<string> _quickResponses = new();
  private List<string> _missionList = new();
  
  public QuestionData(NPC speaker, string dialogueText, string translationKey = null) : base(speaker, translationKey, dialogueText)
    {
        //this.quickResponse = true;
        this.speaker = speaker;
        
        /*if(!string.IsNullOrWhiteSpace(QuestID))
          this.onFinish = new Action(AddQuest);
        
        this._questToAdd = QuestID;*/
        
        try
        {
            parseDialogueString(dialogueText);
            //checkForSpecialDialogueAttributes();
        }
        catch (Exception)
        {
            //IGameLogger log = Game1.log;
            var interpolatedStringHandler = new DefaultInterpolatedStringHandler(56, 3);
            interpolatedStringHandler.AppendLiteral("Failed parsing dialogue string for NPC ");
            interpolatedStringHandler.AppendFormatted(speaker?.Name);
            interpolatedStringHandler.AppendLiteral(" (key: ");
            interpolatedStringHandler.AppendFormatted(translationKey);
            interpolatedStringHandler.AppendLiteral(", text: ");
            interpolatedStringHandler.AppendFormatted(dialogueText);
            interpolatedStringHandler.AppendLiteral(").");
            var stringAndClear = interpolatedStringHandler.ToStringAndClear();
            ModEntry.Mon.Log(stringAndClear,LogLevel.Error);
            parseDialogueString("...");
        }
    }

  protected sealed override void parseDialogueString(string masterString)
    {
      var source = masterString.Split('#');
      if (source[0] != "$qna") return;
      
      var questions = source[1].Split('_');
      var answers = source[2].Split('_');
      var missions = source[3].Split('_');

      //this._isLastDialogueInteractive = true;
        
      _quickResponses ??= new List<string>();
      _playerResponses ??= new List<NPCDialogueResponse>();
      _missionList ??= new List<string>();
        
      foreach (var count in questions)
      {
        var index = GetIndex(questions,count);
          
        _playerResponses.Add(new NPCDialogueResponse(null, -1, "quickResponse" + index.ToString(), Game1.parseText(count)));
        _quickResponses.Add(answers[index]);
        _missionList.Add(missions[index]);
      }
    }
  public override bool chooseResponse(Response response)
  {
      for (var index = 0; index < _playerResponses.Count; ++index)
      {
        if (_playerResponses[index].responseKey == null || response.responseKey == null ||
            !_playerResponses[index].responseKey.Equals(response.responseKey)) continue;
        //get dialogue
        speaker.setNewDialogue(new Dialogue(speaker, null, _quickResponses[index]));
        Game1.drawDialogue(speaker);
          
        //face farmer
        speaker.faceTowardFarmerForPeriod(4000, 3, false, farmer);
          
        //if mission, add
        if (_missionList[index] != "none")
        {
          Game1.player.addQuest(_missionList[index]);
        }
          
        return true;
      }
      return false;
    }
    
    /*
    private void AddQuest()
    {
      Game1.player.addQuest(this._questToAdd);
    }*/
  private static int GetIndex(string[] questions, string which)
    {
      var count = 0;
      foreach (var text in questions)
      {
        if (text != which)
        {
          count++;
        }
        else
        {
          return count;
        }
      }
      throw new KeyNotFoundException();
    }
}