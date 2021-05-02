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
    internal class SlimeRing : CustomRing
    {
        private SlimeFollower _slime;

        internal override Ring RingObject { get; }

        internal SlimeRing(Ring pairedRing)
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

            // Spawn slime
            _slime = new SlimeFollower(who.getTileLocation());

            location.characters.Add(_slime);
        }

        internal override void HandleUnequip(Farmer who, GameLocation location)
        {
            if (_slime != null)
            {
                location.characters.Remove(_slime);
                _slime = null;
            }
        }

        internal override void HandleNewLocation(Farmer who, GameLocation location)
        {
            // Ensure we can force a character to appear
            if (location.characters is null)
            {
                return;
            }

            // Spawn slime
            if (_slime is null)
            {
                _slime = new SlimeFollower(who.getTileLocation());
            }

            _slime.resetForNewLocation(who.getTileLocation());
            location.characters.Add(_slime);
        }

        internal override void HandleLeaveLocation(Farmer who, GameLocation location)
        {
            if (_slime != null)
            {
                location.characters.Remove(_slime);
            }
        }

        internal override void Update(Farmer who, GameLocation location)
        {

        }
    }
}
