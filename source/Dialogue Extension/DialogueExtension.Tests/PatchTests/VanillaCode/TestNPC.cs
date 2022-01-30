/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using DialogueExtension.Tests.MockWrappers;
using SDV.Shared.Abstractions;

namespace DialogueExtension.Tests.PatchTests.VanillaCode
{
  public class TestNPC
  {
    public IGameWrapper Game;
    public dynamic Game1 => Game;
    //public IMockDialogueWrapper Dialogue;
    public INPCWrapper NPC;


    public IDialogueWrapper tryToRetrieveDialogue(
      string preface,
      int heartLevel,
      string appendToEnd = "")
    {
      var num = Game1.year;
      if (Game1.year > 2)
        num = 2;
      if (Game1.player.spouse != null && Game1.player.spouse.Length > 0 && appendToEnd.Equals(""))
      {
        var retrieveDialogue = tryToRetrieveDialogue(preface, heartLevel, "_inlaw_" + Game1.player.spouse);
        if (retrieveDialogue != null)
          return retrieveDialogue;
      }

      var str = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
      if (NPC.Name == "Pierre" && (Game1.isLocationAccessible("CommunityCenter") || Game1.player.HasTownKey) &&
          str == "Wed")
        str = "Sat";
      if (NPC.Dialogue.ContainsKey(preface + Game1.dayOfMonth + appendToEnd) && num == 1)
        return new MockDialogueWrapper(NPC.Dialogue[preface + Game1.dayOfMonth + appendToEnd], NPC);
      if (NPC.Dialogue.ContainsKey(preface + Game1.dayOfMonth + "_" + num + appendToEnd))
        return new MockDialogueWrapper(NPC.Dialogue[preface + Game1.dayOfMonth + "_" + num + appendToEnd], NPC);
      if (heartLevel >= 10 && NPC.Dialogue.ContainsKey(preface + str + "10" + appendToEnd))
      {
        if (!NPC.Dialogue.ContainsKey(preface + str + "10_" + num + appendToEnd))
          return new MockDialogueWrapper(NPC.Dialogue[preface + str + "10" + appendToEnd], NPC);
        return new MockDialogueWrapper(NPC.Dialogue[preface + str + "10_" + num + appendToEnd], NPC);
      }

      if (heartLevel >= 8 && NPC.Dialogue.ContainsKey(preface + str + "8" + appendToEnd))
      {
        if (!NPC.Dialogue.ContainsKey(preface + str + "8_" + num + appendToEnd))
          return new MockDialogueWrapper(NPC.Dialogue[preface + str + "8" + appendToEnd], NPC);
        return new MockDialogueWrapper(NPC.Dialogue[preface + str + "8_" + num + appendToEnd], NPC);
      }

      if (heartLevel >= 6 && NPC.Dialogue.ContainsKey(preface + str + "6" + appendToEnd))
      {
        if (!NPC.Dialogue.ContainsKey(preface + str + "6_" + num))
          return new MockDialogueWrapper(NPC.Dialogue[preface + str + "6" + appendToEnd], NPC);
        return new MockDialogueWrapper(NPC.Dialogue[preface + str + "6_" + num + appendToEnd], NPC);
      }

      if (heartLevel >= 4 && NPC.Dialogue.ContainsKey(preface + str + "4" + appendToEnd))
      {
        if (preface == "fall_" && str == "Mon" && NPC.Name.Equals("Penny") &&
            Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
        {
          if (!NPC.Dialogue.ContainsKey(preface + str + "_" + num + appendToEnd))
            return new MockDialogueWrapper(NPC.Dialogue["fall_Mon"], NPC);
          return new MockDialogueWrapper(NPC.Dialogue[preface + str + "_" + num + appendToEnd], NPC);
        }

        if (!NPC.Dialogue.ContainsKey(preface + str + "4_" + num))
          return new MockDialogueWrapper(NPC.Dialogue[preface + str + "4" + appendToEnd], NPC);
        return new MockDialogueWrapper(NPC.Dialogue[preface + str + "4_" + num + appendToEnd], NPC);
      }

      if (heartLevel >= 2 && NPC.Dialogue.ContainsKey(preface + str + "2" + appendToEnd))
      {
        if (!NPC.Dialogue.ContainsKey(preface + str + "2_" + num))
          return new MockDialogueWrapper(NPC.Dialogue[preface + str + "2" + appendToEnd], NPC);
        return new MockDialogueWrapper(NPC.Dialogue[preface + str + "2_" + num + appendToEnd], NPC);
      }

      if (!NPC.Dialogue.ContainsKey(preface + str + appendToEnd))
        return null;
      if (NPC.Name.Equals("Caroline") && Game1.isLocationAccessible("CommunityCenter") && preface == "summer_" &&
          str == "Mon")
        return new MockDialogueWrapper(NPC.Dialogue["summer_Wed"], NPC);
      if (!NPC.Dialogue.ContainsKey(preface + str + "_" + num + appendToEnd))
        return new MockDialogueWrapper(NPC.Dialogue[preface + str + appendToEnd], NPC);
      return new MockDialogueWrapper(NPC.Dialogue[preface + str + "_" + num + appendToEnd], NPC);
    }
  }
}