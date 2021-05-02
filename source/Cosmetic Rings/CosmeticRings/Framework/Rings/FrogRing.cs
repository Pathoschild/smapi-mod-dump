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
    internal class FrogRing : CustomRing
    {
        private FrogFollower _frog;

        internal override Ring RingObject { get; }

        internal FrogRing(Ring pairedRing)
        {
            RingObject = pairedRing;
        }

        internal override void HandleEquip(Farmer who, GameLocation location)
        {
            // Ensure we can force a character to appear
            if (location.characters is null)
            {
                return;
            }

            // Spawn rabbit
            _frog = new FrogFollower(who.getTileLocation());

            location.characters.Add(_frog);
        }

        internal override void HandleUnequip(Farmer who, GameLocation location)
        {
            if (_frog != null)
            {
                location.characters.Remove(_frog);
                _frog = null;
            }
        }

        internal override void HandleNewLocation(Farmer who, GameLocation location)
        {
            // Ensure we can force a character to appear
            if (location.characters is null)
            {
                return;
            }

            // Spawn rabbit
            if (_frog is null)
            {
                _frog = new FrogFollower(who.getTileLocation());
            }

            _frog.resetForNewLocation(who.getTileLocation());
            location.characters.Add(_frog);
        }

        internal override void HandleLeaveLocation(Farmer who, GameLocation location)
        {
            if (_frog != null)
            {
                location.characters.Remove(_frog);
            }
        }

        internal override void Update(Farmer who, GameLocation location)
        {

        }
    }
}
