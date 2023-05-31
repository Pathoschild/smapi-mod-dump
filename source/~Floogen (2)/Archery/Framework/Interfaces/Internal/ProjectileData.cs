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

namespace Archery.Framework.Interfaces.Internal
{
    public class ProjectileData : IProjectileData
    {
        public string AmmoId { get; init; }
        public Vector2? Position { get; set; }
        public Vector2? Velocity { get; set; }
        public float? InitialSpeed { get; init; }
        public float? Rotation { get; set; }
        public int? BaseDamage { get; set; }
        public float? BreakChance { get; set; }
        public float? CriticalChance { get; set; }
        public float? CriticalDamageMultiplier { get; set; }
        public float? Knockback { get; set; }
        public bool? DoesExplodeOnImpact { get; set; }
        public int? ExplosionRadius { get; set; }
        public int? ExplosionDamage { get; set; }
    }
}
