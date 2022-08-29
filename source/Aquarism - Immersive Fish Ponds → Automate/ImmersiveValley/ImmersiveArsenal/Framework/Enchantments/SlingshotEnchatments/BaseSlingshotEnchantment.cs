/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Framework.Enchantments;

#region using directives

using StardewValley.Projectiles;
using StardewValley.Tools;
using System.Xml.Serialization;

#endregion using directives

[XmlType("Mods_DaLion_BaseSlingshotEnchantment")]
public class BaseSlingshotEnchantment : BaseEnchantment
{
    public override bool CanApplyTo(Item item) => item is Slingshot && ModEntry.Config.EnableSlingshotEnchants;

    public void OnFire(Slingshot slingshot, BasicProjectile projectile, GameLocation location, Farmer farmer)
    {
        _OnFire(slingshot, projectile, location, farmer);
    }

    protected virtual void _OnFire(Slingshot slingshot, BasicProjectile projectile, GameLocation location, Farmer farmer)
    {
    }

    protected virtual void _OnCollisionWithMonster(Slingshot slingshot, BasicProjectile projectile, GameLocation location, Farmer farmer)
    {
    }
}