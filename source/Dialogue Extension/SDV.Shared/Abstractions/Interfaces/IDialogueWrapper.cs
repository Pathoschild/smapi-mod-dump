/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using System.Collections.Generic;
using StardewValley;

namespace SDV.Shared.Abstractions
{
  public interface IDialogueWrapper : IWrappedType<Dialogue>
  {
    string CurrentEmotion { get; set; }
    IFarmerWrapper farmer { get; }
    void setCurrentDialogue(string dialogue);
    void addMessageToFront(string dialogue);
    int getPortraitIndex();
    void prepareDialogueForDisplay();
    string getCurrentDialogue();
    bool isItemGrabDialogue();
    bool isOnFinalDialogue();
    bool isDialogueFinished();
    string checkForSpecialCharacters(string str);
    string exitCurrentDialogue();
    IEnumerable<INpcDialogueResponseWrapper> getNPCResponseOptions();
    IEnumerable<IResponseWrapper> getResponseOptions();
    bool isCurrentDialogueAQuestion();
    bool chooseResponse(IResponseWrapper response);
  }
}
