/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

namespace Archery.Framework.Interfaces.Internal
{
    public class WeaponData : IWeaponData
    {
        public string WeaponId { get; init; }
        public WeaponType WeaponType { get; init; }
        public int? MagazineSize { get; init; }
        public int? AmmoInMagazine { get; set; }
        public float ChargeTimeRequiredMilliseconds { get; init; }
        public float ProjectileSpeed { get; init; }
        public IRandomRange DamageRange { get; init; }
    }
}
