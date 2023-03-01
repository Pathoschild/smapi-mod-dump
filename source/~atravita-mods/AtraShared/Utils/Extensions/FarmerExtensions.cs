/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace AtraShared.Utils.Extensions;

/// <summary>
/// Extensions on Farmer.
/// </summary>
public static class FarmerExtensions
{
    /// <summary>
    /// Faces the farmer towards the pixel location.
    /// </summary>
    /// <param name="farmer">Farmer to turn.</param>
    /// <param name="nonTilePosition">Pixel to turn to.</param>
    public static void FaceFarmerTowardsPosition(this Farmer farmer, Vector2 nonTilePosition)
    {
        Vector2 delta = nonTilePosition - farmer.Position;
        if (Math.Abs(delta.X) < Math.Abs(delta.Y))
        {
            farmer.FacingDirection = delta.Y <= 0 ? Game1.up : Game1.down;
        }
        else
        {
            farmer.FacingDirection = delta.X <= 0 ? Game1.left : Game1.right;
        }
    }

    /// <summary>
    /// Gets a facing direction corresponding to the direction an Farmer is facing.
    /// </summary>
    /// <param name="farmer">Farmer in question.</param>
    /// <returns>Direction they're facing.</returns>
    public static Vector2 GetFacingDirection(this Farmer farmer)
    {
        return farmer.facingDirection.Get() switch
        {
            Game1.up => -Vector2.UnitY,
            Game1.down => Vector2.UnitY,
            Game1.left => -Vector2.UnitX,
            Game1.right => Vector2.UnitX,
            _ => Vector2.Zero,
        };
    }
}