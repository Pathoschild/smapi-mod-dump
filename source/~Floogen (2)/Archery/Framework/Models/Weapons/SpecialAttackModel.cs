/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Archery.Framework.Interfaces.Internal;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;
using System.Collections.Generic;

namespace Archery.Framework.Models.Weapons
{
    public class SpecialAttackModel
    {
        public string Id { get; set; }
        public bool TriggerAfterButtonRelease { get; set; }
        public List<object> Arguments { get; set; }

        internal ISpecialAttack Generate(Slingshot slingshot, GameTime time, GameLocation currentLocation, Farmer who)
        {
            return new SpecialAttack()
            {
                Slingshot = slingshot,
                Time = time,
                Location = currentLocation,
                Farmer = who,
                Arguments = this.Arguments
            };
        }
    }
}
