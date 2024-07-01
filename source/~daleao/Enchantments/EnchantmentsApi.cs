/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Enchantments;

#region using directives

using DaLion.Enchantments.Framework.Projectiles;
using Microsoft.Xna.Framework;
using StardewValley.Projectiles;

#endregion using directives

/// <summary>The <see cref="EnchantmentsMod"/> API implementation.</summary>
public class EnchantmentsApi : IEnchantmentsApi
{
    /// <inheritdoc />
    public BasicProjectile CreateQuincyProjectile(
        Farmer firer, Vector2 startingPosition, float xVelocity, float yVelocity, float rotationVelocity, float scale)
    {
        return new QuincyProjectile(firer, startingPosition, xVelocity, yVelocity, rotationVelocity, scale);
    }
}
