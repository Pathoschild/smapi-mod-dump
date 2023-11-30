/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Enchantments;

#region using directives

using System.Linq;
using System.Xml.Serialization;
using DaLion.Overhaul.Modules.Combat.Projectiles;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>Summons two "echoes" of the fired projectile. The phantoms automatically aim at the nearest enemy after a short delay.</summary>
[XmlType("Mods_DaLion_RunaanEnchantment")]
public sealed class RunaanEnchantment : BaseSlingshotEnchantment
{
    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Enchantments_Runaan_Name();
    }

    /// <inheritdoc />
    protected override void _OnFire(
        Slingshot slingshot,
        BasicProjectile projectile,
        float overcharge,
        Vector2 startingPosition,
        float xVelocity,
        float yVelocity,
        float rotationVelocity,
        GameLocation location,
        Farmer firer)
    {
        if (projectile is not ObjectProjectile @object || slingshot.Get_IsOnSpecial() ||
            !location.characters.OfType<Monster>().Any())
        {
            return;
        }

        var velocity = new Vector2(xVelocity, yVelocity);
        var speed = velocity.Length() * overcharge;
        var facingDirectionVector = ((FacingDirection)firer.FacingDirection).ToVector() * 64f;

        // do clockwise projectile
        var runaanStartingPosition = startingPosition + facingDirectionVector.Rotate(30);
        var clockwise = new RunaanProjectile(
                @object.Ammo,
                @object.TileSheetIndex,
                slingshot,
                firer,
                overcharge,
                runaanStartingPosition,
                speed,
                rotationVelocity);
        firer.currentLocation.projectiles.Add(clockwise);

        // do anti-clockwise projectile
        runaanStartingPosition = startingPosition + facingDirectionVector.Rotate(-30);
        var antiClockwise = new RunaanProjectile(
                @object.Ammo,
                @object.TileSheetIndex,
                slingshot,
                firer,
                overcharge,
                runaanStartingPosition,
                speed,
                rotationVelocity);
        firer.currentLocation.projectiles.Add(antiClockwise);
    }
}
