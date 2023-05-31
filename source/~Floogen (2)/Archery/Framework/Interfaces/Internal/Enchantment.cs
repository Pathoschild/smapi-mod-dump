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
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;

namespace Archery.Framework.Interfaces.Internal
{
    public class Enchantment : IEnchantment
    {
        public BasicProjectile Projectile { get; init; }
        public GameTime Time { get; init; }
        public GameLocation Location { get; init; }
        public Farmer Farmer { get; init; }
        public Monster? Monster { get; init; }
        public int? DamageDone { get; init; }
        public List<object> Arguments { get; init; }

        internal AmmoType AmmoType { get; set; }
        internal TriggerType TriggerType { get; set; }
        internal Func<List<object>, string> GetName { get; set; }
        internal Func<List<object>, string> GetDescription { get; set; }
    }
}
