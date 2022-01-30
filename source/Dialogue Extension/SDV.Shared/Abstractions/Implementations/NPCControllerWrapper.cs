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
using Microsoft.Xna.Framework;
using StardewValley;

namespace SDV.Shared.Abstractions
{
  public class NPCControllerWrapper : INPCControllerWrapper
  {
    public NPCControllerWrapper(NPCController item) => GetBaseType = item;

    public delegate void endBehavior();

    public NPCController GetBaseType { get; }
    public void destroyAtNextCrossroad()
    {
    }

    public bool update(GameTime time, IGameLocationWrapper location, IEnumerable<INPCControllerWrapper> allControllers) => false;
  }
}
