/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using Archipelago.MultiClient.Net.Models;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;
using ArchipelagoLocation = StardewArchipelago.Locations.InGameLocations.ArchipelagoLocation;

namespace StardewArchipelago.Locations
{
    internal class SoundPurchaseableArchipelagoLocation : ArchipelagoLocation
    {
        private string _sound;

        public SoundPurchaseableArchipelagoLocation(string locationName, IMonitor monitor, IModHelper modHelper, LocationChecker locationChecker, ArchipelagoClient archipelago, Hint[] myActiveHints, string sound) : this(locationName, locationName, monitor, modHelper, locationChecker, archipelago, myActiveHints, sound)
        {
        }

        public SoundPurchaseableArchipelagoLocation(string locationDisplayName, string locationName, IMonitor monitor, IModHelper modHelper, LocationChecker locationChecker, ArchipelagoClient archipelago, Hint[] myActiveHints, string sound) : base(locationDisplayName, locationName, monitor, modHelper, locationChecker, archipelago, myActiveHints)
        {
            _sound = sound;
        }

        public override bool actionWhenPurchased(string shopId)
        {
            Game1.playSound(_sound);
            return base.actionWhenPurchased(shopId);
        }
    }
}
