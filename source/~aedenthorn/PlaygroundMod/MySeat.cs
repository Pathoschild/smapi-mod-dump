/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System.Collections.Generic;

namespace PlaygroundMod
{
    public class MySeat : ISittable
    {
        public Vector2? AddSittingFarmer(Farmer who)
        {
            return Vector2.Zero;
        }

        public Rectangle GetSeatBounds()
        {
            return Rectangle.Empty;
        }

        public List<Vector2> GetSeatPositions(bool ignore_offsets = false)
        {
            return new();
        }

        public int GetSittingDirection()
        {
            return 1;
        }

        public int GetSittingFarmerCount()
        {
            return 1;
        }

        public Vector2? GetSittingPosition(Farmer who, bool ignore_offsets = false)
        {
            return Vector2.Zero;
        }

        public bool HasSittingFarmers()
        {
            return true;
        }

        public bool IsSeatHere(GameLocation location)
        {
            return true;
        }

        public bool IsSittingHere(Farmer who)
        {
            return true;
        }

        public void RemoveSittingFarmer(Farmer farmer)
        {

        }
    }
}