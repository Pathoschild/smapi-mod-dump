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

namespace SDV.Shared.Abstractions
{
  public interface IHorseWrapper : INPCWrapper
  {
    Guid HorseId { get; set; }
    IFarmerWrapper rider { get; set; }
    IFarmerWrapper getOwner();
    void squeezeForGate();
    void dismount(bool from_demolish = false);
    void nameHorse(string name);
    void SyncPositionToRider();
    bool collideWith(IObjectWrapper o);
  }
}