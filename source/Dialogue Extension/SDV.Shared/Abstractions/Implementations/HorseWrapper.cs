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
using StardewValley;

namespace SDV.Shared.Abstractions
{
  public class HorseWrapper : NPCWrapper, IHorseWrapper
  {

    public HorseWrapper(Character character) : base(character)
    {
    }

    public Guid HorseId { get; set; }
    public IFarmerWrapper rider { get; set; }
    public IFarmerWrapper getOwner() => null;

    public void squeezeForGate()
    {
    }

    public void dismount(bool from_demolish = false)
    {
    }

    public void nameHorse(string name)
    {
    }

    public void SyncPositionToRider()
    {
    }

    public bool collideWith(IObjectWrapper o) => false;
  }
}