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
    internal class FireflyRing : CustomRing
    {
        private List<FireflyFollower> _fireflies;

        internal override Ring RingObject { get; }

        internal FireflyRing(Ring pairedRing)
        {
            RingObject = pairedRing;
            _fireflies = new List<FireflyFollower>();
        }

        internal override void HandleEquip(Farmer who, GameLocation location)
        {
            // Ensure we can force a critter to appear
            if (location.critters is null)
            {
                location.critters = new List<Critter>();
            }

            // Spawn firefly
            for (int x = 0; x < Game1.random.Next(1, 4); x++)
            {
                FireflyFollower firefly = new FireflyFollower(who.getTileLocation());
                _fireflies.Add(firefly);

                location.critters.Add(firefly);
            }
        }

        internal override void HandleUnequip(Farmer who, GameLocation location)
        {
            if (_fireflies != null)
            {
                foreach (var firefly in _fireflies)
                {
                    Game1.currentLightSources.Remove(firefly.light);
                    location.critters.Remove(firefly);
                }

                _fireflies.Clear();
            }
        }

        internal override void HandleNewLocation(Farmer who, GameLocation location)
        {
            // Ensure we can force a critter to appear
            if (location.critters is null)
            {
                location.critters = new List<Critter>();
            }

            if (_fireflies.Count == 0)
            {
                // Spawn firefly
                for (int x = 0; x < Game1.random.Next(3); x++)
                {
                    FireflyFollower firefly = new FireflyFollower(who.getTileLocation());
                    _fireflies.Add(firefly);

                    location.critters.Add(firefly);
                }
            }

            foreach (var firefly in _fireflies)
            {
                firefly.resetForNewLocation(who.getTileLocation());
                location.critters.Add(firefly);
            }
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
