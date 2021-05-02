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
    internal class RaindropRing : CustomRing
    {
        private RainCloud _rainCloud;

        internal override Ring RingObject { get; }

        internal RaindropRing(Ring pairedRing)
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

            // Spawn cloud
            _rainCloud = new RainCloud(who.getTileLocation(), 0, (float)Game1.random.Next(15) / 500f, (float)Game1.random.Next(-10, 0) / 50f, (float)Game1.random.Next(10) / 50f);

            location.critters.Add(_rainCloud);
        }

        internal override void HandleUnequip(Farmer who, GameLocation location)
        {
            if (_rainCloud != null)
            {
                location.critters.Remove(_rainCloud);
                _rainCloud = null;
            }
        }

        internal override void HandleNewLocation(Farmer who, GameLocation location)
        {
            // Ensure we can force a critter to appear
            if (location.critters is null)
            {
                location.critters = new List<Critter>();
            }

            // Spawn cloud
            _rainCloud = new RainCloud(who.getTileLocation(), 0, (float)Game1.random.Next(15) / 500f, (float)Game1.random.Next(-10, 0) / 50f, (float)Game1.random.Next(10) / 50f);

            location.critters.Add(_rainCloud);
        }

        internal override void HandleLeaveLocation(Farmer who, GameLocation location)
        {

        }

        internal override void Update(Farmer who, GameLocation location)
        {

        }
    }
}
