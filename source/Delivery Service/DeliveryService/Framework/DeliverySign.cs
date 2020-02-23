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