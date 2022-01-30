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
using StardewValley.Objects;

namespace SDV.Shared.Abstractions
{
  public interface IBedFurnitureWrapper : IFurnitureWrapper
  {
    BedFurniture.BedType bedType { get; set; }
    bool IsBeingSleptIn(IGameLocationWrapper location);
    void ReserveForNPC();
    bool CanModifyBed(IGameLocationWrapper location, IFarmerWrapper who);

    Point GetBedSpot();
    void UpdateBedTile(bool check_bounds);
  }
}