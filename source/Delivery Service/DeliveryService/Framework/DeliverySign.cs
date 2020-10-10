/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AxesOfEvil/SV_DeliveryService
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Objects;

namespace DeliveryService.Framework
{
    internal class DeliverySign
    {
        /// <summary>The location or building which contains the chest.</summary>
        public GameLocation Location { get; }

        /// <summary>The chest's tile position within its location or building.</summary>
        public Sign Sign { get; }

        public DeliverySign(GameLocation location, Sign sign)
        {
            this.Sign = sign;
            this.Location = location;
        }
    }
}