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
using System.Linq;
using StardewModdingAPI;
using StardewValley;

namespace DynamicDialogues;

public static class DialoguePatches
{
    internal static bool parseDialogueString_prefix(ref Dialogue instance, string masterString)
    {
      ModEntry.Mon.Log("masterString: " +masterString,LogLevel.Info);
      if (!masterString.Contains("$n"))
        return true;
      
      masterString ??= "...";
      instance.temporaryDialogue = null;

      var playerResponses = ModEntry.Help.Reflection.GetField<List<NPCDialogueResponse>>(typeof(Dialogue), "playerResponses");
      if (playerResponses.GetValue() != null)
        playerResponses.GetValue().Clear();
        
      var source = masterString.Split('#');
      for (var count = 0; count < source.Length; ++count)
      {
        if (source[count].Length >= 2) 
        { 
          source[count] = instance.checkForSpecialCharacters(source[count]); 
          string str1;
          try
          {
            str1 = source[count].Substring(0, 2);
          }
          catch (Exception)
          {
            str1 = "     ";
          }

          switch (str1)
          {
            case "$e":
              continue;
            case "$b":
              if (instance.dialogues.Count > 0)
              {
                instance.dialogues[^1] += "{";
              }
              continue;
            case "$k":
              var dtbk = ModEntry.Help.Reflection.GetField<bool>(typeof(Dialogue), "dialogueToBeKilled");
              dtbk.SetValue(true);
              continue;
            case "$1" when ArgUtility.SplitBySpaceAndGet(source[count], 1) != null:
            {
              var str2 = ArgUtility.SplitBySpaceAndGet(source[count], 1);
              if (instance.farmer.mailReceived.Contains(str2))
              {
                count += 3;
                if (count < source.Length)
                {
                  source[count] = instance.checkForSpecialCharacters(source[count]);
                  instance.dialogues.Add(source[count]);
                  continue;
                }
                continue;
              }
              source[count + 1] = instance.checkForSpecialCharacters(source[count + 1]);
              instance.dialogues.Add(str2 + "}" + source[count + 1]);
              return true; //might cause some bug later on
            }
            case "$c" when ArgUtility.SplitBySpaceAndGet(source[count], 1) != null:
            {
              var chance = Convert.ToDouble(ArgUtility.SplitBySpaceAndGet(source[count], 1));
              if (!Game1.random.GetChance(chance))
              {
                ++count;
                continue;
              }
              instance.dialogues.Add(source[count + 1]);
              count += 3;
              continue;
            }
            case "$f":
              foreach (var str3 in ArgUtility.SplitBySpace(source[count])[1].Split('/'))
                instance.farmer.DialogueQuestionsAnswered.Remove(str3);
              ++count;
              continue;
            case "$q":
              if (instance.dialogues.Count > 0)
                instance.dialogues[^1] += "{";
              var strArray1 = ArgUtility.SplitBySpace(source[count]);
              var strArray2 = strArray1[1].Split('/');
              var flag1 = false;
              foreach (var t in strArray2)
              {
                if (!instance.farmer.DialogueQuestionsAnswered.Contains(t)) continue;
                          
                flag1 = true;
                break;
              }
              if (flag1 && strArray2[0] != "-1")
              {
                if (!strArray1[2].Equals("null"))
                {
                  source = ((IEnumerable<string>) source).Take<string>(count).Concat<string>((IEnumerable<string>) instance.speaker.Dialogue[strArray1[2]].Split('#')).ToArray<string>();
                  --count;
                  continue;
                }
                continue;
              }

              var ildi = ModEntry.Help.Reflection.GetField<bool>(typeof(Dialogue), "isLastDialogueInteractive");
              ildi.SetValue(true);
              //instance.isLastDialogueInteractive = true;
              continue;
            case "$r":
              var strArray3 = ArgUtility.SplitBySpace(source[count]);
              //ModEntry.Help.Reflection.GetField<List<NPCDialogueResponse>>(typeof(Dialogue),"playerResponses");
              var npcDialogueResponses = playerResponses.GetValue();
              if(npcDialogueResponses == null)
                playerResponses.SetValue(new List<NPCDialogueResponse>());
              var ildi2 = ModEntry.Help.Reflection.GetField<bool>(typeof(Dialogue), "isLastDialogueInteractive");
              ildi2.SetValue(true);
              playerResponses.GetValue().Add(new NPCDialogueResponse(strArray3[1], Convert.ToInt32(strArray3[2]), strArray3[3], source[count + 1]));
              ++count;
              continue;
            case "$p":
              var strArray4 = ArgUtility.SplitBySpace(source[count]);
              var strArray5 = source[count + 1].Split('|');
              var flag2 = false;
              for (var index = 1; index < strArray4.Length; ++index)
              {
                if (instance.farmer.DialogueQuestionsAnswered.Contains(strArray4[1]))
                {
                  flag2 = true; 
                  break;
                }
              }
              if (flag2)
              {
                source = strArray5[0].Split('#');
                count = -1;
                continue;
              }
              source[count + 1] = ((IEnumerable<string>) source[count + 1].Split('|')).Last<string>();
              continue;
            case "$d":
              var strArray6 = ArgUtility.SplitBySpace(source[count]);
              var str4 = masterString.Substring(masterString.IndexOf('#') + 1);
              var flag3 = false;
              switch (strArray6[1].ToLower())
              {
                case "joja":
                  flag3 = Game1.isLocationAccessible("JojaMart");
                  break;
                case "cc":
                case "communitycenter":
                  flag3 = Game1.isLocationAccessible("CommunityCenter");
                  break;
                case "bus":
                  flag3 = instance.farmer.mailReceived.Contains("ccVault");
                  break;
                case "kent":
                  flag3 = Game1.year >= 2;
                  break;
              }
              var separator = str4.Contains('|') ? '|' : '#';
              source = !flag3 ? str4.Split(separator)[1].Split('#') : str4.Split(separator)[0].Split('#');
              --count;
              continue;
            case "$y":
              var quickResponse = ModEntry.Help.Reflection.GetField<bool>(typeof(Dialogue), "quickResponse");
              quickResponse.SetValue(true);
                        
              var ildi3 = ModEntry.Help.Reflection.GetField<bool>(typeof(Dialogue), "isLastDialogueInteractive");
              ildi3.SetValue(true);
                        
              var quickResponses = ModEntry.Help.Reflection.GetField<List<string>>(typeof(Dialogue), "quickResponses");
              if (quickResponses.GetValue() == null)
                quickResponses.SetValue(new List<string>());
                        
              if (playerResponses.GetValue() == null)
                playerResponses.SetValue(new List<NPCDialogueResponse>());
                        
              var str5 = source[count].Substring(source[count].IndexOf('\'') + 1);
              var strArray7 = str5.Substring(0, str5.Length - 1).Split('_'); instance.dialogues.Add(strArray7[0]);
              for (var index = 1; index < strArray7.Length; index += 2)
              {
                playerResponses.GetValue().Add(new NPCDialogueResponse((string) null, -1, "quickResponse" + index.ToString(), Game1.parseText(strArray7[index])));
                quickResponses.GetValue().Add(strArray7[index + 1].Replace("*", "#$b#"));
              }
              continue;
            case "$n":
              //idk custom behavior? doesn't seem needed rn
              continue;
            default:
              if (source[count].Contains("^"))
              {
                if (instance.farmer.IsMale)
                {
                  instance.dialogues.Add(source[count].Substring(0, source[count].IndexOf("^")));
                  continue;
                }
                instance.dialogues.Add(source[count].Substring(source[count].IndexOf("^") + 1));
                continue;
              }
              if (source[count].Contains('¦'))
              {
                if (instance.farmer.IsMale)
                {
                  instance.dialogues.Add(source[count].Substring(0, source[count].IndexOf("¦")));
                  continue;
                }
                instance.dialogues.Add(source[count].Substring(source[count].IndexOf("¦") + 1));
                continue;
              }
              instance.dialogues.Add(source[count]);
              continue;
          }
        }
      }

      return true;
    }
    //for changing NPCs mid dialogue
    internal static bool PrefixCurrentDialogueForDisplay(Dialogue instance)
    {
      try
      {
        if (ModEntry.Config.Debug)
        {
          ModEntry.Mon.Log("Current dialogue: " + instance.dialogues[instance.currentDialogueIndex], LogLevel.Info);
        }
                
        if (instance.dialogues.Count == 0)
          return true;
                
        var str1 = Utility.ParseGiftReveals(instance.dialogues[instance.currentDialogueIndex]);

        if (str1.StartsWith("$npc"))
        {
          instance.showPortrait = true;
                    
          //get npc
          str1 = str1.Replace("$npc", "");
          var who = str1.Replace("$npc", null);
          if (ModEntry.Config.Debug)
            ModEntry.Mon.Log("new speaker for current dialogue: " + who, LogLevel.Info);
          instance.speaker = Utility.fuzzyCharacterSearch(who);
                    
          //go to next dialogue
          ++instance.currentDialogueIndex;
                    
          var checkDialogueAttributes = ModEntry.Help.Reflection.GetMethod(typeof(Dialogue), "checkForSpecialDialogueAttributes");
          checkDialogueAttributes.Invoke();
                    
          return false;
        }
      }
      catch (Exception e)
      { 
        ModEntry.Mon.Log("Error: "+e,LogLevel.Error); 
        throw;
      }

      return true;
    }
}