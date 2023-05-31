/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Projectiles;

namespace Archery.Framework.Interfaces.Internal.Events
{
    public class BaseEventArgs : IBaseEventArgs
    {
        public Vector2 Origin { get; init; }
    }

    public class WeaponFiredEventArgs : BaseEventArgs, IWeaponFiredEventArgs
    {
        public string WeaponId { get; init; }
        public string AmmoId { get; init; }
        public BasicProjectile Projectile { get; init; }
    }

    public class WeaponChargeEventArgs : BaseEventArgs, IWeaponChargeEventArgs
    {
        public string WeaponId { get; init; }
        public float ChargePercentage { get; init; }
    }

    public class CrossbowLoadedEventArgs : BaseEventArgs, ICrossbowLoadedEventArgs
    {
        public string WeaponId { get; init; }
        public string AmmoId { get; init; }
    }

    public class AmmoChangedEventArgs : BaseEventArgs, IAmmoChangedEventArgs
    {
        public string WeaponId { get; init; }
        public string AmmoId { get; init; }
    }

    public class AmmoHitMonsterEventArgs : WeaponFiredEventArgs, IAmmoHitMonsterEventArgs
    {
        public Monster Monster { get; init; }
        public int DamageDone { get; init; }
    }
}
