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
using JetBrains.Annotations;
using StardewValley;

namespace SDV.Shared.Abstractions
{
  public class DialogueWrapper : IDialogueWrapper
  {
    public DialogueWrapper([NotNull] Dialogue item) => GetBaseType = item;

    public DialogueWrapper(string masterDialogue, INPCWrapper speaker) : this(new Dialogue(masterDialogue, (NPC)speaker.GetBaseType)){}

    public string CurrentEmotion { get; set; }
    public IFarmerWrapper farmer { get; }
    public void setCurrentDialogue(string dialogue)
    {
    }

    public void addMessageToFront(string dialogue)
    {
    }

    public int getPortraitIndex() => 0;

    public void prepareDialogueForDisplay()
    {
    }

    public string getCurrentDialogue() => null;

    public bool isItemGrabDialogue() => false;

    public bool isOnFinalDialogue() => false;

    public bool isDialogueFinished() => false;

    public string checkForSpecialCharacters(string str) => null;

    public string exitCurrentDialogue() => null;

    public IEnumerable<INpcDialogueResponseWrapper> getNPCResponseOptions() => null;

    public IEnumerable<IResponseWrapper> getResponseOptions() => null;

    public bool isCurrentDialogueAQuestion() => false;

    public bool chooseResponse(IResponseWrapper response) => false;
    public Dialogue GetBaseType { get; }
  }
}
