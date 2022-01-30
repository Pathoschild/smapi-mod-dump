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
using StardewValley.Objects;

namespace SDV.Shared.Abstractions
{
  public class BedFurnitureWrapper : FurnitureWrapper, IBedFurnitureWrapper
  {
    public BedFurnitureWrapper(Object item) : base(item)
    {
    }

    public BedFurniture.BedType bedType { get; set; }
    public bool IsBeingSleptIn(IGameLocationWrapper location) => false;

    public void ReserveForNPC()
    {
    }

    public bool CanModifyBed(IGameLocationWrapper location, IFarmerWrapper who) => false;

    public Point GetBedSpot() => default;

    public void UpdateBedTile(bool check_bounds)
    {
    }
  }
}
