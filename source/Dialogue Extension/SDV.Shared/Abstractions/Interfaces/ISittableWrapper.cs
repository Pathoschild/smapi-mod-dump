/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace SDV.Shared.Abstractions
{
  public interface ISittableWrapper : ISittable
  {
    bool IsSittingHere(IFarmerWrapper who);
    void RemoveSittingFarmer(IFarmerWrapper farmer);
    Vector2? GetSittingPosition(IFarmerWrapper who, bool ignore_offsets = false);
    Vector2? AddSittingFarmer(IFarmerWrapper who);
    bool IsSeatHere(IGameLocationWrapper location);
  }
}
