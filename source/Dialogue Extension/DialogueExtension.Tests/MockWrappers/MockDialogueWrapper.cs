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
using System.Linq;
using System.Runtime.CompilerServices;
using SDV.Shared.Abstractions;
using StardewValley;

namespace DialogueExtension.Tests.MockWrappers
{
  public class MockDialogueWrapper : IDialogueWrapper
  {
    public MockDialogueWrapper(IEnumerable<object> args) : this((string)args.ToArray()[0], (INPCWrapper)args.ToArray()[1])
    {

    }
    public MockDialogueWrapper(string dialogue, INPCWrapper npc)
    {
      CurrentEmotion = dialogue;
    }
    public Dialogue GetBaseType { get; }
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

    public IEnumerable<INpcDialogueResponseWrapper> getNPCResponseOptions()
    {
      yield break;
    }

    public IEnumerable<IResponseWrapper> getResponseOptions()
    {
      yield break;
    }

    public bool isCurrentDialogueAQuestion() => false;

    public bool chooseResponse(IResponseWrapper response) => false;
  }
}
