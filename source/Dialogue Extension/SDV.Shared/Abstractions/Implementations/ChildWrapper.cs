/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using StardewValley;

namespace SDV.Shared.Abstractions
{
  public class ChildWrapper : NPCWrapper, IChildWrapper
  {
    public ChildWrapper(NPC npc) : base(npc)
    {
    }

    public bool isInCrib() => false;

    public void toss(IFarmerWrapper who)
    {
    }

    public void performToss(IFarmerWrapper who)
    {
    }

    public void doneTossing(IFarmerWrapper who)
    {
    }

    public void tenMinuteUpdate()
    {
    }

    public int GetChildIndex() => 0;

    public void toddlerReachedDestination(ICharacterWrapper c, IGameLocationWrapper l)
    {
    }

    public void resetForPlayerEntry(IGameLocationWrapper l)
    {
    }
  }
 }
