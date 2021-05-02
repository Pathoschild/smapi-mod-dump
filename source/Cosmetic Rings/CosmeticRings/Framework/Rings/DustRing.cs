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
    internal class DustRing : CustomRing
    {
        private DustSprite _dustSprite;

        internal override Ring RingObject { get; }

        internal DustRing(Ring pairedRing)
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
            _dustSprite = new DustSprite(who.getTileLocation());

            location.characters.Add(_dustSprite);
        }

        internal override void HandleUnequip(Farmer who, GameLocation location)
        {
            if (_dustSprite != null)
            {
                location.characters.Remove(_dustSprite);
                _dustSprite = null;
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
            if (_dustSprite is null)
            {
                _dustSprite = new DustSprite(who.getTileLocation());
            }

            _dustSprite.resetForNewLocation(who.getTileLocation());
            location.characters.Add(_dustSprite);
        }

        internal override void HandleLeaveLocation(Farmer who, GameLocation location)
        {
            if (_dustSprite != null)
            {
                location.characters.Remove(_dustSprite);
            }
        }

        internal override void Update(Farmer who, GameLocation location)
        {

        }
    }
}
