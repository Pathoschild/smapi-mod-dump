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
using System.Reflection;
using StardewValley;

namespace DialogueExtension.Tests.MockWrappers
{
  public class ExtendNPC : NPC
  {
    public ExtendNPC()
    {
    }

    public Dictionary<string, string> SetDialogue
    {
      get => Dialogue;
      set
      {
        var dfield = typeof(NPC).GetField("dialogue", BindingFlags.NonPublic | BindingFlags.Instance);
        dfield.SetValue(this, value);
      }
    }
  }
}
