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
    internal class JunimoRing : CustomRing
    {
        private JunimoFollower _junimoFollower;

        internal override Ring RingObject { get; }

        internal JunimoRing(Ring pairedRing)
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

            // Spawn Junimo
            _junimoFollower = new JunimoFollower(who.getTileLocation());

            location.characters.Add(_junimoFollower);
        }

        internal override void HandleUnequip(Farmer who, GameLocation location)
        {
            if (_junimoFollower != null)
            {
                location.characters.Remove(_junimoFollower);
                _junimoFollower = null;
            }
        }

        internal override void HandleNewLocation(Farmer who, GameLocation location)
        {
            // Ensure we can force a character to appear
            if (location.characters is null)
            {
                return;
            }

            // Spawn Junimo
            if (_junimoFollower is null)
            {
                _junimoFollower = new JunimoFollower(who.getTileLocation());
            }

            _junimoFollower.resetForNewLocation(who.getTileLocation());
            location.characters.Add(_junimoFollower);
        }

        internal override void HandleLeaveLocation(Farmer who, GameLocation location)
        {
            if (_junimoFollower != null)
            {
                location.characters.Remove(_junimoFollower);
            }
        }

        internal override void Update(Farmer who, GameLocation location)
        {

        }
    }
}
