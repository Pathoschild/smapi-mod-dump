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
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Common;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;

namespace DeliveryService.Framework
{public class DeliveryChest
    {
        /// <summary>The location or building which contains the chest.</summary>
        public ObjectLocation Location { get; }
        public DeliveryOptions DeliveryOptions { get; }

        /// <summary>The chest's tile position within its location or building.</summary>
        public Chest Chest { get; }

        public DeliveryChest(Chest chest, ObjectLocation location)
        {
            this.Chest = chest;
            this.Location = location;
            this.DeliveryOptions = new DeliveryOptions();
        }
        public DeliveryChest(Chest chest) : this(chest, LocationHelper.FindLocation(chest)) { }
        public bool Exists()
        {
            return (Chest != null && Location != null && LocationHelper.FindAtLocation(Location.Location, Chest));
        }
        public bool IsFridge()
        {
            return (this.Location.Location is FarmHouse house && Game1.player.HouseUpgradeLevel > 0 && house.fridge.Value == this.Chest);
        }
    }
}