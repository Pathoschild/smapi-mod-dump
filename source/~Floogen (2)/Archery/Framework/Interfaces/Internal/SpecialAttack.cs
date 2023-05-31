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
using StardewValley.Tools;
using System;
using System.Collections.Generic;

namespace Archery.Framework.Interfaces.Internal
{
    public class SpecialAttack : ISpecialAttack
    {
        public Slingshot Slingshot { get; init; }
        public GameTime Time { get; init; }
        public GameLocation Location { get; init; }
        public Farmer Farmer { get; init; }
        public List<object> Arguments { get; init; }

        internal WeaponType WeaponType { get; set; }
        internal Func<List<object>, string> GetName { get; set; }
        internal Func<List<object>, string> GetDescription { get; set; }
        internal Func<List<object>, int> GetCooldownInMilliseconds { get; set; }
    }
}
