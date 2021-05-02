/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/CosmeticRings
**
*************************************************/

using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmeticRings.Framework.Rings
{
    internal abstract class CustomRing
    {
        internal abstract Ring RingObject { get; }

        internal abstract void HandleEquip(Farmer who, GameLocation location);

        internal abstract void HandleUnequip(Farmer who, GameLocation location);

        internal abstract void HandleNewLocation(Farmer who, GameLocation location);

        internal abstract void HandleLeaveLocation(Farmer who, GameLocation location);

        internal abstract void Update(Farmer who, GameLocation location);
    }
}
