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
  public class SittableWrappoer : ISittableWrapper
  {
    bool ISittable.IsSittingHere(Farmer who) => false;

    public bool HasSittingFarmers() => false;

    void ISittable.RemoveSittingFarmer(Farmer farmer)
    {
    }

    public int GetSittingFarmerCount() => 0;

    public List<Vector2> GetSeatPositions(bool ignore_offsets = false) => null;

#pragma warning disable CS1066
    Vector2? ISittable.GetSittingPosition(Farmer who, bool ignore_offsets = false) => null;
#pragma warning restore CS1066

    Vector2? ISittable.AddSittingFarmer(Farmer who) => null;

    public int GetSittingDirection() => 0;

    public Rectangle GetSeatBounds() => default;

    bool ISittable.IsSeatHere(GameLocation location) => false;
    public bool IsSittingHere(IFarmerWrapper who) => false;

    public void RemoveSittingFarmer(IFarmerWrapper farmer)
    {
    }

    public Vector2? GetSittingPosition(IFarmerWrapper who, bool ignore_offsets = false) => null;

    public Vector2? AddSittingFarmer(IFarmerWrapper who) => null;

    public bool IsSeatHere(IGameLocationWrapper location) => false;
  }
}