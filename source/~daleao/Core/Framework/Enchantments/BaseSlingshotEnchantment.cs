/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Core.Framework.Enchantments;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using DaLion.Shared.Constants;
using StardewValley.Enchantments;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>The base class for <see cref="Slingshot"/> weapon enchantments.</summary>
[XmlType("Mods_DaLion_BaseSlingshotEnchantment")]
public class BaseSlingshotEnchantment : BaseEnchantment
{
    /// <inheritdoc />
    public override bool CanApplyTo(Item item)
    {
        return item is Slingshot slingshot && slingshot.QualifiedItemId != QualifiedWeaponIds.BasicSlingshot;
    }

    /// <summary>Raised when the <paramref name="slingshot"/> fires a <see cref="BasicProjectile"/>.</summary>
    /// <param name="slingshot">The <see cref="Slingshot"/>.</param>
    /// <param name="firedProjectile">The fired <see cref="BasicProjectile"/>.</param>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="firer">The <see cref="Farmer"/> who fired the shot.</param>
    public void OnFire(
        Slingshot slingshot,
        BasicProjectile firedProjectile,
        GameLocation location,
        Farmer firer)
    {
        this._OnFire(
            slingshot,
            firedProjectile,
            location,
            firer);
    }

    /// <inheritdoc cref="OnFire"/>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Copies vanilla style.")]
    protected virtual void _OnFire(
        Slingshot slingshot,
        BasicProjectile firedProjectile,
        GameLocation location,
        Farmer firer)
    {
    }
}
