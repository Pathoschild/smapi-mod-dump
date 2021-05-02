/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/CosmeticRings
**
*************************************************/

using CosmeticRings.Framework.Critters;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmeticRings.Framework.Rings
{
    internal class FairyRing : CustomRing
    {
        private Fairy _fairy;

        internal override Ring RingObject { get; }

        internal FairyRing(Ring pairedRing)
        {
            RingObject = pairedRing;
        }

        internal override void HandleEquip(Farmer who, GameLocation location)
        {
            // Ensure we can force a critter to appear
            if (location.critters is null)
            {
                location.critters = new List<Critter>();
            }

            // Spawn butterfly
            _fairy = new Fairy(who.getTileLocation());

            location.critters.Add(_fairy);
        }

        internal override void HandleUnequip(Farmer who, GameLocation location)
        {
            if (_fairy != null)
            {
                Game1.currentLightSources.Remove(_fairy.light);
                location.critters.Remove(_fairy);
                _fairy = null;
            }
        }

        internal override void HandleNewLocation(Farmer who, GameLocation location)
        {
            // Ensure we can force a critter to appear
            if (location.critters is null)
            {
                location.critters = new List<Critter>();
            }

            if (_fairy is null)
            {
                // Spawn butterfly
                _fairy = new Fairy(who.getTileLocation());
            }

            _fairy.resetForNewLocation(who.getTileLocation());
            location.critters.Add(_fairy);
        }

        internal override void HandleLeaveLocation(Farmer who, GameLocation location)
        {

        }

        internal override void Update(Farmer who, GameLocation location)
        {
            // Do nothing
        }
    }
}
